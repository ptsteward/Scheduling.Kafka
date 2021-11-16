using Confluent.Kafka;
using Google.Protobuf;

namespace KafkaTesting.MessageProducers.Messages
{
    public interface IMessageProducer<T> where T : class, IMessage<T>, new()
    {
        Message<string, T> ProduceMessage();
    }
}
