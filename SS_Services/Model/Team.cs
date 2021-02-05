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

