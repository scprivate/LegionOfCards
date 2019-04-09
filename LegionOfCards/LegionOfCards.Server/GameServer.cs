using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data.Net;
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
    public class GameServer
    {
        public static GameServer Instance { get; internal set; }

        public WebSocketServer Socket { get; }

        public EventHandler Events { get; }

        public CommandHandler Commands { get; }
        
        public event Action<Client> Connect;
        public event Action<Client, CloseEventArgs> Disconnect;
        public event Action<Client, ErrorEventArgs> Error; 

        public GameServer()
        {
            Socket = new WebSocketServer(25319);
            Events = new EventHandler();
            Events.Add<GameServer>();
            Commands = new CommandHandler();
            Socket.AddWebSocketService<Client>("/locgcapi");
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

        [RemoteEvent("test-connection")]
        public static void TestConnectionEvent(Client client, string msg)
        {
            client.TriggerEvent("connection-result", msg);
        }
    }
}
