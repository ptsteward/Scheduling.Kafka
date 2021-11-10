using Carvana.Sched.Scheduling.Contracts.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaTesting
{
    public class TopicProducer
    {
        public async Task ProduceAsync()
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",
                Acks = Acks.Leader,
            };
            var schemaConfig = new SchemaRegistryConfig()
            {
                Url = "localhost:8081",
            };
            var producerTopic = "RESOURCES";
            var rand = new Random();

            using var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
            var produceTask = Task.Run(async () =>
            {
                var builder = new ProducerBuilder<string, Resource>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<Resource>(schemaClient))
                .SetErrorHandler((p, e) => Console.WriteLine(e));

                using var producer = builder.Build();
                Console.WriteLine($"Producing messages to Topic:{producerTopic}");
                while (true)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"Producing new message");
                    
                    var msg = ProduceMessage(rand);
                    
                    await producer.ProduceAsync(producerTopic, msg);
                    Console.WriteLine("Produced new message");
                }
            });
            await produceTask;
        }

        private Message<string, Resource> ProduceMessage(Random rand)
        {
            var key = rand.Next(0, 2) == 0 ? "xyz" : "abc";
            var msg = new Message<string, Resource>()
            {
                Key = key,
                Value = new Resource()
                {
                    Instance = new Instance()
                    {
                        Identity = new Identity()
                        {
                            IdentityGuid = Guid.NewGuid().ToString(),
                            IdentityKind = key
                        },
                        InstanceGuid = Guid.NewGuid().ToString(),
                    },
                }
            };
            msg.Value.Capabilities.AddRange(new[]
            {
                        new Capability()
                        {
                            Identity = new Identity()
                            {
                                IdentityGuid = Guid.NewGuid().ToString(),
                                IdentityKind = key
                            },
                        }
                    });
            foreach (var capability in msg.Value.Capabilities)
            {
                capability.Requirements.AddRange(new[]
                {
                            new Requirement()
                            {
                                Identity = new Identity()
                                {
                                    IdentityGuid = Guid.NewGuid().ToString(),
                                    IdentityKind = key,
                                }
                            }
                        });
            }
            return msg;
        }
    }
}
