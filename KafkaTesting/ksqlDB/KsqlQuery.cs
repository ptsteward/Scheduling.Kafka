using Newtonsoft.Json;

namespace KafkaTesting.ksqlDB
{
    public sealed class Header
    {
        public string QueryId { get; set; }
        public string[] ColumnNames { get; set; }
        public string[] ColumnTypes { get; set; }
    }

    public sealed class Row
    {
        public object[] Columns { get; set; }
    }

    public sealed class KsqlQuery
    {
        [JsonProperty("sql")] 
        public string Ksql { get; set; }

        [JsonProperty("streamsProperties")] 
        public Dictionary<string, string> StreamProperties { get; } = new Dictionary<string, string>() { { "ksql.streams.auto.offset.reset", "earliest" } };
    }
}