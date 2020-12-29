using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using EventHub.Model;

using System.Data.SqlClient;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/event")]
    public class EventController : Controller
    {
        
        [HttpGet("getOpenEvents")]
        public DataSet GetOpenEvents()
        {
            int a = ConfigurationManager.ConnectionStrings["eventHubconnString"].ConnectionString.Length;
            
            string test = "Server=127.0.0.1;Port=5432;Database=NewEventHub;User Id=postgres;Password=Passworld";
            DataSet ds = new DataSet();
            using (NpgsqlConnection conn = new NpgsqlConnection(test))
            {
                string query = "SELECT * FROM event";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                da.Fill(ds, "event");
            }
            return ds;
        }


        [HttpGet("newGetOpenEvents")]
        public DataSet NewGetOpenEvents()
        {
            string table = "event";
       
            string stringConnect = ConfigurationManager.ConnectionStrings["eventHubconnString"].ConnectionString;
            NpgsqlConnection sqlConn = new NpgsqlConnection(stringConnect);

            string query = "SELECT * FROM event";

            NpgsqlDataAdapter sqldataAdapter = new NpgsqlDataAdapter(query, sqlConn);
            DataSet dataSet = new DataSet();

            sqldataAdapter.Fill(dataSet, table);

            return dataSet;
        }

            /*
            public IActionResult Index()
            {
                return View();
            }
            */
        }
}
