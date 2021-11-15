using System.Runtime.CompilerServices;

namespace KafkaTesting.ksqlDB
{
    public class KsqlQueryExecutor
    {
        private readonly KsqlClient client;
        private readonly KsqlStreamParser streamParser;

        public KsqlQueryExecutor(KsqlClient client, KsqlStreamParser streamParser)
        {
            this.client = client;
            this.streamParser = streamParser;
        }

        public async IAsyncEnumerable<T> ExecuteQuery<T>(KsqlQuery query, [EnumeratorCancellation] CancellationToken token = default)
        {
            await using var queryStream = await client.ExecuteQueryAsync(query, token);
            using var reader = new StreamReader(queryStream);

            var header = await streamParser.ParseStreamHeaderAsync(reader);

            try
            {
                await foreach (var item in streamParser.ParseStreamAsync<T>(reader, header))
                    yield return item;
            }
            finally
            {
                var statusCode = await client.CloseStreamAsync(header.QueryId);
                Console.WriteLine($"CloseStream:{statusCode}");
            }
        }
    }
}