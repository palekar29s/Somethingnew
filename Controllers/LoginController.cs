using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Databases;
using Somethingnew.Models;

namespace Somethingnew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseClass _db;

        public LoginController(DatabaseClass db)
        {
            _db = db;
        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                return Ok("yes done ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _db.GetUsers();
            return Ok(users);
        }
       // [Authorize(Roles = "Admin,Waiter,Cashier")]
        [HttpPost("login")]
        public IActionResult Login(LoginDto model)
        {
            var user = _db.ValidateUser(model.Email, model.Password);

            if (user == null)
            {
                return Unauthorized("Invalid Credentials");
            }

            var token = _db.GenerateToken(user);

            return Ok(new
            {
                token = token,
                role = user.Role.Substring(0, 1).ToUpper() + user.Role.Substring(1).ToLower()
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public IActionResult AddUser([FromBody] Users user)
        {
            var result = _db.AddUser(user);

            if (result)
            {
                return Ok(new
                {
                    Message = "User added successfully"
                });
            }

            return BadRequest(new
            {
                Message = "Failed to add user"
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var result = _db.DeleteUser(userId);
            return Ok(result);
        }
    }
}
