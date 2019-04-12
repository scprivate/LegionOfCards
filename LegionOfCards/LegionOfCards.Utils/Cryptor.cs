using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;

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

        public static string GenerateSecret()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(rsa.ToXmlString(true)));
        }

        public static string GenerateSessionToken(string secret, string userID)
        {
            return new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(secret).AddClaim("user", userID)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds())
                .Build();
        }

        public static TokenResult ValidateSessionToken(string secret, string token, string userID)
        {
            try
            {
                Dictionary<string, object> data = new JwtBuilder().WithSecret(secret).MustVerifySignature().Decode<Dictionary<string, object>>(token);
                if ((string) data["user"] != userID) return TokenResult.WrongUser;
                return TokenResult.TokeFine;
            }
            catch (TokenExpiredException)
            {
                return TokenResult.TokenExpired;
            }
            catch (SignatureVerificationException)
            {
                return TokenResult.InvalidSignature;
            }
            catch (Exception)
            {
                return TokenResult.ValidationException;
            }
        }

        public enum TokenResult
        {
            TokenExpired, WrongUser, InvalidSignature, ValidationException, TokeFine
        }
    }
}
