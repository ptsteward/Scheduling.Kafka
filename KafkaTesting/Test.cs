using Carvana.Sched.Scheduling.Contracts.Kafka;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carvana.Sched.Scheduling.Contracts.Kafka
{
    public partial class Test
    {
        public IEnumerable<string> Capabilities2 { get; set; }
        public string Capabilities3 { get; set; }
    }
}
