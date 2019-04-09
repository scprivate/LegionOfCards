using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionOfCards.Data.Models
{
    public class User
    {
        public string ID { get; }

        public string Name { get; }

        public string Email { get; }

        internal User(string name, string email)
        {
            ID = Database.GenerateKeyAndCheck(Database.UserTable, "UserID", () => Guid.NewGuid().ToString());
            Name = name;
            Email = email;
        }

        internal User(Dictionary<string, object> data)
        {
            ID = (string) data["UserID"];
            Name = (string) data["Name"];
            Email = (string) data["Email"];
        }
    }
}
