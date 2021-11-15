using Newtonsoft.Json;

namespace KafkaTesting.ksqlDB.Objects
{
    public class KsqlQuery
    {
        [JsonProperty("sql")]
        public string Ksql { get; init; } = string.Empty;

        [JsonProperty("streamsProperties")]
        public Dictionary<string, string> StreamProperties { get; init; } = new Dictionary<string, string>();
    }
}