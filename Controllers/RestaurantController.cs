using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Databases;

namespace Somethingnew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly DatabaseClass _db;

        public RestaurantController(DatabaseClass db)
        {
            _db = db;
        }
        [HttpGet("GetRestaurantTables")]
        public IActionResult GetRestaurantTables()
        {
            var tables = _db.GetRestaurantTables();
            return Ok(tables);
        }
    }

}
