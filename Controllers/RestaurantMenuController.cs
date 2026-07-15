using Microsoft.AspNetCore.Authorization;
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
        // =============================
        // Restaurant Tables APIs
        // =============================

        [Authorize(Roles = "Admin,Waiter,Cashier")]
        [HttpGet("GetRestaurantTables")]
        public IActionResult GetRestaurantTables()
        {
            var tables = _db.GetRestaurantTables();
            return Ok(tables);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRestaurantTable")]
        public IActionResult AddRestaurantTable([FromBody] RestaurantTable table)
        {
            var result = _db.AddRestaurantTable(table);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRestaurantTable")]
        public IActionResult UpdateRestaurantTable([FromBody] RestaurantTable table)
        {
            var result = _db.UpdateRestaurantTable(table);
            return Ok(new
            {
                success = true,
                message = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateTableStatus/{tableId}")]
        public IActionResult UpdateTableStatus(int tableId, [FromBody] string status)
        {
            var result = _db.UpdateTableStatus(tableId, status);
            return Ok(new
            {
                success = true,
                message = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRestaurantTable/{tableId}")]
        public IActionResult DeleteRestaurantTable(int tableId)
        {
            var result = _db.DeleteRestaurantTable(tableId);
            return Ok(new
            {
                success = true,
                message = result
            });
        }
        //Api related to restaurant tables ends


        //Api related to menu items
        // =============================
        // Menu Items APIs
        // =============================

        [Authorize(Roles = "Admin,Waiter")]
        [HttpGet("GetMenuItems")]
        public IActionResult GetMenuItems()
        {
            return Ok(_db.GetMenuItems());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddMenuItem")]
        public IActionResult AddMenuItem([FromBody] MenuItem item)
        {
            return Ok(_db.AddMenuItem(item));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateMenuItem")]
        public IActionResult UpdateMenuItem([FromBody] MenuItem item)
        {
            return Ok(_db.UpdateMenuItem(item));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateMenuAvailability/{menuItemId}")]
        public IActionResult UpdateMenuAvailability(int menuItemId, [FromBody] bool isAvailable)
        {
            return Ok(_db.UpdateMenuAvailability(menuItemId, isAvailable));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteMenuItem/{menuItemId}")]
        public IActionResult DeleteMenuItem(int menuItemId)
        {
            return Ok(_db.DeleteMenuItem(menuItemId));
        }
        //Api related to menu items ends


    }

}
