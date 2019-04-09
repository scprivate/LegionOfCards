using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Server.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RemoteEvent : Attribute
    {
        public string Name { get; }

        public RemoteEvent(string name)
        {
            Name = name;
        }
    }
}
