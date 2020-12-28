using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
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
        List<User> members;
        Place position;

        #region PROPERTIES
        int Id { get; set; }
        List<User> Members { get; set; }
        Place Place { get; set; }
        #endregion

        public Team()
        {
            
        }
    }
}
