using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class RootObject
{
    [JsonProperty("event")]
    public Event Event { get; set; }
    [JsonProperty("user")]
    public User User { get; set; }
    [JsonProperty("sport")]
    public Sport Sport { get; set; }
    [JsonProperty("team")]
    public Team Team { get; set; }
    [JsonProperty("account")]
    public Account Account { get; set; }
    [JsonProperty("prize")]
    public Prize Prize { get; set; }
}

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
    DateTime initialDate;
    DateTime endDate;
    string local;
    string description;
    int slots;
    EventStatus status;
    float? entryFee;
    int sportId;
    int teamMax;
    int userId;
    

    /*
    Data Structures 
    Dictionary<Team, DateTime> teams;
    List<Prize> prizeChart;
    */

    #region PROPERTIES
    [JsonProperty("id")]
    public int Id { get => id; set => id = value; }
    
    [JsonProperty("name")]
    public string Name { get => name; set => name = value; } // Check for only alpahbet characters
    
    [JsonProperty("initialdate")]
    public DateTime InitialDate { get => initialDate; set => initialDate = value; } // check if date > present
    
    [JsonProperty("enddate")]
    public DateTime EndDate { get => endDate; set => endDate = value; } // check if endDate > startDate + date > present
    
    [JsonProperty("description")]
    public string Description { get => description; set => description = value; }
    
    [JsonProperty("slots")]
    public int Slots { get => slots; set => slots = value; } // only values > 0
    
    [JsonProperty("local")]
    public string Local { get => local; set => local = value; }
    [JsonProperty("entryFee")]
    public float? EntryFee { get => entryFee; set => entryFee = value; }

    [JsonProperty("status")]
    public EventStatus Status { get => status; set => status = value; }
    
    [JsonProperty("sportId")]
    public int SportId { get => sportId; set => sportId = value; }
    
    [JsonProperty("teamMax")]
    public int TeamMax { get => teamMax; set => teamMax = value; }
    
    public int UserId { get => userId; set => userId = value; }
    /*
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
