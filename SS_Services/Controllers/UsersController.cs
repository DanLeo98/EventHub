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
    /// <summary>
    /// Controller that manages login and register
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Registers an user
        /// </summary>
        /// <param name="user"> User </param>
        /// <returns> Register Result </returns>
        [HttpPost("registerUser")]
        public ActionResult Register([FromBody] User user)
        {
            try
            {
                // Validate Object
                if (user.ValidateObject())
                {
                    try
                    {
                        // Connects to DB
                        using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                        {
                            conn.Open();
                            // Creates account for user
                            string query0 = "INSERT INTO account(bankId)" +
                                "VALUES(NULL);";
                            NpgsqlCommand cmd0 = new NpgsqlCommand(query0, conn);
                            cmd0.ExecuteNonQuery();
                            
                            // Gets user account id
                            string query1 = "SELECT id from account;";
                            NpgsqlCommand cmd1 = new NpgsqlCommand(query1, conn);
                            var reader = cmd1.ExecuteReader();

                            reader.Read();
                            int id = 0;
                            while (reader.IsOnRow)
                            {
                                id = reader.GetInt32(0);
                                reader.Read();
                            }
                            reader.Close();

                            // Registers user
                            string query = "INSERT INTO \"user\"(name,email,password,accountid)" +
                                "VALUES(@name,@email,@pass,@acc);";
                            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                            NpgsqlParameter p_name = new NpgsqlParameter("@name", user.Name);
                            cmd.Parameters.Add(p_name);

                            NpgsqlParameter p_email = new NpgsqlParameter("@email", user.Email);
                            cmd.Parameters.Add(p_email);

                            NpgsqlParameter p_pass = new NpgsqlParameter("@pass", user.Password);
                            cmd.Parameters.Add(p_pass);

                            NpgsqlParameter p_acc = new NpgsqlParameter("@acc", id);
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


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="user"> User </param>
        /// <returns>Login result</returns>
        [HttpPost("login")]
        public ActionResult Login([FromBody] User user)
        {
            try
            {
                int id;
                // Validate user exists
                switch (user.ValidateUser(_config.GetConnectionString("DefaultConnection"), out id))
                {
                    case 0:
                        var token = GenerateTokenJWT();
                        user.Id = id;
                        Session session = new Session(token, user);
                        return Ok(session);
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
        /// <summary>
        /// Generation of token
        /// </summary>
        /// <returns>Token string</returns>
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
