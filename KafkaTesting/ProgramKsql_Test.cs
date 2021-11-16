using Carvana.Sched.Scheduling.Contracts.Kafka;
using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Extensions;
using KafkaTesting.MessageProducers;
using KafkaTesting.MessageProducers.Messages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace KafkaTesting
{
    public class ProgramKsql_Test
    {
        public static async Task _Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection()
                    .AddKsqlStreamProvider(new Uri("http://localhost:8088"), Assembly.GetExecutingAssembly(), "KsqlQueries");
                var provider = services.BuildServiceProvider().GetRequiredService<IKsqlStreamProvider>();

                var stream = provider.ExecuteQueryAsync<Test2>("testing");
                var producer = new TopicProducer<Test>(new TestMessageProducer(), "test_topic");
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
