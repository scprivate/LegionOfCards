using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LegionOfCards.Data;
using LegionOfCards.Data.Controllers;
using LegionOfCards.Data.Models;
using LegionOfCards.Discord;
using LegionOfCards.Server.Account;
using LegionOfCards.Server.Commands;
using LegionOfCards.Server.Frontend;
using LegionOfCards.Utils;
using EventHandler = LegionOfCards.Server.Events.EventHandler;

namespace LegionOfCards.Server
{
    public class GodotServer
    {
        public static GodotServer Instance { get; internal set; }

        public bool Running { get; private set; }

        public EventHandler Events { get; }

        public CommandHandler Commands { get; }

        public SessionManager Sessions { get; }

        public AccountManager AccountManager { get; }

        private readonly TcpListener _listener;
        private readonly Thread _acceptThread;
        private readonly List<Client> _clients;
        private int _id;

        public GodotServer()
        {
            string sessionSecret;
            if (JsonStorage.Exists("session_secret"))
            {
                Dictionary<string, object> data = JsonStorage.Load<Dictionary<string, object>>("session_secret");
                if (DateTime.FromBinary((long)data["exp"]) < DateTime.UtcNow)
                {
                    sessionSecret = GenerateNewSecret();
                }
                else
                {
                    sessionSecret = (string)data["val"];
                }
            }
            else
            {
                sessionSecret = GenerateNewSecret();
            }

            AccountManager = new AccountManager();
            Sessions = new SessionManager(sessionSecret);
            Events = new EventHandler();
            Events.Add<WebServer>();
            Events.Add<SessionManager>();
            Events.Add<AccountManager>();
            Commands = new CommandHandler();
            Running = true;
            _id = 0;
            _clients = new List<Client>();
            _listener = new TcpListener(IPAddress.Any, 25352);
            _acceptThread = new Thread(HandleAccepting) {IsBackground = true};
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
            foreach (Client client in _clients)
            {
                client.TriggerEvent(name, args);
            }
        }

        internal void OnDisconnect(Client client)
        {
            _clients.Remove(client);
        }

        public void Start()
        {
            Logger.Info("Starting tcp server...");
            _listener.Start();
            _acceptThread.Start();
            Logger.Success("Server started on ANY (localhost / " + (NetUtils.GetExternalIp() ?? "external") + ")!"); ;
        }

        public void Stop()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        private async Task StopAsync()
        {
            Running = false;
            Logger.Info("Forcing clients disconnect!");
            BroadcastEvent("force-disconnect");
            Logger.Warn("Waiting for clients to disconnect...");
            await WaitForClear();
            Logger.Success("All clients disconnected! Shutting down...");
            _listener.Stop();
        }

        private async Task WaitForClear()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_clients.Count <= 0)
                    {
                        break;
                    }

                    Task.Delay(1000);
                }
            });
        }

        private void HandleAccepting()
        {
            while (Running)
            {
                try
                {
                    TcpClient socket = _listener.AcceptTcpClient();
                    Logger.Info("Client accepted!");
                    Client client = new Client(_id, socket, this);
                    _clients.Add(client);
                    client.TriggerEvent("connection-established");
                    _id++;
                }
                catch(Exception ex)
                {
                    if(Running)
                        Logger.Error("An error occurred while waiting for accept requests!", ex);
                }
            }
        }
    }
}
