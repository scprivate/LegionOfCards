using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data;
using LegionOfCards.Data.Controllers;
using LegionOfCards.Data.Models;
using LegionOfCards.Server.Events;
using LegionOfCards.Server.Frontend;

namespace LegionOfCards.Server.Account
{
    public class AccountManager
    {
        private static readonly string DiscordVerificationUrl = "http://127.0.0.1:5000/discord/verify?user_id=";

        public int CheckCreationData(string username, string email)
        {
            int resultCode = 0;
            try
            {
                if (Database.Exists($"SELECT * FROM {Database.UserTable} WHERE Email = @mail",
                    new Tuple<string, object>("@mail", email)))
                {
                    resultCode = 1;
                }

                if (Database.Exists($"SELECT * FROM {Database.UserTable} WHERE Name = @name",
                    new Tuple<string, object>("@name", username)))
                {
                    resultCode += 2;
                }

                return resultCode;
            }
            catch
            {
                resultCode = -1;
            }

            return resultCode;
        }

        public bool CreateAccount(string username, string email, string password)
        {
            if (CheckCreationData(username, email) == 0)
            {
                try
                {
                    UserController.CreateUser(username, email, password);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool DeleteAccount(Client client, string accessToken)
        {
            try
            {
                string userID = WebServer.Instance.Sessions.ValidateSession(client, accessToken);
                if (userID != null)
                {
                    UserController.DeleteUser(userID);
                    WebServer.Instance.Sessions.DestroySession(client, accessToken);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateDiscordVerifyUrl(string userID)
        {
            return DiscordVerificationUrl + userID;
        }

        [RemoteEvent("delete-account")]
        public static void OnDeleteAccount(Client client, string accessToken)
        {
            if(!WebServer.Instance.AccountManager.DeleteAccount(client, accessToken))
                client.TriggerEvent("account-deletion_failure");
        }

        [RemoteEvent("request-discord_verification")]
        public static void OnRequestDiscordVerification(Client client, string accessToken)
        {
            string userID = WebServer.Instance.Sessions.ValidateSession(client, accessToken);
            if (userID != null)
            {
                client.TriggerEvent("send-discord_verify_url",
                    WebServer.Instance.AccountManager.GenerateDiscordVerifyUrl(userID));
            }
        }

        [RemoteEvent("check-creation_data")]
        public static void OnCheckCreationData(Client client, string username, string email)
        {
            client.TriggerEvent("creation_data-check_result",
                WebServer.Instance.AccountManager.CheckCreationData(username, email));
        }

        [RemoteEvent("create-account")]
        public static void OnCreateAccount(Client client, string username, string email, string password)
        {
            client.TriggerEvent("account-creation_result",
                WebServer.Instance.AccountManager.CreateAccount(username, email, password));
        }
    }
}