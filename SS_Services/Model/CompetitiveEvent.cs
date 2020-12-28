using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
    public class CompetitiveEvent: Event
    {
        float entryFee;

        /* Data Structures */
        Dictionary<Team, DateTime> teams;
        List<Prize> prizeChart;

        #region PROPERTIES
        public float EntryFee { get; set; }

        #endregion

        public bool AddTeam(Team teamToAdd)
        {
            // check for space and if new team is null   +   lidar com slots
            return false;
        }

        public CompetitiveEvent()
        {
            teams = new Dictionary<Team, DateTime>(); 
            prizeChart = new List<Prize>();
        }
    }
}
