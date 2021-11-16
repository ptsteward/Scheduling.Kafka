using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
