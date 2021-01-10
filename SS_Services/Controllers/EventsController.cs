using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Entities.EntitiesService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : Controller
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
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);

        }

        [HttpGet("getCompEvents")]
        public ActionResult GetCompEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);

        }

        [HttpPut("editEvent")]
        public ActionResult EditEvent([FromBody] Event ev)
        {
            if (ValidateObject(ev))
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        string query = "update event set name = '"+ev.Name+"', initial_date = '"+ev.InitialDate.ToString("yyyy-MM-dd") +"',end_Date = '"+ev.EndDate.ToString("yyyy-MM-dd" )+ "', description = '"+ev.Description+"', " +
                            "slots = "+ev.Slots+", local = '"+ev.Local+"', status = "+(int)ev.Status+ ", entryFee = "+ev.EntryFee+", sportid = " +ev.SportId+", userid = "+ev.UserId+",  team_max = "+ev.TeamMax+" where id = "+ev.Id+"; ";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        int rowsAff = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAff == 1) return Ok();
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            }
            return BadRequest();
        }
        

        [Authorize] //IS NOT WORKING
        [HttpPost("createEvent")]
        public ActionResult CreateEvent([FromBody] Event ev)
        {
            try
            {
            //RootObject root = JsonConvert.DeserializeObject<RootObject>(ev);
            // ELIMINATE TRY
                if (ValidateObject(ev))
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                        {
                            conn.Open();

                            string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,entryFee,sportid,userid,team_max)" +
                                "VALUES('" + ev + "','" + ev.InitialDate.ToString("yyyy-MM-dd") + "','" + ev.EndDate.ToString("yyyy-MM-dd") + "','"
                                + ev.Description + "'," + ev.Slots + ",'" + ev.Local + "'," + (int)ev.Status + "," + ev.EntryFee + ","+ ev.SportId + "," + ev.UserId + "," + ev.TeamMax + ");";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                            int rowsAff = cmd.ExecuteNonQuery();
                            conn.Close();

                            if (rowsAff == 1) return Ok();
                        }
                    }
                    catch (Exception e)
                    {
                        return StatusCode(StatusCodes.Status503ServiceUnavailable);
                    }
                }
                return Unauthorized();
            } catch (Exception e)
            {
                return BadRequest();
            }
        }


        private bool ValidateObject(Event eve) {
            return true;
        }

    }
}
