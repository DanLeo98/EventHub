using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Model;
using Npgsql;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";

        [HttpPost("register")]
        public ActionResult Register(string use)
        {
            User user = JsonSerializer.Deserialize<User>(use);
            if (user.ValidateObject())
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        //Create account
                        string query = "INSERT INTO user(name,email,password,accountid)" +
                            "VALUES('" + user.Name + "','" + user.Email + "','" + user.Password + "'," + user.account.AccountId + ");";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        int rowsAff = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAff == 1) return Ok();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            }
            return Unauthorized();
        }
        


        /*
        public IActionResult Index()
        {
            return View();
        }
        */
    }
}
