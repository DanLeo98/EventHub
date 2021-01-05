using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
    public class User
    {
        int id;
        string name;
        string email;
        string password;
        Account account;

        #region PROPERTIES
        int Id { get => id; set => id = value; }
        string Name { get => name; set => name = value; }
        string Email { get => email; set => email = value; }
        string Password { get => password; set => password = value; }
        #endregion


        public User()
        {
            account = new Account();
        }

        public User(string name, string email, string password)
        {
            account = new Account();
        }

        #region FUNCTIONS
        public bool CheckCredentials()
        {
            return true;
        }
        #endregion


    }
}
