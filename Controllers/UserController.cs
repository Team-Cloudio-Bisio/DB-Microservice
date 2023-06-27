using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DBMicroservice.Configuration;
using DBMicroservice.Data;
using DBMicroservice.Model;
using Microsoft.AspNetCore.Mvc;

namespace DBMicroservice.Controllers {
        
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase {
        
        private readonly DBMicroservice.Configuration.IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        private DBUserContext _context;

        public UserController(DBMicroservice.Configuration.IConfiguration configuration, ILogger<UserController> logger) {
            _logger = logger;
            _configuration = configuration;

            _context = new DBUserContext(_configuration.GetDBConnectionString());
        }

        [HttpGet("", Name = "GetUser")]
        public async Task<IEnumerable<User>> Get() {
            List<User> res = await _context.GetUsers();
            _logger.Log(LogLevel.Warning, "gianni");

            return Enumerable.Range(0, res.Count).Select(i => res[i]);
        }
        
        [HttpPost("", Name = "InsertUser")]
        public async Task<IActionResult> Insert(User user) {
            int res = await _context.InsertUser(user);

            if (res == 1)
                return StatusCode(200, "Insert succesful!");
            else
                return StatusCode(401, "Insert unsuccesful");
        }

        [HttpDelete("", Name = "DeleteUser")]
        public async Task<IActionResult> Delete(string username) {
            int res = await _context.DeleteUser(username);

            if (res == 1)
                return StatusCode(200, "Delete succesful!");
            else
                return StatusCode(401, "Delete unsuccesful");
        }
        
        [HttpPatch("", Name = "PatchUser")]
        public async Task<IActionResult> Patch(User user) {
            int res = await _context.PatchUser(user);

            if (res == 1)
                return StatusCode(200, "Patch succesful!");
            else
                return StatusCode(401, "Patch unsuccesful");
        }

    }
}
