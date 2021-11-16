//using Carvana.Sched.Scheduling.Contracts.Kafka;
//using Google.Protobuf.Collections;
//using KafkaTesting.MessageProducers;
//using ksqlDB.RestApi.Client.KSql.Linq;
//using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
//using ksqlDB.RestApi.Client.KSql.Query.Context;
//using ksqlDB.RestApi.Client.KSql.Query.Options;
//using ksqlDB.RestApi.Client.KSql.RestApi.Parameters;
//using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
//using Newtonsoft.Json;
//using System;
//using System.Threading.Tasks;

//namespace KafkaTesting
//{
//    public class ProgramKsql
//    {
//        public static async Task _Main(string[] args)
//        {
//            var producer = new TopicProducer();
//            var context = new KSqlDBContext(@"http://localhost:8088");
//            var queryParam = new QueryStreamParameters()
//            {
//                AutoOffsetReset = AutoOffsetReset.Latest,
//                Sql = @"SELECT * FROM REBUILT_STREAM EMIT CHANGES;"
//            };
//            //var stream = context.CreateQueryStream<Test>(queryParam);




//            var produceTask = producer.ProduceAsync();
//            try
//            {
//                //Console.WriteLine("Waiting 5s");
//                //await Task.Delay(5000);
//                //var table = context.CreatePullQuery<Resource>("resource_table_by_location").GetManyAsync();
//                //await foreach (var resource in stream)
//                //{
//                //    //Console.WriteLine($"IdentityGuid:{resource?.Instance?.Identity?.IdentityGuid}");
//                //    //Console.WriteLine($"IdentityGuid:{resource?.Identity?.IdentityGuid}");
//                //    //Console.WriteLine($"Key:{resource?.Instance?.Identity?.IdentityKind}, IdentityGuid:{resource?.Instance?.Identity?.IdentityGuid}");
//                //    Console.WriteLine($"Key:{resource?.Location} InstanceId:{resource?.Instance?.InstanceGuid}");// Count:{resource?.Count} Kind:{resource?.Instance?.Identity?.IdentityKind} Guid:{resource?.Instance?.Identity?.IdentityGuid}");
//                //}
//                //await foreach (var test in stream)
//                //{
//                //    Console.WriteLine($"STREAM: {JsonConvert.SerializeObject(test)}");
//                //    Console.WriteLine($"STREAM: IdentityGuid:{test?.Identity?.IdentityGuid} IdentityKind:{test?.Identity?.IdentityKind} Capabilities:{test?.Capabilities}");
//                //}
//                var tableParam = new QueryStreamParameters()
//                {
//                    AutoOffsetReset = AutoOffsetReset.Earliest,
//                    Sql = @"SELECT * FROM TEST_TABLES2 WHERE IdentityKind='MSP' OR IdentityKind='PHX';"
//                };
//                Console.WriteLine("Waiting 5s");
//                await Task.Delay(5000);
//                //var table = context.CreatePullQuery<Test>("test_table").Where(t => t.Identity.IdentityKind == "MSP").GetManyAsync();
//                var table = context.CreateQueryStream<KsqlPayload>(tableParam);
//                await foreach (var payload in table)
//                {
//                    var test = JsonConvert.DeserializeObject<Test>(payload.Payload);
//                    Console.WriteLine($"TABLE: {JsonConvert.SerializeObject(test)}");
//                    Console.WriteLine($"TABLE: IdentityGuid:{test?.Identity?.IdentityGuid} IdentityKind:{test?.Identity?.IdentityKind} Capabilities:{test?.Capabilities}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"ProduceException: {ex}");
//            }
//        }
//    }

//    public class KsqlPayload
//    {
//        public string Payload { get; set; }
//    }

//    public class PayloadDeserializer
//    {
//        public T Deserialize<T>(KsqlPayload payload)
//            => JsonConvert.DeserializeObject<T>(payload.Payload);
//    }
//}
