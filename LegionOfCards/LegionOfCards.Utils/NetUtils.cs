using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegionOfCards.Utils
{
    public class NetUtils
    {
        public static string GetExternalIp()
        {
            try
            {
                var externalIp = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIp = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                    .Matches(externalIp)[0].ToString();
                return externalIp;
            }
            catch { return null; }
        }
    }
}
