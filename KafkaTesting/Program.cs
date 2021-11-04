using System;
using Carvana.Sched.Scheduling.Contracts.Kafka;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf.WellKnownTypes;

namespace KafkaTesting
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",
                Acks = Acks.Leader,
            };
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",
                GroupId = "1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
            };
            var consumeAllConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",
                GroupId = "2",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            var schemaConfig = new SchemaRegistryConfig()
            {
                Url = "localhost:8081",
            };
            var producerTopic = "capability_topic";
            var consumerTopic = "capability_stream_topic";
            var partitions = 10;
            var rand = new Random();
            using var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
            var produceTask = Task.Run(async () =>
            {
                var builder = new ProducerBuilder<string, Capability>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<Capability>(schemaClient))
                .SetErrorHandler((p, e) => Console.WriteLine(e));
                using var producer = builder.Build();
                Console.WriteLine($"Producing messages to Topic:{producerTopic}");
                while (true)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"Producing new message");
                    var key = rand.Next(0, 2) == 0 ? "xyz" : "abc";
                    var msg = new Message<string, Capability>()
                    {
                        Key = key,
                        Value = new Capability()
                        {
                            Identity = new Identity()
                            {
                                IdentityGuid = Guid.NewGuid().ToString(),
                                IdentityKind = key
                            }
                        }
                    };
                    await producer.ProduceAsync(producerTopic, msg);
                    Console.WriteLine("Produced new message");
                }
            });
            var consumerTasks = Enumerable.Range(0, partitions).Select(async p =>
            {
                var builder = new ConsumerBuilder<string, Capability>(consumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<Capability>().AsSyncOverAsync())
                .SetErrorHandler((p, e) => Console.WriteLine(e));
                using var consumer = builder.Build();
                Console.WriteLine($"Consumer {p} Topic:{consumerTopic} Partition:{p}");
                consumer.Assign(new TopicPartition(consumerTopic, new Partition(p)));
                consumer.Subscribe(consumerTopic);
                while (true)
                {
                    await Task.Delay(100);
                    var result = consumer.Consume();
                    Console.WriteLine(@$"
Consumer {p}: 
New Message Key:{result?.Message?.Key} 
IdentityKind:{result?.Message?.Value?.Identity?.IdentityKind} 
IdentityGuid:{result?.Message?.Value?.Identity?.IdentityGuid}");
                }
            });
            var consumeAllTask = Task.Run(async () =>
            {
                var builder = new ConsumerBuilder<string, Capability>(consumeAllConfig)
                .SetValueDeserializer(new ProtobufDeserializer<Capability>().AsSyncOverAsync())
                .SetErrorHandler((p, e) => Console.WriteLine(e));
                using var consumer = builder.Build();
                Console.WriteLine($"Consumer All Topic:{consumerTopic} Partition:{Partition.Any}");
                consumer.Assign(new TopicPartition(consumerTopic, Partition.Any));
                consumer.Subscribe(consumerTopic);
                while (true)
                {
                    await Task.Delay(100);
                    var result = consumer.Consume();
                    Console.WriteLine($@"
Consumer {result.Partition}: 
New Message Key:{result?.Message?.Key} 
IdentityKind:{result?.Message?.Value?.Identity?.IdentityKind} 
IdentityGuid:{result?.Message?.Value?.Identity?.IdentityGuid}");
                }
            });
            try
            {
                var completedTask = await Task.WhenAny(consumerTasks.Concat(new[] { produceTask, consumeAllTask }));
                await completedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
