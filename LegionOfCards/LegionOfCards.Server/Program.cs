using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Server.Commands;
using LegionOfCards.Utils;

namespace LegionOfCards.Server
{
    public class Program
    {
        private static bool _running;

        static void Main(string[] args)
        {
            Console.Title = "Legion of Cards - Dedicated Server - (c) Legion of Sensei";
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("This service was made by an voluntary group of Legion of Sensei (www.legionfosensei.de) and is associated to it. The head of the project is DasDarki. All rights belongs to the head and Legion of Sensei. The rights will be split into 60/40 for head/Legion of sensei. All rights reserved!");
            Console.ForegroundColor = ConsoleColor.Gray;
            GameServer.Instance = new GameServer();
            _running = true;
            GameServer.Instance.Commands.Add<Program>();
            GameServer.Instance.Start();
            Logger.Info("Use 'help' for more information! Use 'exit' to stop and exit the server!");
            while (_running)
            {
                if (!GameServer.Instance.Commands.Handle(Console.ReadLine()))
                {
                    Logger.Warn("Command not found! Use 'help' for more information!");
                }
            }
            GameServer.Instance.Stop();
            Console.WriteLine("Press any key to exit program...");
            Console.ReadKey();
        }

        [Command("help")]
        public static void OnHelpCommand()
        {
            Logger.Info("All commands:");
            Console.WriteLine("help - Shows all commands");
            Console.WriteLine("exit - Stops and exits the server");
        }

        [Command("exit")]
        public static void OnExitCommand()
        {
            _running = false;
        }
    }
}
