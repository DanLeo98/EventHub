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


public enum Place
{
    first = 1,
    second = 2,
    third = 3,
    fourth = 4,
    fifth = 5,
    sixth = 6,
    seventh = 7,
    eighth = 8,
    na = 0
}

public class Team
{
    int id;
    //List<User> members;
    Place position;

    #region PROPERTIES
    public int Id { get => id; set => id = value; }
    //List<User> Members { get => members; set => members = value; }
    public Place Position { get => position; set => position = value; }
    #endregion

    public Team()
    {

    }
}

