using System.Runtime.CompilerServices;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlStreamContext
    {
        IAsyncEnumerable<T> ExecuteQueryAsync<T>(string queryName, IReadOnlyDictionary<string, string> options, CancellationToken token = default);
    }
}