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

        }

        public static User CreateUser(string name, string email, string password)
        {
            User user = new User(name, email);
            Database.Setter($"INSERT INTO {Database.UserTable} (UserID, Name, Password, Email, IsMod) VALUES (@id, @name, @pw, @mail, @mod)",
                new Tuple<string, object>("@id", user.ID), new Tuple<string, object>("@name", name), new Tuple<string, object>("@pw", Cryptor.MD5(password)), 
                new Tuple<string, object>("@mail", email), new Tuple<string, object>("@mod", false));
            return user;
        }
    }
}