using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Newtonsoft.Json;
using Entities.EntitiesService;


namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        string connString = "Server=127.0.0.1;Port=5432;Database=EventHub;User Id=postgres;Password=100998";
        private IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("registerUser")]
        public ActionResult Register([FromBody] User user)
        {
            try
            {
                //RootObject root = JsonConvert.DeserializeObject<RootObject>(use);
                if (ValidateObject(user))
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                        {
                            conn.Open();
                            //Create account
                            string query = "INSERT INTO user(name,email,password,accountid)" +
                                "VALUES('" + user + "','" + user + "','" + user + "'," + user + ");";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                            int rowsAff = cmd.ExecuteNonQuery(); //TEST number
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
                return StatusCode(StatusCodes.Status406NotAcceptable);
            } catch (Exception e)
            {
                return BadRequest();
            }
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] User user)
        {
            try
            {
                //RootObject root = JsonConvert.DeserializeObject<RootObject>(user);
                // ELIMINATE TRY
                switch (ValidateUser(user))
                {
                    case 0:
                        var token = GenerateTokenJWT();
                        return Ok(token);
                    case 1:
                        return NotFound();
                    default:
                        return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            } catch (Exception e)
            {
                return BadRequest();
            }
        }


        #region SUPPORT FUNCTIONS
        private int ValidateUser(User user)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM \"user\" WHERE email = '" + user.Email + "' AND password = '" + user.Password + "';";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    int rowsAff = 0;
                    reader.Read();
                    while (reader.IsOnRow)
                    {
                        rowsAff++;
                        reader.Read();
                    }

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
        private string GenerateTokenJWT()
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(5);  //5 minute valid
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, audience: audience, expires: DateTime.Now.AddMinutes(2), signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        private bool ValidateObject(User user)
        {
            return true;
        }
        #endregion


    }
}
