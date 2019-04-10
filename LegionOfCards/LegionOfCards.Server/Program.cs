using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data;
using LegionOfCards.Data.Controllers;
using LegionOfCards.Data.Models;
using LegionOfCards.Discord;
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
            Logger.Info("Init database...");
            Database.Init();
            Logger.Success("Database init successful!");
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

        [Command("adduser")]
        public static void OnAddUserCommand(string name, string email, string pw)
        {
            User user = UserController.CreateUser(name, email, pw);
            Logger.Success("User created: " + user.ID);
        }

        [Command("help")]
        public static void OnHelpCommand()
        {
            Logger.Info("All commands:");
            Console.WriteLine("help - Shows all commands");
            Console.WriteLine("exit - Stops and exits the server");
            Console.WriteLine("giveduellist <id> - Gives the duellist rank to the given <id> on discord");
        }

        [Command("giveduellist")]
        public static void OnGiveDuellistCommand(string id)
        {
            if (BotInterface.GiveDuellist(id).GetAwaiter().GetResult())
            {
                Logger.Success("'" + id + "' should got its duellist rank!");
            }
            else
            {
                Logger.Fatal("An error occurred while performing this command, because a failure was returned from the bot!");
            }
        }

        [Command("exit")]
        public static void OnExitCommand()
        {
            _running = false;
        }
    }
}
