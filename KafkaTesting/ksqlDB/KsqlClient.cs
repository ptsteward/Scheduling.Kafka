using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace KafkaTesting.ksqlDB
{
    public class KsqlClient
    {
        private const string KsqlMediaType = "application/vnd.ksqlapi.delimited.v1";

        private readonly HttpClient _client;

        public KsqlClient(HttpClient client) => _client = client;

        public async Task<Stream> ExecuteQueryAsync(KsqlQuery query, CancellationToken token = default)
        {
            var request = JsonConvert.SerializeObject(query);
            var msg = new HttpRequestMessage(HttpMethod.Post, "/query-stream")
            {
                Content = new StringContent(request, Encoding.UTF8, KsqlMediaType),
                Headers = { { "accept", MediaTypeWithQualityHeaderValue.Parse(KsqlMediaType).ToString() } },
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

            var response = await _client.SendAsync(msg,
                HttpCompletionOption.ResponseHeadersRead,
                token);

            Console.WriteLine($"{response.StatusCode} {response.ReasonPhrase}");
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<HttpStatusCode> CloseStreamAsync(string queryId, CancellationToken token = default)
        {
            var request = JsonConvert.SerializeObject(new { queryId });
            var msg = new HttpRequestMessage(HttpMethod.Post, "/close-query")
            {
                Content = new StringContent(request, Encoding.UTF8, MediaTypeNames.Application.Json),
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

            using var response = await _client.SendAsync(msg, token);

            return response.StatusCode;
        }
    }
}