using KafkaTesting.ksqlDB.Objects;
using System.Net;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlClient
    {
        Task<HttpStatusCode> CloseStreamAsync(string queryId, CancellationToken token = default);
        Task<Stream> ExecuteQueryAsync(KsqlQuery query, CancellationToken token = default);
    }
}