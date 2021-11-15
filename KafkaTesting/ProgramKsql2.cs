using Carvana.Sched.Scheduling.Contracts.Kafka;
using KafkaTesting.ksqlDB;
using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace KafkaTesting
{
    public class ProgramKsql2
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection()
                    .AddKsqlStreamProvider(new Uri("http://localhost:8088"), Assembly.GetExecutingAssembly(), "KsqlQueries");
                var provider = services.BuildServiceProvider().GetRequiredService<IKsqlStreamProvider>();

                var stream = provider.ExecuteQueryAsync<Test2>("testing");
                var producer = new TopicProducer();
                var produceTask = producer.ProduceAsync();
                await foreach (var test in stream)
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
                Console.WriteLine($"ExceptionFromRoot:{ex}");
            }
        }
    }
}
