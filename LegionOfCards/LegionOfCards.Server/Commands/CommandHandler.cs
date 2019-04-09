using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Utils;

namespace LegionOfCards.Server.Commands
{
    public class CommandHandler
    {
        private static readonly Type CommandAttribute = typeof(Command);
        private readonly Dictionary<string, MethodInfo> _commandCallbacks;
        private readonly Dictionary<Type, Func<string, object>> _customDeserializers;

        internal CommandHandler()
        {
            _customDeserializers = new Dictionary<Type, Func<string, object>>
            {
                {typeof(int), val => int.Parse(val)}, {typeof(double), val => double.Parse(val)},
                {typeof(string), val => val}, {typeof(bool), val => bool.Parse(val)}
            };
            _commandCallbacks = new Dictionary<string, MethodInfo>();
        }

        public void Add<T>()
        {
            Type type = typeof(T);
            foreach (MethodInfo callback in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.IsDefined(callback, CommandAttribute))
                {
                    string name = callback.GetCustomAttribute<Command>()?.Name;
                    if (name != null)
                    {
                        _commandCallbacks.Add(name, callback);
                    }
                }
            }
        }

        public void RegisterDeserializer<T>(Func<string, object> func)
        {
            _customDeserializers.Add(typeof(T), func);
        }

        internal bool Handle(string line)
        {
            try
            {
                string[] parts = line.Split(' ');
                string name = parts[0];
                if (_commandCallbacks.ContainsKey(name))
                {
                    MethodInfo callback = _commandCallbacks[name];
                    object[] args = new object[parts.Length - 1];
                    if (parts.Length > 1)
                    {
                        for (int i = 1; i < parts.Length; i++)
                        {
                            Type paramType = callback.GetParameters()[i - 1].ParameterType;
                            if (_customDeserializers.ContainsKey(paramType))
                            {
                                args[i - 1] = _customDeserializers[paramType].Invoke(parts[i]);
                            }
                        }
                    }

                    callback.Invoke(null, args);
                    return true;
                }
            }
            catch
            {
                Logger.Fatal("Could not parse command '" + line + "' cause an parsing error occurred!");
            }

            return false;
        }
    }
}
