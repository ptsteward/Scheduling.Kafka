using Confluent.Kafka;
using Google.Protobuf;

namespace KafkaTesting.MessageProducers.Messages
{
    public interface IMessageProducer<TMessage>
        where TMessage : class, IMessage<TMessage>, new()
    {
        Message<string, TMessage> ProduceMessage();
    }
}
