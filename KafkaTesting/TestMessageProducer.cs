using Carvana.Sched.Scheduling.Contracts.Kafka;
using Confluent.Kafka;

namespace KafkaTesting
{
    public static class TestMessageProducer
    {
        private static Random rand = new Random();

        public static Message<string, Test> ProduceMessage()
        {
            var which = rand.Next(0, 2);
            var key = which == 0 ? "xyz" : "abc";
            var locations = new[] { "PHX", "MSP" };//, "TXS", "IWA" };
            var instances1 = new[]
            {
                "1",
                "3",
                "5",
                "7",
                "9"
            };
            var instances2 = new[]
            {
                "2",
                "4",
                "6",
                "8",
                "0"
            };
            var msg = new Message<string, Test>()
            {
                Key = key,
                Value = new Test()
                {
                    Identity = new Identity()
                    {
                        IdentityGuid = key,
                        IdentityKind = locations[which],
                        IdentityType = 1
                    },                    
                }
            };
            
            msg.Value.Capabilities.AddRange(which == 0 && rand.Next(0, 2) == 0 ? instances1 : instances2);
            var testX = new TestX()
            {
                Identity = msg.Value.Identity
            };
            testX.Capabilities.Add(msg.Value.Capabilities);
            msg.Value.Amap.Add(1, testX);
            msg.Value.Amap.Add(2, testX);
            return msg;
        }
    }
}
