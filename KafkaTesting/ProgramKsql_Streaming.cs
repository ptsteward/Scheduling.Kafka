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
    public class ProgramKsql_Streaming
    {
        private const string ksqlQueryName = "resources";

        public static async Task Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection()
                    .AddKsqlStreamProvider(new Uri("http://localhost:8088"), Assembly.GetExecutingAssembly(), "KsqlQueries");
                var provider = services.BuildServiceProvider().GetRequiredService<IKsqlStreamProvider>();

                var stream = provider.ExecuteQueryAsync<Resource>(ksqlQueryName);
                var producer = new TopicProducer<Resource>(new ResourceMessageProducer(), "resource_topic");
                var produceTask = producer.ProduceAsync();
                await foreach (var resource in stream)
                {
                    PrintItem(resource);
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExceptionFromRoot:{ex}");
            }
        }

        private static void PrintItem(Resource item)
        {
            Console.WriteLine($@"
STREAM:
Instance: {JsonConvert.SerializeObject(item?.Instance)}
Location:{item?.Location}
Capabilities: {JsonConvert.SerializeObject(item?.Capabilities)}");
        }
    }
}
