using System.Runtime.CompilerServices;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlStreamProvider
    {
        IAsyncEnumerable<T> ExecuteQueryAsync<T>(string queryName, CancellationToken token = default);
    }
}