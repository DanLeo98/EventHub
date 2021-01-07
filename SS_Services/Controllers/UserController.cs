using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Model;
using Npgsql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Newtonsoft.Json;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";
        private IConfiguration _config;

        /*
        //FIX THIS ACTION
        [HttpPost("registerUser")]
        public ActionResult Register(string use)
        {
            RootObject root = JsonConvert.DeserializeObject<RootObject>(use);
            if (root.User.ValidateObject())
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        //Create account
                        string query = "INSERT INTO user(name,email,password,accountid)" +
                            "VALUES('" + root.User.Name + "','" + root.User.Email + "','" + root.User.Password + "'," + root.User.account.AccountId + ");";
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
        */

        [Authorize]
        [HttpPost("login")]
        public ActionResult Login(string user)
        {
            try
            {
                RootObject root = JsonConvert.DeserializeObject<RootObject>(user);
                switch (ValidateUser(root.User))
                {
                    case 0:
                        var token = GenerateTokenJWT();
                        return Ok(token);
                    case 1:
                        return NotFound();
                    case 2:
                        return StatusCode(StatusCodes.Status503ServiceUnavailable);
                    default:
                        return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            } catch (Exception e)
            {
                return BadRequest();
            }
        }


        #region SUPPORT FUNCTIONS
        // TURN PRIVATE AFTER TESTING
        [HttpGet("validateUseTest")]
        public int ValidateUser(User user)
        {
            if (user.ValidateObject())
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM \"user\" WHERE name = '"+user.Name+"' AND password = '"+user.Password+"';";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        int rowsAff = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAff == 1) return 0;
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 2;
                }
            }
            return 3;
        }
        private string GenerateTokenJWT()
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(5);  //5 minute valid
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, audience: audience, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        #endregion


    }
}
