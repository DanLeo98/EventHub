/*
 * Authors: João Rodrigues and Daniel Leonard
 * Project: Practical Work, implementing services
 * Current Solution: Client of services for sport events
 * 
 * 
 * Subject: Integration of Informatic Systems
 * Degree: Graduation on Engeneer of Informatic Systems
 * Lective Year: 2020/21
 */

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
