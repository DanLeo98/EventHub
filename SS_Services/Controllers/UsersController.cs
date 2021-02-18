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
                    if (user.Register(_config.GetConnectionString("DefaultConnection")) == 0)
                    {
                        return Ok();
                    }
                    else
                    {
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

            var token = new JwtSecurityToken(issuer: issuer, audience: audience, expires: expiry, signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        #endregion
    }
}
