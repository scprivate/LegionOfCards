using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Data.Stringle
{
    public class StringleConvert
    {
        private static readonly Type AttributeType = typeof(StringleDeserialize);

        public static T Deserialize<T>(string value) where T : StringleConvert
        {
            try
            {
                Type type = typeof(T);
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (method.ReturnType == type && Attribute.IsDefined(method, AttributeType))
                    {
                        return (T) method.Invoke(null, new object[] { value });
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
