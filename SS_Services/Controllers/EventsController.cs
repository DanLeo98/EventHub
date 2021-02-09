using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        private IConfiguration _config;

        public EventsController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpGet("getFriendlyEvents")]
        public ActionResult GetFriendlyEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
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
                using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
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

        [Authorize] //WORK FROM SCRATCH
        [HttpPut("editEvent")]
        public ActionResult EditEvent([FromBody] Event ev)
        {
            if (ev.ValidateObject())
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
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
       
        [Authorize]
        [HttpPost("createEvent")]
        public ActionResult CreateEvent([FromBody] Event ev)
        {
            try
            {
                if (ev.ValidateObject())
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                        {
                            conn.Open();

                            string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,entryFee,sportid,userid,team_max)" +
                                "VALUES(@name,@ini_date,@end_date,@desc,@slots,@local,@status,@fee,@sport,@user,@max);";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                            NpgsqlParameter p_name = new NpgsqlParameter("@name", ev.Name);
                            cmd.Parameters.Add(p_name);

                            NpgsqlParameter p_initial_date = new NpgsqlParameter("@ini_date", ev.InitialDate.ToString("yyyy-MM-dd"));
                            cmd.Parameters.Add(p_initial_date);

                            NpgsqlParameter p_end_date = new NpgsqlParameter("@end_date", ev.EndDate.ToString("yyyy-MM-dd"));
                            cmd.Parameters.Add(p_end_date);

                            NpgsqlParameter p_desc = new NpgsqlParameter("@desc", ev.Description);
                            cmd.Parameters.Add(p_desc);

                            NpgsqlParameter p_slots = new NpgsqlParameter("@slots", ev.Slots);
                            cmd.Parameters.Add(p_slots);

                            NpgsqlParameter p_local = new NpgsqlParameter("@local", ev.Local);
                            cmd.Parameters.Add(p_local);

                            NpgsqlParameter p_status = new NpgsqlParameter("@status", ev.Status);
                            cmd.Parameters.Add(p_status);

                            NpgsqlParameter p_fee = new NpgsqlParameter("@fee", ev.EntryFee);
                            cmd.Parameters.Add(p_fee);

                            NpgsqlParameter p_sport = new NpgsqlParameter("@sport", ev.SportId);
                            cmd.Parameters.Add(p_sport);

                            NpgsqlParameter p_user = new NpgsqlParameter("@user", ev.UserId);
                            cmd.Parameters.Add(p_user);

                            NpgsqlParameter p_max = new NpgsqlParameter("@max", ev.TeamMax);
                            cmd.Parameters.Add(p_max);
                            
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

        [HttpPost("joinEvent")] //CHECK FIRST
        public ActionResult JoinEvent([FromBody] Event ev)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "insert into team(position,eventId) values (0,@id);";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    NpgsqlParameter p_id = new NpgsqlParameter("@id", ev.Id);
                    cmd.Parameters.Add(p_id);

                    int rowsAff = cmd.ExecuteNonQuery();
                    if (rowsAff == 1)
                    {
                        query = "select id from team where eventid = "+ev.Id+";";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(query, conn);
                        int teamId = (int)cmd1.ExecuteScalar();
                        query = "insert into roster values("+ev.UserId+","+teamId+");";
                        NpgsqlCommand cmd2 = new NpgsqlCommand(query, conn);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    } else
                    {
                        conn.Close();
                        return NotFound();
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok();
        }

        

    }
}
