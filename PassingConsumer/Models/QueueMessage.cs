using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PassingConsumer.Models
{
    public class QueueMessage
    {
        public string processName { get; set; }
        public int processNumber { get; set; }
        public DateTime earliestStart { get; set; }
        public DateTime latestStart { get; set; }
        public List<Argument> arguments { get; set; }
        public string JobId { get; set; }
    }

    public class Argument
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }


}
