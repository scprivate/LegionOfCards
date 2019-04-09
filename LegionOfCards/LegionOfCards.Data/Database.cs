using System;
using System.Collections.Generic;
using System.Linq;
using LegionOfCards.Utils;
using MySql.Data.MySqlClient;

namespace LegionOfCards.Data
{
    public class Database
    {
        private static readonly string ConnectionString = "SERVER=legionofsensei.de;UID=mrpneu;PASSWORD=b3vz30A;DATABASE=mrpneu;";

        public static string UserTable => "loc__users";
        public static string BanTable => "loc__banns";

        public static void Init()
        {
            CreateTable(UserTable, "UserID TEXT, Name TEXT, Password TEXT, Email TEXT, IsMod TINYINT(1)");
            CreateTable(BanTable, "BanID TEXT, UserID TEXT, ModID TEXT, Expires TEXT, Reason TEXT");
        }

        private static void CreateTable(string name, string rows)
        {
            Setter($"CREATE TABLE IF NOT EXISTS {name} ({rows})");
        }

        public static string FromDateTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime ToDateTime(string text)
        {
            return DateTime.Parse(text);
        }

        public static bool Sneaky { get; set; } = false;

        public static bool CustomSetter(string connectionString, string query, params Tuple<string, object>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("An Error occurred while performing setter mysql command ('" + query + "')! ", e);
                return false;
            }
        }

        public static void Setter(string query, params Tuple<string, object>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("An Error occurred while performing setter mysql command ('" + query + "')!", e);
            }
        }

        public static Dictionary<string, object> GetterOne(string query, params Tuple<string, object>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Enumerable.Range(0, reader.FieldCount)
                                    .ToDictionary(reader.GetName, reader.GetValue);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                    Logger.Error("An Error occurred while performing getter one mysql command!", e);
                return null;
            }
        }

        public static Dictionary<string, object> CustomGetterOne(string connectionString, string query, params Tuple<string, object>[] args)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Enumerable.Range(0, reader.FieldCount)
                                    .ToDictionary(reader.GetName, reader.GetValue);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error("An Error occurred while performing getter one mysql command!", e);
                return null;
            }
        }

        public static bool Exists(string query, params Tuple<string, object>[] args)
        {
            try
            {
                List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.Read();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                    Logger.Error("An Error occurred while performing exists mysql command!", e);
                return false;
            }
        }

        public static List<Dictionary<string, object>> Getter(string query, params Tuple<string, object>[] args)
        {
            try
            {
                List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = BuildCommand(connection, query, args))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               results.Add(Enumerable.Range(0, reader.FieldCount)
                                   .ToDictionary(reader.GetName, reader.GetValue));
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                    Logger.Error("An Error occurred while performing getter mysql command!", e);
                return null;
            }
        }

        public static string GenerateKeyAndCheck(string tableToCheck, string rowToCheck, Func<string> generator)
        {
            string key;
            while (Exists($"SELECT * FROM {tableToCheck} WHERE {rowToCheck} = @val",
                new Tuple<string, object>("@val", key = generator.Invoke()))) ;
            return key;
        }

        private static MySqlCommand BuildCommand(MySqlConnection connection, string text, params Tuple<string, object>[] args)
        {
            MySqlCommand command = new MySqlCommand(text, connection);
            command.Prepare();
            foreach (Tuple<string, object> arg in args)
            {
                command.Parameters.AddWithValue(arg.Item1, arg.Item2);
            }

            return command;
        }
    }
}
