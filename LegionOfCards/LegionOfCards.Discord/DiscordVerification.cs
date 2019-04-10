using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace LegionOfCards.Discord
{
    public class DiscordVerification
    {
        public static event Action<string> VerificationSuccess;

        public static void Start()
        {
            var server = new Server("localhost", 25325, false, request => new HttpResponse());
            server.StaticRoutes.Add(HttpMethod.GET, "/discord/verify/success", request =>
            {
                if (request.Headers.ContainsKey("User"))
                {
                    VerificationSuccess?.Invoke(request.Headers["User"]);
                }

                return new HttpResponse(request, false, 400, null, "text/plain", "bad request", false);
            });
        }
    }
}
