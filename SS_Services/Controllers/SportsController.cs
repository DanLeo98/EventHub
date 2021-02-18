/*
 * Authors: João Rodrigues and Daniel Leonard
 * Project: Practical Work, implementing services
 * Current Solution: Client of services for sport events
 * 
 * 
 * Subject: Integration of Informatic Systems
 * Degree: Graduation on Engeneer of Informatic Systems
 * Lective Year: 2020/21
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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
                sports = Sport.GetSports(_config.GetConnectionString("DefaultConnection"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Ok(sports);

        }
    }
}
