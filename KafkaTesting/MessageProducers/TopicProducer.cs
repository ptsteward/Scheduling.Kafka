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
    public class TopicProducer<TMessage>
        where TMessage : class, IMessage<TMessage>, new()
    {
        private readonly IMessageProducer<TMessage> messageProducer;
        private readonly string topic = string.Empty;

        public TopicProducer(IMessageProducer<TMessage> messageProducer, string topic)
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
                //TransactionalId = "1"
            };
            var schemaConfig = new SchemaRegistryConfig()
            {
                Url = "localhost:8081",
            };

            using var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
            var produceTask = Task.Run(async () =>
            {
                var builder = new ProducerBuilder<string, TMessage>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<TMessage>(schemaClient))
                .SetErrorHandler((p, e) => Console.WriteLine(e));

                using var producer = builder.Build();
                //producer.InitTransactions(TimeSpan.FromSeconds(3));
                Console.WriteLine($"Producing messages to Topic:{topic}");
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"Producing new message");                                       
                    var msg = messageProducer.ProduceMessage();
                    
                    try
                    {
                        //producer.BeginTransaction();
                        await producer.ProduceAsync(topic, msg);
                        //producer.CommitTransaction();
                    }
                    catch (KafkaException ex)
                    {
                        //producer.AbortTransaction();
                        throw;
                    }

                    Console.WriteLine("Produced new message");
                }
            });
            await produceTask;
        }        
    }
}
