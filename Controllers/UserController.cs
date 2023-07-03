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
        
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private DBUserContext _context;

        public UserController(ILogger<UserController> logger, IConfiguration configuration) {
            _configuration = configuration;
            _logger = logger;
            
            logger.Log(LogLevel.Information, "Started User Controller");

            string connstring = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            
            _context = new DBUserContext(connstring);
        }

        [HttpGet("", Name = "GetUser")]
        public async Task<IEnumerable<User>> Get() {
            List<User> res = await _context.GetUsers();

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
