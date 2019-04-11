using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LegionOfCards.Data.Net
{
    public class Packet
    {
        public string Key { get; set; }

        public object[] Args { get; set; }

        [JsonConstructor]
        public Packet()
        {
        }

        public Packet(string key, object[] args)
        {
            Key = key;
            Args = args;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
