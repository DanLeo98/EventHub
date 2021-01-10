using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Entities.EntitiesService;
using Microsoft.AspNetCore.Http;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";

        [HttpGet("getTeams/{id}")]
        public ActionResult getTeams([FromRoute] int userId)
        {
            Dictionary<Event,int> events = new Dictionary<Event,int>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
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

        [HttpGet("getTeamDetails/{id}")]
        public ActionResult getTeamDetails([FromRoute] int teamId)
        {
            List<User> teamMembers = new List<User>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
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
