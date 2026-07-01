using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Databases;
using Somethingnew.Models;

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
        [HttpPost("AddRestaurantTable")]
        public IActionResult AddRestaurantTable([FromBody] RestaurantTable table)
        {
            var result = _db.AddRestaurantTable(table);
            return Ok(result);
        }

        [HttpPut("UpdateTableStatus/{tableId}")]
        public IActionResult UpdateTableStatus(int tableId, [FromBody] string status)
        {
            var result = _db.UpdateTableStatus(tableId, status);
            return Ok(result);
        }
        [HttpDelete("DeleteRestaurantTable/{tableId}")]
        public IActionResult DeleteRestaurantTable(int tableId)
        {
            var result = _db.DeleteRestaurantTable(tableId);
            return Ok(result);
        }
    }

}
