using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data.Net;
using LegionOfCards.Utils;

namespace LegionOfCards.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 25352);
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Verbindung eingegangen: " + client.Client.AddressFamily);
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(new Packet("greetings", new object[] { "Hallo Welt!" }).ToString());
                writer.Flush();
                writer.Close();
            }
        }
    }
}
