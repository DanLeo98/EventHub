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
        int ID { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        #endregion


        User()
        {
            
        }

        User(string name, string email, string password)
        {

        }

        #region FUNCTIONS
        bool CheckCredentials()
        {
            return true;
        }
        #endregion


    }
}
