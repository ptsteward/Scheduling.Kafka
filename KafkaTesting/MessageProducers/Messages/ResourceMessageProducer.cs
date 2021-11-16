using Carvana.Sched.Scheduling.Contracts.Kafka;
using Confluent.Kafka;

namespace KafkaTesting.MessageProducers.Messages
{
    public class ResourceMessageProducer : IMessageProducer<Resource>
    {
        private static Random rand = new Random();

        public Message<string, Resource> ProduceMessage()
        {
            var which = rand.Next(0, 2);
            var key = which == 0 ? "xyz" : "abc";
            var locations = new[] { "PHX", "MSP" };//, "TXS", "IWA" };
            var instances1 = new[]
            {
                "1",
                "3",
                //"5",
                //"7",
                //"9"
            };
            var instances2 = new[]
            {
                "2",
                "4",
                //"6",
                //"8",
                //"0"
            };
            var msg = new Message<string, Resource>()
            {
                Key = key,
                Value = new Resource()
                {
                    Instance = new Instance()
                    {
                        Identity = new Identity()
                        {
                            IdentityGuid = key,
                            IdentityKind = locations[which]
                        },
                        InstanceGuid = "1"//which == 0 ? instances1[rand.Next(0, instances1.Length)] : instances2[rand.Next(0, instances2.Length)]
                    },
                    Location = "PHX"//locations[which]
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
                },
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
                    },
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
