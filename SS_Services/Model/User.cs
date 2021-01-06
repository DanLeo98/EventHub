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
        public Account account;

        #region PROPERTIES
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
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

        public bool ValidateObject()
        {
            if (1 == 1) return true;
            //return false;
        }
        #endregion


    }
}
