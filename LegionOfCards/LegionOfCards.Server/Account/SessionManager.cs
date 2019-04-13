using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data;
using LegionOfCards.Data.Controllers;
using LegionOfCards.Server.Events;
using LegionOfCards.Server.Frontend;
using LegionOfCards.Utils;

namespace LegionOfCards.Server.Account
{
    public class SessionManager
    {
        public List<Session> Sessions { get; }

        private readonly string _secret;

        public SessionManager(string secret)
        {
            _secret = secret;
            Sessions = new List<Session>();
        }

        public Session CreateSession(string userID)
        {
            Session session = new Session(userID, Cryptor.GenerateSessionToken(_secret, userID));
            Database.Setter($"INSERT INTO {Database.SessionTable} (Token, UserID) VALUES (@token, @user)",
                new Tuple<string, object>("@token", session.Token), new Tuple<string, object>("@user", userID));
            lock (Sessions)
            {
                Sessions.Add(session);
            }
            return session;
        }
        
        /// <returns>Null if the validation was a failure</returns>
        public Session ValidateSession(Client client, string accessToken, bool requestLogin = true)
        {
            Session session = GetSession(accessToken);
            if (session != null)
            {
                if (Cryptor.ValidateSessionToken(GodotServer.Instance.Sessions._secret, accessToken, session.User.ID) ==
                    Cryptor.TokenResult.TokeFine)
                {
                    return session;
                }
            }

            if (requestLogin)
            {
                client.TriggerEvent("session-destroyed");
            }
            return null;
        }

        public void DestroySession(Client client, string token, bool send = true)
        {
            Session session = GetSession(token);
            if (session != null)
            {
                Sessions.Remove(session);
            }

            Database.Setter($"DELETE FROM {Database.SessionTable} WHERE Token = @token",
                new Tuple<string, object>("@token", token));
            if(send)
                client.TriggerEvent("session-destroyed");
        }

        public Session GetSession(string token)
        {
            foreach (Session session in Sessions)
            {
                if (session.Token == token)
                {
                    return session;
                }
            }

            return null;
        }

        [RemoteEvent("logout-session")]
        public static void OnLogoutSession(Client client, string accessToken)
        {
            GodotServer.Instance.Sessions.DestroySession(client, accessToken);
        }

        [RemoteEvent("ping_session")]
        public static void OnPingSession(Client client, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Dictionary<string, object> possibleSession = Database.GetterOne(
                    $"SELECT * FROM {Database.SessionTable} WHERE Token = @token",
                    new Tuple<string, object>("@token", token));
                if (possibleSession != null)
                {
                    Database.Setter($"DELETE FROM {Database.SessionTable} WHERE Token = @token", new Tuple<string, object>("@token", token));
                    if (Cryptor.ValidateSessionToken(GodotServer.Instance.Sessions._secret, token,
                            (string)possibleSession["UserID"]) == Cryptor.TokenResult.TokeFine)
                    {
                        Session session = GodotServer.Instance.Sessions.CreateSession((string)possibleSession["UserID"]);
                        client.TriggerEvent("ping_result", true, session.User.ID, session.Token);
                        return;
                    }
                }
            }

            client.TriggerEvent("ping_result", false, "", "");
        }

        [RemoteEvent("check-password")]
        public static void OnCheckPassword(Client client, string identifier, string password)
        {
            string hashedPassword = Cryptor.MD5(password);
            string userID = UserController.CheckPassword(identifier, hashedPassword);
            if (string.IsNullOrEmpty(userID))
            {
                client.TriggerEvent("session-result", false, "", "");
            }
            else
            {
                Session session = GodotServer.Instance.Sessions.CreateSession(userID);
                client.TriggerEvent("session-result", true, session.User.ID, session.Token);
            }
        }
    }
}
