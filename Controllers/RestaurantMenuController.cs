using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Databases;
using Somethingnew.Models;

namespace Somethingnew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantMenuController : ControllerBase
    {
        //Restaurant,menuitem
        private readonly DatabaseClass _db;

        public RestaurantMenuController(DatabaseClass db)
        {
            _db = db;
        }

        //Api related to restaurant tables
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
        //Api related to restaurant tables ends


        //Api related to menu items
        [HttpGet("GetMenuItems")]
        public IActionResult GetMenuItems()
        {
            var menuItems = _db.GetMenuItems();
            return Ok(menuItems);
        }
        [HttpPost("AddMenuItem")]
        public IActionResult AddMenuItem([FromBody] MenuItem item)
        {
            var result = _db.AddMenuItem(item);
            return Ok(result);
        }
        [HttpPut("UpdateMenuItem")]
        public IActionResult UpdateMenuItem([FromBody] MenuItem item)
        {
            var result = _db.UpdateMenuItem(item);
            return Ok(result);
        }
        [HttpDelete("DeleteMenuItem/{menuItemId}")]
        public IActionResult DeleteMenuItem(int menuItemId)
        {
            var result = _db.DeleteMenuItem(menuItemId);
            return Ok(result);
        }

        //Api related to menu items ends

        
    }

}
