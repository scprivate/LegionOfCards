using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Discord
{
    public class BotInterface
    {
        private static readonly string BaseUrl = "http://localhost:25320/maodu/api/";

        public static async Task<bool> GiveDuellist(string id)
        {
            string response = await PostRequest("giveduellist", id);
            if (string.IsNullOrEmpty(response)) return false;
            return response == "201";
        }

        private static async Task<string> PostRequest(string path, string data)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(BaseUrl + path, new StringContent(data));
                    if (response.Content == null) return null;
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
