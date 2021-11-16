using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Extensions;
using KafkaTesting.ksqlDB.Objects;
using Newtonsoft.Json;
using System.Reflection;

namespace KafkaTesting.ksqlDB
{
    public class KsqlQueryReader : IKsqlQueryReader
    {
        private readonly Assembly? ksqlAssembly;
        private readonly string ksqlRootNamespace;
        private readonly string ksqlNamespace;

        public KsqlQueryReader(Assembly ksqlQueryAssembly, string ksqlNamespace = "KsqlQueries")
        {
            this.ksqlAssembly = ksqlQueryAssembly ?? throw new ArgumentNullException(nameof(ksqlQueryAssembly));
            ksqlRootNamespace = this.ksqlAssembly?.GetName().Name ?? string.Empty;
            this.ksqlNamespace = ksqlNamespace;
        }

        public async Task<KsqlQuery> GetKsqlQueryAsync(string queryName)
        {
            var raw = await GetRawQueryStringAsync(queryName);
            if (string.IsNullOrEmpty(raw)) return new KsqlQuery();

            (string query, Dictionary<string, string> options) = ParseQuery(raw);

            return new KsqlQuery()
            {
                Ksql = query,
                StreamProperties = options
            };
        }

        private async Task<string> GetRawQueryStringAsync(string queryName)
        {
            var resourceName = $"{ksqlRootNamespace}.{ksqlNamespace}.{queryName}.ksql";

            using var stream = ksqlAssembly!.GetManifestResourceStream(resourceName);
            if (stream is null) return string.Empty;

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private (string query, Dictionary<string, string> options) ParseQuery(string input)
        {
            var query = input.Substring(0, input.LastIndexOf(";") + 1);
            var optionsRaw = input.Substring(input.LastIndexOf(";") + 1).Trim();
            var optionsMaybe = JsonConvert.DeserializeObject<Dictionary<string, string>>(optionsRaw);
            var options = optionsMaybe.DefaultOptionsIfEmpty();
            return (query, options);
        }
    }
}
