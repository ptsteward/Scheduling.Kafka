using KafkaTesting.ksqlDB.Objects;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlQueryReader
    {
        Task<KsqlQuery> GetKsqlQueryAsync(string queryName, IReadOnlyDictionary<string, string> options);
    }
}
