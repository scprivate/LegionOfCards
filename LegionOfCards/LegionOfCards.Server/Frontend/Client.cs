using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data.Net;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using Logger = LegionOfCards.Utils.Logger;

namespace LegionOfCards.Server.Frontend
{
    public class Client : WebSocketBehavior
    {
        public void TriggerEvent(string name, params object[] args)
        {
            Send(JsonConvert.SerializeObject(new Packet(name, args)));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            try
            {
                Packet packet = JsonConvert.DeserializeObject<Packet>(message);
                WebServer.Instance.Events.Handle(this, packet);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while parsing incoming packet! ('" + message + "')", ex);
            }
        }

        protected override void OnOpen()
        {
            WebServer.Instance.InvokeConnect(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WebServer.Instance.InvokeDisconnect(this, e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            WebServer.Instance.InvokeError(this, e);
        }
    }
}
