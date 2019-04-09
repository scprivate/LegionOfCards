using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Utils;

namespace LegionOfCards.Data.Models
{
    public class User
    {
        public string ID { get; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsMod { get; set; }

        internal string HashedPassword { get; }

        internal User(string name, string email, string password)
        {
            ID = Database.GenerateKeyAndCheck(Database.UserTable, "UserID", () => Guid.NewGuid().ToString());
            Name = name;
            Email = email;
            HashedPassword = Cryptor.MD5(password);
        }

        internal User(Dictionary<string, object> data)
        {
            ID = (string) data["UserID"];
            Name = (string) data["Name"];
            Email = (string) data["Email"];
            HashedPassword = (string) data["Password"];
            IsMod = (bool) data["IsMod"];
        }
    }
}
