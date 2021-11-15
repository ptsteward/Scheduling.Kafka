using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Objects;
using Newtonsoft.Json;

namespace KafkaTesting.ksqlDB
{
    public class KsqlStreamParser : IKsqlStreamParser
    {
        private readonly IKsqlRowParser rowParser;

        public KsqlStreamParser(IKsqlRowParser rowParser) => this.rowParser = rowParser;

        public async Task<StreamHeader> ParseStreamHeaderAsync(StreamReader reader)
        {
            var headerJson = await reader.ReadLineAsync();
            var header = JsonConvert.DeserializeObject<StreamHeader>(headerJson);
            return header;
        }

        public async IAsyncEnumerable<T> ParseStreamAsync<T>(StreamReader reader, StreamHeader header)
        {
            while (!reader.EndOfStream)
            {
                var rawJson = await reader.ReadLineAsync();
                var json = rowParser.ParseStreamRowToJson(rawJson ?? string.Empty, header.ColumnNames);
                var objT = JsonConvert.DeserializeObject<T>(json);
                yield return objT;
            }
        }
    }
}
