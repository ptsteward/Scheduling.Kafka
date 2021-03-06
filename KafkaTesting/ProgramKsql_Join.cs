using Carvana.Sched.Scheduling.Contracts.Kafka;
using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Extensions;
using KafkaTesting.MessageProducers;
using KafkaTesting.MessageProducers.Messages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTesting
{
    public class ProgramKsql_Join
    {
        private const string ksqlQueryName = "resources_push";

        public static async Task _Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection()
                    .AddKsqlStreamProvider(new Uri("http://localhost:8088"), Assembly.GetExecutingAssembly(), "KsqlQueries");
                var provider = services.BuildServiceProvider().GetRequiredService<IKsqlStreamContext>();

                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));
                var resources = new TopicProducer<Resource>(new ResourceMessageProducer2(), "resource_topic");
                var locations = new TopicProducer<Location>(new LocationMessageProducer(), "location_topic");
                //await Task.WhenAll(resources.ProduceAsync(cts.Token), locations.ProduceAsync(cts.Token));
                var _ = resources.ProduceAsync();
                var __ = locations.ProduceAsync();

                Console.WriteLine("Produce Done, Requesting State");
                var options = new Dictionary<string, string>()
                {
                    ["ksql.streams.auto.offset.reset"] = "earliest"
                };
                var stream = provider.ExecuteQueryAsync<ResourcesTable>(ksqlQueryName, options);
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

        private static void PrintItem(ResourcesTable item)
        {
            /*Console.WriteLine($@"
STREAM:
InstanceKey:{item?.InstanceKey}
LocationKey:{item?.LocationKey}");*/

            Console.WriteLine($@"
STREAM:
InstanceKey:{item?.InstanceKey}
LocationKey:{item?.LocationKey}
Instance: {JsonConvert.SerializeObject(item?.Instance)}
Location:{item?.Location}
Capabilities: {JsonConvert.SerializeObject(item?.Capabilities)}");
        }
    }
}
