﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using EventHub.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace EventHub.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";
        
        [HttpGet("getFriendlyEvents")]
        public ActionResult GetFriendlyEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
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
                        friendly.StartDate = reader.GetDateTime(2);
                        friendly.EndDate = reader.GetDateTime(3);
                        friendly.Description = reader.GetString(4);
                        friendly.Slots = reader.GetInt32(5);
                        friendly.Local = reader.GetString(6);
                        friendly.Status = (EventStatus)reader.GetInt32(7);

                        events.Add(friendly);
                    }
                    conn.Close();
                }
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);
        }

        [HttpPost("createFriendlyEvent")]
        public ActionResult CreateFriendlyEvent(string ev, int userId)
        {
            Event eve = (Event)JsonConvert.DeserializeObject(ev,typeof(Event));
            if (eve.ValidateObject())
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,sportid,userid,team_max)" +
                            "VALUES('" + eve.Name + "','" + eve.StartDate.ToString("yyyy’-‘MM’-‘dd") + "','" + eve.EndDate.ToString("yyyy’-‘MM’-‘dd")
                            + eve.Description + "'," + eve.Slots + ",'" + eve.Local + "'," + (int)eve.Status + "," + eve.SportId + "," + userId + "," + eve.TeamMax + ");";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        int rowsAff = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAff == 1) return Ok();
                    }
                } catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }            
            }
            return Unauthorized();
        }


    }
}
