using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EventHub.Controllers
{
    /// <summary>
    /// Controller that manages sports related functions (get and add)
    /// </summary>
    [ApiController]
    [Route("api/sports")]
    public class SportsController : Controller
    {
        private IConfiguration _config;

        public SportsController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Get list of sports
        /// </summary>
        /// <returns></returns>
        [HttpGet("getSports")]
        public ActionResult GetSports()
        {
            List<Sport> sports = new List<Sport>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection((_config.GetConnectionString("DefaultConnection"))))
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
