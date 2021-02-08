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

        [Authorize]
        [HttpPost("registerUser")]
        public ActionResult Register([FromBody] User user)
        {
            try
            {
                //RootObject root = JsonConvert.DeserializeObject<RootObject>(use);
                if (user.ValidateObject())
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                        {
                            conn.Open();
                            //Parameterized query
                            //Create account
                            string query = "INSERT INTO user(name,email,password,accountid)" +
                                "VALUES(@name,@email,@pass,@acc);";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                            NpgsqlParameter p_name = new NpgsqlParameter("@name", user.Name);
                            cmd.Parameters.Add(p_name);

                            NpgsqlParameter p_email = new NpgsqlParameter("@email", user.Email);
                            cmd.Parameters.Add(p_email);

                            NpgsqlParameter p_pass = new NpgsqlParameter("@pass", user.Password);
                            cmd.Parameters.Add(p_pass);

                            NpgsqlParameter p_acc = new NpgsqlParameter("@acc", user.AccountId);
                            cmd.Parameters.Add(p_acc);

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
                switch (user.ValidateUser(connString))
                {
                    case 0:
                        var token = GenerateTokenJWT();
                        return Ok(token);
                    case 1:
                        return NotFound();
                    default:
                        return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            } catch (Exception)
            {
                return BadRequest();
            }
        }


        #region SUPPORT FUNCTIONS
        private string GenerateTokenJWT()
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(120);  //120 minute valid
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, audience: audience, expires: DateTime.Now.AddMinutes(2), signingCredentials: credentials);
            //var token = new JwtSecurityToken(issuer: issuer, expires: DateTime.Now.AddMinutes(2), signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        #endregion


    }
}
