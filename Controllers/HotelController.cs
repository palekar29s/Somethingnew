using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Models;

namespace Somethingnew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly DatabaseClass _db;

        public HotelController(DatabaseClass db)
        {
            _db = db;
        }


        //[HttpGet]
        //public ActionResult<List<Hoteldetail>> GetHotels()
        //{
        //    return Ok(hotels);
        //}


        //[HttpGet]
        //public IActionResult GetFoods()
        //{
        //    return Ok(foods);
        //}



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
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _db.GetUsers();
            return Ok(users);
        }
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
                Token = token
            });
        }
    }
}
