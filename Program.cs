using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace uss_champlin_loader
{
    class Program
    {
        static ILogger _logger;
        static void Main(string[] args)
        {
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
        }
    }
}