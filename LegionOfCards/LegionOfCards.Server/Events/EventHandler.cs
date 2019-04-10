using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data.Net;
using LegionOfCards.Server.Frontend;

namespace LegionOfCards.Server.Events
{
    public class EventHandler
    {
        private static readonly Type EventAttribute = typeof(RemoteEvent);
        private readonly Dictionary<string, List<MethodInfo>> _eventCallbacks;

        internal EventHandler()
        {
            _eventCallbacks = new Dictionary<string, List<MethodInfo>>();
        }

        public void Add<T>()
        {
            Type type = typeof(T);
            foreach (MethodInfo callback in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.IsDefined(callback, EventAttribute))
                {
                    string name = callback.GetCustomAttribute<RemoteEvent>()?.Name;
                    if (name != null)
                    {
                        List<MethodInfo> callbacks = new List<MethodInfo>();
                        if (_eventCallbacks.ContainsKey(name))
                        {
                            callbacks = _eventCallbacks[name];
                            _eventCallbacks.Remove(name);
                        }
                        callbacks.Add(callback);
                        _eventCallbacks.Add(name, callbacks);
                    }
                }
            }
        }

        internal void Handle(Client sender, Packet packet)
        {
            if (_eventCallbacks.ContainsKey(packet.Key))
            {
                object[] args = new object[packet.Args.Length + 1];
                args[0] = sender;
                Array.Copy(packet.Args, 0, args, 1, packet.Args.Length);
                foreach (MethodInfo callback in _eventCallbacks[packet.Key])
                {
                    callback.Invoke(null, args);
                }
            }
        }
    }
}
