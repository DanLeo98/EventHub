using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        int sportId;
        int teamMax;
        /*
        float? entryFee;
        
        
        Data Structures 
        Dictionary<Team, DateTime> teams;
        List<Prize> prizeChart;
        */

        #region PROPERTIES
        [JsonProperty("id")]
        public int Id { get => id; set => id = value; }
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; } // Check for only alpahbet characters
        [JsonProperty("initial_Date")]
        public DateTime StartDate { get => startDate; set => startDate = value; } // check if date > present
        [JsonProperty("end_Date")]
        public string Local { get => local; set => local = value; }
        [JsonProperty("description")]
        public string Description { get => description; set => description = value; }
        [JsonProperty("slots")]
        public DateTime EndDate { get => endDate; set => endDate = value; } // check if endDate > startDate + date > present
        [JsonProperty("local")]
        public int Slots { get => slots; set => slots = value; } // only values > 0
        [JsonProperty("status")]
        public EventStatus Status { get => status; set => status = value; }
        [JsonProperty("sportId")]
        public int SportId { get => sportId; set => sportId = value; }
        [JsonProperty("team_Max")]
        public int TeamMax { get => teamMax; set => teamMax = value; }
        /*
        public float? EntrFee { get => entryFee; set => entryFee = value; }
        
        public Dictionary<Team, DateTime> Teams { get => teams; set => teams = value; }
        public List<Prize> PrizeChart { get => prizeChart; set => prizeChart = value; }
        */
        #endregion
        public Event()
        {

        }

        public bool ValidateObject()
        {
            if (1 == 1) return true;
            /*
             * return false;
             * teams = new Dictionary<Team, DateTime>(); 
            prizeChart = new List<Prize>();
            */
        }


    }
}
