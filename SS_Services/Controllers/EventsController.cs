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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
                        if (ev.CreateEvent(_config.GetConnectionString("DefaultConnection")) == 0)
                        {
                            return Ok();
                        }
                        else
                        {
                            return BadRequest();
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
