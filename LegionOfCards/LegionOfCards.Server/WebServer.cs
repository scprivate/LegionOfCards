using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data;
using LegionOfCards.Data.Controllers;
using LegionOfCards.Data.Models;
using LegionOfCards.Data.Net;
using LegionOfCards.Discord;
using LegionOfCards.Server.Account;
using LegionOfCards.Server.Commands;
using LegionOfCards.Server.Events;
using LegionOfCards.Server.Frontend;
using LegionOfCards.Utils;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using EventHandler = LegionOfCards.Server.Events.EventHandler;
using Logger = LegionOfCards.Utils.Logger;

namespace LegionOfCards.Server
{
    public class WebServer
    {
        public static WebServer Instance { get; internal set; }

        public WebSocketServer Socket { get; }

        public EventHandler Events { get; }

        public CommandHandler Commands { get; }

        public SessionManager Sessions { get; }

        public AccountManager AccountManager { get; }
        
        public event Action<Client> Connect;
        public event Action<Client, CloseEventArgs> Disconnect;
        public event Action<Client, ErrorEventArgs> Error;

        public WebServer()
        {
            string sessionSecret;
            if (JsonStorage.Exists("session_secret"))
            {
                Dictionary<string, object> data = JsonStorage.Load<Dictionary<string, object>>("session_secret");
                if (DateTime.FromBinary((long) data["exp"]) < DateTime.UtcNow)
                {
                    sessionSecret = GenerateNewSecret();
                }
                else
                {
                    sessionSecret = (string) data["val"];
                }
            }
            else
            {
                sessionSecret = GenerateNewSecret();
            }

            AccountManager = new AccountManager();
            Sessions = new SessionManager(sessionSecret);
            Socket = new WebSocketServer(25319);
            Events = new EventHandler();
            Events.Add<WebServer>();
            Events.Add<SessionManager>();
            Events.Add<AccountManager>();
            Commands = new CommandHandler();
            //Socket.AddWebSocketService<Client>("/locgcapi");
            DiscordVerification.VerificationSuccess += OnDiscordVerified;
            DiscordVerification.Start();
        }

        private string GenerateNewSecret()
        {
            string secret = Cryptor.GenerateSecret();
            JsonStorage.Save("session_secret", new Dictionary<string, object>
            {
                { "exp", DateTime.UtcNow.AddDays(1).ToBinary() },
                { "val", secret }
            });
            return secret;
        }

        private void OnDiscordVerified(string userID)
        {
            User user = UserController.GetUser(userID);
            BotInterface.GiveDuellist(user.DiscordID).GetAwaiter().GetResult();
        }

        public void BroadcastEvent(string name, params object[] args)
        {
            Socket.WebSocketServices.Broadcast(JsonConvert.SerializeObject(new Packet(name, args)));
        }

        public void Start()
        {
            try
            {
                Logger.Info("Starting server...");
                Socket.Start();
                Logger.Success("Server started on ANY (localhost / " + (NetUtils.GetExternalIp() ?? "external") + ")!");
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while starting the server!", ex);
            }
        }

        public void Stop()
        {
            try
            {
                Logger.Warn("Stopping server...");
                Socket.Stop();
                Logger.Success("Server stopped!");
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while stopping the server!", ex);
            }
        }

        internal void InvokeConnect(Client client)
        {
            Connect?.Invoke(client);
        }

        internal void InvokeDisconnect(Client client, CloseEventArgs args)
        {
            Disconnect?.Invoke(client, args);
        }

        internal void InvokeError(Client client, ErrorEventArgs args)
        {
            Error?.Invoke(client, args);
        }

        
    }
}
