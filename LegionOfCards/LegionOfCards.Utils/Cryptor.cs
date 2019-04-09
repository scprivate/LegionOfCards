using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Utils
{
    public class Cryptor
    {
        public static string MD5(string textToHash)
        {
            if (string.IsNullOrEmpty(textToHash))
            {
                return string.Empty;
            }
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] textToHashBytes = Encoding.Default.GetBytes(textToHash);
            byte[] result = md5.ComputeHash(textToHashBytes);
            return BitConverter.ToString(result);
        }
    }
}
