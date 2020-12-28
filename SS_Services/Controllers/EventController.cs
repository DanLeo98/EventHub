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
        
        [HttpGet("getOpenEvents")]
        public DataSet GetOpenEvents()
        {
            //var test = ConfigurationManager.ConnectionStrings["eventHubconnString"].ConnectionString;
            DataSet ds = new DataSet();
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998"))
            {
                string query = "SELECT * FROM event";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                da.Fill(ds, "Events");
            }
            return ds;
        }

        
        /*
        public IActionResult Index()
        {
            return View();
        }
        */
    }
}
