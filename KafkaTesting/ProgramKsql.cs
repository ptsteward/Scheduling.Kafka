using Carvana.Sched.Scheduling.Contracts.Kafka;
using ksqlDB.RestApi.Client.KSql.Linq;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using ksqlDB.RestApi.Client.KSql.RestApi.Parameters;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaTesting
{
    public class ProgramKsql
    {
        public class Resource2
        {
            public Instance2 Instance { get; set; }
        }

        public class Instance2
        {
            public Identity2 Identity { get; set; }
        }

        public class Identity2
        {
            public string IdentityGuid { get; set; }
        }

        public static async Task Main(string[] args)
        {
            var context = new KSqlDBContext(@"http://localhost:8088");
            var param = new QueryStreamParameters()
            {
                AutoOffsetReset = AutoOffsetReset.Earliest,
                //Sql = "SELECT Identity FROM Resources EMIT CHANGES;"
            };
            param.Properties.Add("VALUE_FORMAT", "PROTOBUF");
            var stream = context.CreateQueryStream<Resource2>(param);           

            var producer = new TopicProducer();
            var produceTask = producer.ProduceAsync();

            await foreach(var resource in stream)
            {
                Console.WriteLine($"IdentityGuid:{resource.Instance.Identity.IdentityGuid}");
            }
            try
            {
                await producer.ProduceAsync();                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProduceException: {ex}");
            }
        }       
    }
}
