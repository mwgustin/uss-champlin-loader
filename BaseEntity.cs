using System;
using Newtonsoft.Json;

public class BaseEntity
{
    [JsonProperty("id")]
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
    public string PartitionKey { get; set; } = "UssChamplin:Crew";
    public string Type { get; set; } = "UssChamplin.Models.Crew";

}