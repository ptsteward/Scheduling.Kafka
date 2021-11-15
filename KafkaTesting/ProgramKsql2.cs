using Carvana.Sched.Scheduling.Contracts.Kafka;
using KafkaTesting.ksqlDB;
using Newtonsoft.Json;
using System.Net;

namespace KafkaTesting
{
    public class ProgramKsql2
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var x = new KsqlQueryExecutor(new KsqlClient(new HttpClient()
                {
                    BaseAddress = new Uri("http://localhost:8088"),
                    DefaultRequestVersion = HttpVersion.Version20,
                    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
                }), new KsqlStreamParser(new KsqlRowParser()));
                var en = x.ExecuteQuery<Test2>(new KsqlQuery()
                {
                    Ksql = "SELECT * FROM test_tables5 EMIT CHANGES;"
                });
                var producer = new TopicProducer();
                var produceTask = producer.ProduceAsync();
                await foreach (var test in en)
                {
                    Console.WriteLine($@"
                    STREAM:
                    Complex: {JsonConvert.SerializeObject(test?.Complex)}
                    Identity:{JsonConvert.SerializeObject(test?.Identity)}
                    Capabilities: {test?.Capabilities}
                    Amap: {JsonConvert.SerializeObject(test?.Amap)}");
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception:{ex}");
            }
        }
    }
}
