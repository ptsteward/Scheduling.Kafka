using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Protobuf;
using KafkaTesting.MessageProducers.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTesting.MessageProducers
{
    public class TopicProducer<T> where T : class, IMessage<T>, new()
    {
        private readonly IMessageProducer<T> messageProducer;
        private readonly string topic = string.Empty;

        public TopicProducer(IMessageProducer<T> messageProducer, string topic)
        {
            this.messageProducer = messageProducer;
            this.topic = topic;
        }

        public async Task ProduceAsync(CancellationToken token = default)
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

            using var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
            var produceTask = Task.Run(async () =>
            {
                var builder = new ProducerBuilder<string, T>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<T>(schemaClient))
                .SetErrorHandler((p, e) => Console.WriteLine(e));

                using var producer = builder.Build();
                Console.WriteLine($"Producing messages to Topic:{topic}");
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"Producing new message");

                    var msg = messageProducer.ProduceMessage();

                    await producer.ProduceAsync(topic, msg);
                    Console.WriteLine("Produced new message");
                }
            });
            await produceTask;
        }        
    }
}
