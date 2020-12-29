using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using EventHub.Model;


namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/event")]
    public class EventController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";
        /*
         * 
        [HttpGet("getFriendlyEvents")]
        public DataSet GetFriendlyEvents()
        {
            DataSet ds = new DataSet();
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                string query = "SELECT * FROM event WHERE entryFee IS NULL;";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                da.Fill(ds, "Events");
            }
            return ds;
        }
        */

        [HttpGet("getFriendlyEvents")]
        public List<Event> GetFriendlyEvents()
        {
            List<Event> events = new List<Event>();
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string query = "SELECT * FROM event WHERE entryFee IS NULL;";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    FriendlyEvent friendly = new FriendlyEvent();

                    friendly.Id = reader.GetInt32(0);
                    friendly.Name = reader.GetString(1);
                    DateTime initialDate = new DateTime();
                    DateTime endDate = new DateTime();
                    friendly.Description = reader.GetString(4);
                    friendly.Slots = reader.GetInt32(5);
                    friendly.Local = reader.GetString(6);
                    // string status = reader.GetString();

                    events.Add(friendly);
                }

                conn.Close();
            }
            return events;
        }



        /*
        public IActionResult Index()
        {
            return View();
        }
        */
    }
}
