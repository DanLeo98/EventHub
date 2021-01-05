using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
    public enum EventStatus
    {
        open,
        closed,
        full,
        expired,
        cancelled
    }
    public class Event
    {
        int id;
        string name;
        DateTime startDate;
        DateTime endDate;
        string local;
        string description;
        int slots;
        EventStatus status;
        float entryFee;
        int teamMax;

        #region PROPERTIES
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; } // Check for only alpahbet characters
        public DateTime StartDate { get => startDate; set => startDate = value; } // check if date > present
        public DateTime EndDate { get => endDate; set => endDate = value; } // check if endDate > startDate + date > present
        public string Local { get => local; set => local = value; }
        public string Description { get => description; set => description = value; }
        public int Slots { get => slots; set => slots = value; } // only values > 0
        public EventStatus Status { get => status; set => status = value; }
        public float EntryFee{ get => entryFee; set => entryFee = value; }
        public int TeamMax { get => teamMax; set => teamMax = value; } // only values > 0
        #endregion

        /* Data Structures */
        Dictionary<Team, DateTime> teams;
        List<Prize> prizeChart;

        public bool AddTeam(Team teamToAdd)
        {
            // check for space and if new team is null   +   lidar com slots
            return false;
        }

        public Event()
        {
            teams = new Dictionary<Team, DateTime>();
            prizeChart = new List<Prize>();
        }
    }
}
