﻿/*
 * Authors: João Rodrigues and Daniel Leonard
 * Project: Practical Work, implementing services
 * Current Solution: Client of services for sport events
 * 
 * 
 * Subject: Integration of Informatic Systems
 * Degree: Graduation on Engeneer of Informatic Systems
 * Lective Year: 2020/21
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController : Controller
    {
        private IConfiguration _config;

        public TeamController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("getTeams/{userId}")]
        public ActionResult getTeams([FromRoute] int userId)
        {
            Dictionary<Event,int> events = new Dictionary<Event,int>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "select e.id,e.name,e.description,e.entryfee,t.id " +
                        "from team t inner join event e on t.eventid = e.id " +
                        "inner join roster r on t.id = r.teamid " +
                        "where r.userid = "+userId+";";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Event ev = new Event();

                        ev.Id = reader.GetInt32(0);
                        ev.Name = reader.GetString(1);
                        ev.Description = reader.GetString(2);
                        ev.EntryFee = reader.GetInt32(3);

                        events.Add(ev, reader.GetInt32(4));
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(events);
        }

        [HttpGet("getTeamDetails/{teamId}")]
        public ActionResult getTeamDetails([FromRoute] int teamId)
        {
            List<User> teamMembers = new List<User>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "select u.name,u.email from roster r inner join \"user\" u on r.userid = u.id" +
                        " inner join team t on t.id = r.teamid" +
                        " where t.id = "+teamId+"; ";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        User user = new User();

                        user.Name = reader.GetString(0);
                        user.Email = reader.GetString(1);
                        teamMembers.Add(user);
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(teamMembers);
        }


    }
}
