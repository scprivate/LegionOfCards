using LegionOfCards.Data.Controllers;
using LegionOfCards.Data.Models;

namespace LegionOfCards.Server.Account
{
    public class Session
    {
        public User User { get; }

        public string Token { get; set; }

        public Session(string userID, string token)
        {
            User = UserController.GetUser(userID);
            Token = token;
        }
    }
}
