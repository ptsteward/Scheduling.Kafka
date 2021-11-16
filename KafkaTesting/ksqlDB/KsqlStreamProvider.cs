using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Objects;
using System.Runtime.CompilerServices;

namespace KafkaTesting.ksqlDB
{
    public class KsqlStreamProvider : IKsqlStreamProvider
    {
        private readonly IKsqlQueryReader queryReader;
        private readonly IKsqlClient client;
        private readonly IKsqlStreamParser streamParser;

        public KsqlStreamProvider(IKsqlQueryReader queryReader, IKsqlClient client, IKsqlStreamParser streamParser)
        {
            this.queryReader = queryReader;
            this.client = client;
            this.streamParser = streamParser;
        }

        public async IAsyncEnumerable<T> ExecuteQueryAsync<T>(string queryName, [EnumeratorCancellation] CancellationToken token = default)
        {
            var query = await queryReader.GetKsqlQueryAsync(queryName);
            if (string.IsNullOrEmpty(query.Ksql))
                yield return default!;

            await using var stream = await client.ExecuteQueryAsync(query, token);
            using var reader = new StreamReader(stream);

            var header = await GetHeaderAsync(reader);

            try
            {
                await foreach (var item in streamParser.ParseStreamAsync<T>(reader, header))
                    yield return item;
            }
            finally
            {
                reader?.Dispose();
                await client.CloseStreamAsync(header.QueryId);
            }
        }

        private async Task<StreamHeader> GetHeaderAsync(StreamReader reader)
        {
            var header = await streamParser.ParseStreamHeaderAsync(reader);
            return header;
        }
    }
}