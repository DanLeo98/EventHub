using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class User
{
    int id, accountId;
    string name;
    string email;
    string password;

    #region PROPERTIES

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Email { get => email; set => email = value; }
    public string Password { get => password; set => password = value; }
    public int AccountId { get => accountId; set => accountId = value; }
    #endregion


    public User()
    {

    }

    public User(string name, string email, string password)
    {
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

