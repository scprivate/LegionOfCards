using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Utils
{
    public class RandomHelper
    {
        public static readonly Random Random = new Random();

        public static string RandomString(int length, string choices = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            StringBuilder builder = new StringBuilder();
            char[] chars = choices.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                builder.Append(chars[Random.Next(chars.Length)]);
            }
            return builder.ToString();
        }
    }
}
