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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;

namespace EventHub.Controllers
{
    /// <summary>
    /// Controller to manage event related functions as creation, list...
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : Controller
    {
        private IConfiguration _config;

        public EventsController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Get list of friendly events
        /// </summary>
        /// <returns></returns>
        [HttpGet("getFriendlyEvents")]
        public ActionResult GetFriendlyEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
               events = Event.GetFriendlies(_config.GetConnectionString("DefaultConnection"));
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);

        }

        /// <summary>
        /// Get list of competitive events
        /// </summary>
        /// <returns></returns>
        [HttpGet("getCompEvents")]
        public ActionResult GetCompEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
                events = Event.GetComps(_config.GetConnectionString("DefaultConnection"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);

        }

        /// <summary>
        /// Registers event
        /// </summary>
        /// <param name="ev"> Event </param>
        /// <returns> Register result </returns>
        [Authorize]
        [HttpPost("createEvent")]
        public ActionResult CreateEvent([FromBody] Event ev)
        {
            try
            {
                if (ev.ValidateEvent())
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                        {
                            conn.Open();

                            // Insert new event
                            // Parameterized
                            string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,entryfee,sportid,userid,team_max)" +
                                "VALUES(@name,@ini_date,@end_date,@desc,@slots,@local,@status,@fee,@sport,@user,@max);";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                            NpgsqlParameter p_name = new NpgsqlParameter("@name", ev.Name);
                            cmd.Parameters.Add(p_name);

                            NpgsqlParameter p_initial_date = new NpgsqlParameter("@ini_date", ev.InitialDate);
                            cmd.Parameters.Add(p_initial_date);

                            NpgsqlParameter p_end_date = new NpgsqlParameter("@end_date", ev.EndDate);
                            cmd.Parameters.Add(p_end_date);

                            NpgsqlParameter p_desc = new NpgsqlParameter("@desc", ev.Description);
                            cmd.Parameters.Add(p_desc);

                            NpgsqlParameter p_slots = new NpgsqlParameter("@slots", ev.Slots);
                            cmd.Parameters.Add(p_slots);

                            NpgsqlParameter p_local = new NpgsqlParameter("@local", ev.Local);
                            cmd.Parameters.Add(p_local);

                            NpgsqlParameter p_status = new NpgsqlParameter("@status", (int)ev.Status);
                            cmd.Parameters.Add(p_status);

                            NpgsqlParameter p_fee;
                            if (ev.EntryFee == null) p_fee = new NpgsqlParameter("@fee", DBNull.Value);
                            else p_fee = new NpgsqlParameter("@fee", ev.EntryFee);
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

        /// <summary>
        /// Gets coordinations from a local
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        [HttpGet("getCoordinates/{local}")]
        public ActionResult GetCoordinates([FromRoute] string local)
        {
            #region GET_REQUEST_GEOCODE
            string baseUrl = "https://maps.googleapis.com/maps/api/geocode/"; // base url request
            string key = "AIzaSyDR7EOwVknHetfyX1zoATYf3WQqtv8lFY0"; // API key

            // Encode of local
            string encodedLocal = WebUtility.UrlEncode(local);

            string query = "json?address=[ADDRESS]&key=[KEY]";

            // URL replacements to actual value
            query = query.Replace("[ADDRESS]", encodedLocal); // received local
            query = query.Replace("[KEY]", key); // API key

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            // Define result type: JSON 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Wait result
            HttpResponseMessage response = client.GetAsync(query).Result;
            #endregion

            // Check if it's returned 200
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result; // answer from GET

                //       Json treatment
                JObject obj = JsonConvert.DeserializeObject(result) as JObject; // Aux object to remove identation           
                string unindentedJson = obj.ToString(Formatting.None); // Remove identation

                return Ok(unindentedJson);
            }
            return BadRequest();
        }

            /*
            [HttpPost("joinEvent")] //CHECK FIRST
            public ActionResult JoinEvent([FromRoute] Event ev)
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

                            string query = "update event set name = '" + ev.Name + "', initial_date = '" + ev.InitialDate.ToString("yyyy-MM-dd") + "',end_Date = '" + ev.EndDate.ToString("yyyy-MM-dd") + "', description = '" + ev.Description + "', " +
                                "slots = " + ev.Slots + ", local = '" + ev.Local + "', status = " + (int)ev.Status + ", entryFee = " + ev.EntryFee + ", sportid = " + ev.SportId + ", userid = " + ev.UserId + ",  team_max = " + ev.TeamMax + " where id = " + ev.Id + "; ";
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
            */
        }
}
