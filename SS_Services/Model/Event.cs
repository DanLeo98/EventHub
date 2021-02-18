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

using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;


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

    /// <summary>
    /// Verifies if event is valid
    /// </summary>
    /// <returns></returns>
    public bool ValidateEvent()
    {
        if (initialDate > DateTime.Today && endDate > initialDate &&
            teamMax > 0 && teamMax < slots) return true;

        return false;
    }

    public static List<Event> GetFriendlies(string connString)
    {
        List<Event> events = new List<Event>();
        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            string query = "SELECT * FROM event WHERE entryFee IS NULL;";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Event friendly = new Event();

                friendly.Id = reader.GetInt32(0);
                friendly.Name = reader.GetString(1);
                friendly.InitialDate = reader.GetDateTime(2);
                friendly.EndDate = reader.GetDateTime(3);
                friendly.Description = reader.GetString(4);
                friendly.Slots = reader.GetInt32(5);
                friendly.Local = reader.GetString(6);
                friendly.Status = (EventStatus)reader.GetInt32(7);
                friendly.TeamMax = reader.GetInt32(11);

                events.Add(friendly);
            }
            conn.Close();
        }
        return events;
    }
    public static List<Event> GetComps(string connString)
    {
        List<Event> events = new List<Event>();
        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            string query = "SELECT * FROM event WHERE entryFee IS NOT NULL;";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Event comp = new Event();

                comp.Id = reader.GetInt32(0);
                comp.Name = reader.GetString(1);
                comp.InitialDate = reader.GetDateTime(2);
                comp.EndDate = reader.GetDateTime(3);
                comp.Description = reader.GetString(4);
                comp.Slots = reader.GetInt32(5);
                comp.Local = reader.GetString(6);
                comp.Status = (EventStatus)reader.GetInt32(7);
                comp.EntryFee = reader.GetFloat(8);
                comp.TeamMax = reader.GetInt32(11);

                events.Add(comp);
            }
            conn.Close();
        }
        return events;
    }
    public int CreateEvent(string connString)
    {
        try
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert new event
                // Parameterized
                string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,entryfee,sportid,userid,team_max)" +
                    "VALUES(@name,@ini_date,@end_date,@desc,@slots,@local,@status,@fee,@sport,@user,@max);";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                NpgsqlParameter p_name = new NpgsqlParameter("@name", this.Name);
                cmd.Parameters.Add(p_name);

                NpgsqlParameter p_initial_date = new NpgsqlParameter("@ini_date", this.InitialDate);
                cmd.Parameters.Add(p_initial_date);

                NpgsqlParameter p_end_date = new NpgsqlParameter("@end_date", this.EndDate);
                cmd.Parameters.Add(p_end_date);

                NpgsqlParameter p_desc = new NpgsqlParameter("@desc", this.Description);
                cmd.Parameters.Add(p_desc);

                NpgsqlParameter p_slots = new NpgsqlParameter("@slots", this.Slots);
                cmd.Parameters.Add(p_slots);

                NpgsqlParameter p_local = new NpgsqlParameter("@local", this.Local);
                cmd.Parameters.Add(p_local);

                NpgsqlParameter p_status = new NpgsqlParameter("@status", (int)this.Status);
                cmd.Parameters.Add(p_status);

                NpgsqlParameter p_fee;
                if (this.EntryFee == null) p_fee = new NpgsqlParameter("@fee", DBNull.Value);
                else p_fee = new NpgsqlParameter("@fee", this.EntryFee);
                cmd.Parameters.Add(p_fee);

                NpgsqlParameter p_sport = new NpgsqlParameter("@sport", this.SportId);
                cmd.Parameters.Add(p_sport);

                NpgsqlParameter p_user = new NpgsqlParameter("@user", this.UserId);
                cmd.Parameters.Add(p_user);

                NpgsqlParameter p_max = new NpgsqlParameter("@max", this.TeamMax);
                cmd.Parameters.Add(p_max);

                int rowsAff = cmd.ExecuteNonQuery();
                conn.Close();

                if (rowsAff == 1) return 0;
                return 1;
            }
        }
        catch (Exception e)
        {
            return 1;
        }
    }

}


