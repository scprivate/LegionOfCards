using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Data.Models;
using LegionOfCards.Utils;

namespace LegionOfCards.Data.Controllers
{
    public class UserController
    {
        public static string CheckPassword(string identifier, string password)
        {
            Dictionary<string, object> data = Database.GetterOne(
                $"SELECT * FROM {Database.UserTable} WHERE Email = @ident OR Name = @ident",
                new Tuple<string, object>("@ident", identifier));
            if (data != null && (string) data["Password"] == password)
            {
                return (string) data["UserID"];
            }

            return null;
        }

        public static User GetUser(string id)
        {
            Dictionary<string, object> data = Database.GetterOne(
                $"SELECT * FROM {Database.UserTable} WHERE UserID = @id", new Tuple<string, object>("@id", id));
            if (data != null)
            {
                return new User(data);
            }

            return null;
        }

        public static void SaveUser(User user)
        {
            if (Database.Exists($"SELECT * FROM {Database.UserTable} WHERE UserID = @id",
                new Tuple<string, object>("@id", user.ID)))
            {
                Database.Setter(
                    $"UPDATE {Database.UserTable} SET Name = @name, Email = @email, IsMod = @mod WHERE UserID = @id",
                    new Tuple<string, object>("@id", user.ID), new Tuple<string, object>("@name", user.Name),
                    new Tuple<string, object>("@email", user.Email), new Tuple<string, object>("@mod", user.IsMod));
            }
        }

        public static void DeleteUser(string userID)
        {
            if (Database.Exists($"SELECT * FROM {Database.UserTable} WHERE UserID = @id",
                new Tuple<string, object>("@id", userID)))
            {
                Database.Setter($"DELETE FROM {Database.UserTable} WHERE UserID = @id", new Tuple<string, object>("@id", userID));
            }
        }

        public static User CreateUser(string name, string email, string password)
        {
            User user = new User(name, email, password);
            Database.Setter($"INSERT INTO {Database.UserTable} (UserID, Name, Password, Email, IsMod, DiscordID) VALUES (@id, @name, @pw, @mail, @mod, '')",
                new Tuple<string, object>("@id", user.ID), new Tuple<string, object>("@name", name), new Tuple<string, object>("@pw", user.HashedPassword), 
                new Tuple<string, object>("@mail", email), new Tuple<string, object>("@mod", false));
            return user;
        }
    }
}