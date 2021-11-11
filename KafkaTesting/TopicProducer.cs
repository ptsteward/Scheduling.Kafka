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
            var producerTopic = "test_topic";

            using var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
            var produceTask = Task.Run(async () =>
            {
                var builder = new ProducerBuilder<string, Test>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<Test>(schemaClient))
                .SetErrorHandler((p, e) => Console.WriteLine(e));

                using var producer = builder.Build();
                Console.WriteLine($"Producing messages to Topic:{producerTopic}");
                while (true)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"Producing new message");

                    var msg = TestMessageProducer.ProduceMessage();

                    await producer.ProduceAsync(producerTopic, msg);
                    Console.WriteLine("Produced new message");
                }
            });
            await produceTask;
        }        
    }
}
