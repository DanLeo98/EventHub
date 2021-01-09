﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("events")]
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
                        comp.EntryFee = reader.GetInt32(8);
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
        /*
        public ActionResult EditEvent
        {
            retu
        }
        */

        [Authorize]
        [HttpPost("createFriendlyEvent")]
        public ActionResult CreateFriendlyEvent(string ev, int userId)
        {
            try
            {
            RootObject root = JsonConvert.DeserializeObject<RootObject>(ev);
                if (ValidateObject(root.Event))
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                        {
                            conn.Open();

                            string query = "INSERT INTO event(name,initial_date,end_date,description,slots,local,status,sportid,userid,team_max)" +
                                "VALUES('" + root.Event.Name + "','" + root.Event.InitialDate.ToString("yyyy-MM-dd") + "','" + root.Event.EndDate.ToString("yyyy-MM-dd") + "','"
                                + root.Event.Description + "'," + root.Event.Slots + ",'" + root.Event.Local + "'," + (int)root.Event.Status + "," + root.Event.SportId + "," + userId + "," + root.Event.TeamMax + ");";
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
