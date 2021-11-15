using Newtonsoft.Json;

namespace KafkaTesting.ksqlDB
{
    public class KsqlStreamParser
    {
        private readonly KsqlRowParser rowParser;

        public KsqlStreamParser(KsqlRowParser rowParser) => this.rowParser = rowParser;

        public async Task<Header> ParseStreamHeaderAsync(StreamReader reader)
        {
            var headerJson = await reader.ReadLineAsync();
            Console.WriteLine(headerJson);
            var header = JsonConvert.DeserializeObject<Header>(headerJson);
            return header;
        }

        public async IAsyncEnumerable<T> ParseStreamAsync<T>(StreamReader reader, Header header)
        {
            while (!reader.EndOfStream)
            {
                var rawJson = await reader.ReadLineAsync();
                Console.WriteLine($"RawJson: {rawJson}");
                var json = rowParser.ParseStreamRowToJson(rawJson ?? string.Empty, header.ColumnNames);
                Console.WriteLine($"Outgoing: {json}");
                var obj = JsonConvert.DeserializeObject<T>(json);
                yield return obj;
            }
        }
    }
}
