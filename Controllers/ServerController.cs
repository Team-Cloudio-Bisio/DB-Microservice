using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DBMicroservice.Configuration;
using DBMicroservice.Data;
using DBMicroservice.Model;
using Microsoft.AspNetCore.Mvc;

namespace DBMicroservice.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class ServerController : ControllerBase {

        private readonly IConfiguration _configuration;
        private readonly ILogger<ServerController> _logger;
        private DBServerContext _context;

        public ServerController(ILogger<ServerController> logger, IConfiguration configuration) {
            _configuration = configuration;
            _logger = logger;
            
            _logger.Log(LogLevel.Information, "Started Server Controller");

            string connstring = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            
            _context = new DBServerContext(connstring);
        }
        
        [HttpGet("", Name = "GetServers")]
        public async Task<IEnumerable<Server>> GetServers() {
            List<Server> res = await _context.GetServers();

            return Enumerable.Range(0, res.Count).Select(i => res[i]);
        }
        
        [HttpGet("byUser", Name = "GetServersUser")]
        public async Task<IEnumerable<Server>> GetServers(string username) {
            List<Server> res = await _context.GetServers(username);

            foreach (Server s in res) {
                s.settings = await _context.GetServerSettings(s.settingsID.Value);
                s.admin = await _context.GetServerAdmins(s.serverName);
                s.whitelist = await _context.GetServerWhitelist(s.serverName);
            }

            return Enumerable.Range(0, res.Count).Select(i => res[i]);
        }

        [HttpGet("{serverName}/whitelist", Name = "GetWhitelist")]
        public async Task<IEnumerable<string>> GetWhitelist(string serverName) {
            List<string> res = await _context.GetServerWhitelist(serverName);

            return Enumerable.Range(0, res.Count).Select(i => res[i]);
        }

        [HttpGet("{serverName}/admin", Name = "GetAdmin")]
        public async Task<IEnumerable<User>> GetAdmin(string serverName) {
            List<User> res = await _context.GetServerAdmins(serverName);

            return Enumerable.Range(0, res.Count).Select(i => res[i]);
        }

        [HttpGet("{serverName}/settings", Name = "GetServerSettings")]
        public async Task<ServerSettings> GetServerSettings(string serverName) {
            ServerSettings res = await _context.GetServerSettings(serverName);

            return res;
        }

        [HttpPost("", Name = "InsertServer")]
        public async Task<IActionResult> InsertServer(Server server) {
            int res = await _context.InsertServer(server);

            if (res == 1)
                res = await _context.PatchServerSettings(server.serverName, server.settings);
            if (res == 1)
                foreach (string user in server.whitelist) {
                    await _context.InsertServerWhitelist(user, server.serverName);
                }
            if (res == 1)
                foreach (User user in server.admin) {
                    await _context.InsertServerAdmin(user.username, server.serverName);
                }

            if (res == 1)
                return StatusCode(200, "Insert succesful!");
            else
                return StatusCode(401, "Insert unsuccesful");
        }

        [HttpPost("admin", Name = "InsertAdmin")]
        public async Task<IActionResult> InsertAdmin(string username, string serverName) {
            int res = await _context.InsertServerAdmin(username, serverName);

            if (res == 1)
                return StatusCode(200, "Insert succesful!");
            else
                return StatusCode(401, "Insert unsuccesful");
        }

        [HttpPost("whitelist", Name = "InsertWhitelist")]
        public async Task<IActionResult> InsertWhitelist(string username, string serverName) {
            int res = await _context.InsertServerWhitelist(username, serverName);

            if (res == 1)
                return StatusCode(200, "Insert succesful!");
            else
                return StatusCode(401, "Insert unsuccesful");
        }

        [HttpPost("settings", Name = "InsertSettings")]
        public async Task<IActionResult> InsertSettings(string serverName, ServerSettings settings) {
            int res = await _context.InsertServerSettings(serverName, settings);

            if (res == 1)
                return StatusCode(200, "Insert succesful!");
            else
                return StatusCode(401, "Insert unsuccesful");
        }

        [HttpDelete("", Name = "DeleteServer")]
        public async Task<IActionResult> DeleteServer(string serverName) {
            int res = await _context.DeleteServer(serverName);

            if (res == 1)
                return StatusCode(200, "Delete succesful!");
            else
                return StatusCode(401, "Delete unsuccesful");
        }

        [HttpDelete("admin", Name = "DeleteAdmin")]
        public async Task<IActionResult> DeleteAdmin(string username, string serverName) {
            int res = await _context.DeleteServerAdmin(username, serverName);

            if (res == 1)
                return StatusCode(200, "Delete succesful!");
            else
                return StatusCode(401, "Delete unsuccesful");
        }

        [HttpDelete("whitelist", Name = "DeleteWhitelist")]
        public async Task<IActionResult> DeleteWhitelist(string username, string serverName) {
            int res = await _context.DeleteServerWhitelist(username, serverName);

            if (res == 1)
                return StatusCode(200, "Delete succesful!");
            else
                return StatusCode(401, "Delete unsuccesful");
        }

        [HttpPatch("", Name = "PatchServer")]
        public async Task<IActionResult> PatchServer(Server server) {
            int res = await _context.PatchServer(server);

            if (res == 1)
                return StatusCode(200, "Patch succesful!");
            else
                return StatusCode(401, "Patch unsuccesful");
        }

        [HttpPatch("settings", Name = "PatchSettings")]
        public async Task<IActionResult> PatchSettings(string serverName, ServerSettings settings) {
            int res = await _context.PatchServerSettings(serverName, settings);

            if (res == 1)
                return StatusCode(200, "Patch succesful!");
            else
                return StatusCode(401, "Patch unsuccesful");
        }

    }
        
}
