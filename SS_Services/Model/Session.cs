using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Session
{
    User currentUser; // identifies who's the user
    string token; // Authorization's Token to use service's methods 

    public Session(string token, User user)
    {
        this.token = token;
        currentUser = user;
    }

    public User CurrentUser
    {
        get => currentUser; set => currentUser = value;
    }
    public string Token { get => token; set => token = value; }
}
