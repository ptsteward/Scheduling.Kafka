using KafkaTesting.ksqlDB.Objects;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlStreamParser
    {
        IAsyncEnumerable<T> ParseStreamAsync<T>(StreamReader reader, StreamHeader header);
        Task<StreamHeader> ParseStreamHeaderAsync(StreamReader reader);
    }
}
