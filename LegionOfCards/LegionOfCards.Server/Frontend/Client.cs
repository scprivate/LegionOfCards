using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LegionOfCards.Data.Net;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
using Logger = LegionOfCards.Utils.Logger;

namespace LegionOfCards.Server.Frontend
{
    public class Client
    {
        public int ID { get; }

        private readonly TcpClient _socket;
        private readonly StreamWriter _writer;
        private readonly GodotServer _server;
        private bool _disable;

        internal Client(int id, TcpClient socket, GodotServer server)
        {
            ID = id;
            _server = server;
            _disable = false;
            _socket = socket;
            _writer = new StreamWriter(socket.GetStream());
            var thread = new Thread(() =>
            {
                try
                {
                    while (!_disable)
                    {
                        string data = "";
                        do
                        {
                            byte[] buffer = new byte[_socket.Available];
                            int length = _socket.GetStream().Read(buffer, 0, _socket.Available);
                            data += Encoding.ASCII.GetString(buffer, 0, length);

                        } while (_socket.GetStream().DataAvailable);

                        if (!data.StartsWith("{"))
                            data = data.Substring(data.IndexOf("{", StringComparison.Ordinal));
                        OnMessage(data);
                    }
                }
                catch
                {
                    Logger.Warn("Client connection lost because of an error but it will be ignored because it should be wanted!");
                    CleanUp();
                }
            }){IsBackground = true};
            thread.Start();
        }

        public void TriggerEvent(string name, params object[] args)
        {
            _writer.Write(new Packet(name, args).ToString());
            _writer.Flush();
        }

        private void OnMessage(string msg)
        {
            try
            {
                Packet packet = JsonConvert.DeserializeObject<Packet>(msg);
                GodotServer.Instance.Events.Handle(this, packet);
            }
            catch (Exception ex)
            {
                if(GodotServer.Instance.Running)
                    Logger.Error("An error occurred while parsing incoming packet! ('" + msg + "')", ex);
                CleanUp();
            }
        }

        private void CleanUp()
        {
            try
            {
                _disable = true;
                _server.OnDisconnect(this);
                _writer.Close();
            }
            catch (Exception e)
            {
                Logger.Warn("An error occurred while cleaning up client! " + e);
            }
        }
    }
}
