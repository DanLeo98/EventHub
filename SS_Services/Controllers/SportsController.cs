using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.EntitiesService;
using Npgsql;
using Microsoft.AspNetCore.Http;


namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/sports")]
    public class SportsController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";

        [HttpGet("getSports")]
        public ActionResult GetSports()
        {
            List<Sport> sports = new List<Sport>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query = "SELECT * FROM sport;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Sport sport = new Sport();
                        sport.Id = reader.GetInt32(0);
                        sport.Name = reader.GetString(1);
                        sports.Add(sport);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(sports);

        }
    }
}
