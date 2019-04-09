using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Server.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        public string Name { get; }

        public Command(string name)
        {
            Name = name;
        }
    }
}