using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Fluent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace uss_champlin_loader
{
    class Program
    {
        // static readonly string ConnectionString = Environment.GetEnvironmentVariable("CosmosConnection");
        static ILogger _logger;
        static async Task Main(string[] args)
        {
            //setup secrets/builder stuff

            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();
            var Configuration = builder.Build();
            var ConnectionString = Configuration["CosmosConnection"];


            //setup logger
            var fac = LoggerFactory.Create(builder => 
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            
            _logger = fac.CreateLogger<Program>();
            
            //startup
            _logger.LogCritical("Beginning");
            Thread.Sleep(2000);

            _logger.LogInformation("---- Setup DB connection ----");
            var client = new CosmosClientBuilder(ConnectionString).WithBulkExecution(true).Build();
            var container = client.GetContainer("champlin", "champlin");
            
            
            _logger.LogInformation("---- Reading file ----");
            List<CrewDto> crewList = new List<CrewDto>();
            
            using(FileStream fs = File.OpenRead("crewList.json"))
            {
                crewList = await JsonSerializer.DeserializeAsync<List<CrewDto>>(fs);
            }

            _logger.LogInformation($"Fetched {crewList.Count} items");


            _logger.LogInformation("---- Upserting items now ----");
            List<Task> taskList = new List<Task>();
            foreach(var crew in crewList)
            {
                taskList.Add(container.UpsertItemAsync<CrewDto>(crew, new PartitionKey("UssChamplin:Crew")));
            }
            await Task.WhenAll(taskList);

            _logger.LogInformation("---- Finished ----");
        }
    }
}