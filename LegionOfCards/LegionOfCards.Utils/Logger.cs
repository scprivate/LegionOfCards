using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Utils
{
    public class Logger
    {
        public static readonly List<string> Cache = new List<string>();

        private static void Print(ConsoleColor prefixColor, string prefix, string message)
        {
            ConsoleColor backup = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{DateTime.Now:T}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");
            Console.ForegroundColor = prefixColor;
            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = backup;
            Cache.Add($"[{DateTime.Now:T}] {prefix} | " + message);
        }

        public static void Info(string message)
        {
            Print(ConsoleColor.Blue, "INFO", message);
        }

        public static void Warn(string message)
        {
            Print(ConsoleColor.Yellow, "WARN", message);
        }

        public static void Error(string message, Exception exception)
        {
            Print(ConsoleColor.Red, "ERROR", message + " " + exception);
        }

        public static void Fatal(string message)
        {
            Print(ConsoleColor.DarkRed, "FATAL", message);
        }

        public static void Success(string message)
        {
            Print(ConsoleColor.Green, "SUCCESS", message);
        }
    }
}
