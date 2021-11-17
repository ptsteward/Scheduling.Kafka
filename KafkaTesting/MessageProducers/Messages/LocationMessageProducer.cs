using Carvana.Sched.Scheduling.Contracts.Kafka;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaTesting.MessageProducers.Messages
{
    public class LocationMessageProducer : IMessageProducer<Location>
    {
        private static Random rand = new Random();

        public Message<string, Location> ProduceMessage()
        {
            var which = rand.Next(0, 2);
            var locations = new[] { "PHX", "MSP" };//, "TXS", "IWA" };

            var msg = new Message<string, Location>()
            {
                Key = "PHX",//locations[which],
                Value = new Location()
                {
                    ShortName = "PHX",//locations[which],
                    LegacyId = "1"
                }
            };

            return msg;
        }
    }
}
