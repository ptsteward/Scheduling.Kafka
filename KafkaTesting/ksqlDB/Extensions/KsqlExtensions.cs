using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KafkaTesting.ksqlDB.Extensions
{
    internal static class KsqlExtensions
    {
        public static Dictionary<string, string> DefaultOptionsIfEmpty(this Dictionary<string, string> options)
            => (options is not null && options.Any()) ?
            options
            :
            new Dictionary<string, string>()
            {
                ["ksql.streams.auto.offset.reset"] = "earliest"
            };

        public static bool TryParse(this string input, out JArray array)
        {
            try
            {
                var output = JArray.Parse(input);
                array = output;
                return true;
            }
            catch (JsonReaderException)
            {
                array = new JArray();
                return false;
            }
        }
    }
}
