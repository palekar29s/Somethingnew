using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Somethingnew.Databases;
using Somethingnew.Models;

namespace Somethingnew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrCaPayController : ControllerBase
    {
        //Category ,Order,Payment
        private readonly DatabaseClass _db;

        public OrCaPayController(DatabaseClass db)
        {
            _db = db;
        }


        //order related APIs
        // ============================
        // Orders APIs
        // ============================

        [Authorize(Roles = "Admin,Waiter")]
        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = _db.CreateOrder(request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetOrders")]
        public IActionResult GetOrders()
        {
            var result = _db.GetOrders();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Waiter")]
        [HttpGet("GetOrdersByWaiter/{waiterId}")]
        public IActionResult GetOrdersByWaiter(int waiterId)
        {
            var result = _db.GetOrdersByWaiter(waiterId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Chef,Waiter")]
        [HttpGet("GetOrderItems")]
        public IActionResult GetOrderItems()
        {
            var result = _db.GetOrderItems();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Chef,Waiter")]
        [HttpGet("GetOrderItemsByOrder/{orderId}")]
        public IActionResult GetOrderItemsByOrder(int orderId)
        {
            var result = _db.GetOrderItemsByOrder(orderId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Chef")]
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public IActionResult UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            var result = _db.UpdateOrderStatus(orderId, status);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Chef")]
        [HttpPut("UpdateOrderItemStatus/{orderItemId}")]
        public IActionResult UpdateOrderItemStatus(int orderItemId, [FromBody] string status)
        {
            var result = _db.UpdateOrderItemStatus(orderItemId, status);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteOrder/{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var result = _db.DeleteOrder(orderId);
            return Ok(result);
        }
        //order related APIs ends

        //payment related APIs
        [HttpGet("GetPayments")]
        public IActionResult GetPayments()
        {
            return Ok(_db.GetPayments());
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpGet("GetPaymentByOrder/{orderId}")]
        public IActionResult GetPaymentByOrder(int orderId)
        {
            return Ok(_db.GetPaymentByOrder(orderId));
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpPost("AddPayment")]
        public IActionResult AddPayment([FromBody] Payment payment)
        {
            return Ok(_db.AddPayment(payment));
        }

        [Authorize(Roles = "Admin,Cashier")]
        [HttpPut("UpdatePayment")]
        public IActionResult UpdatePayment([FromBody] Payment payment)
        {
            return Ok(_db.UpdatePayment(payment));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePayment/{paymentId}")]
        public IActionResult DeletePayment(int paymentId)
        {
            return Ok(_db.DeletePayment(paymentId));
        }

        //payment related APIs ends

        //categories API start
        // =============================
        // Categories APIs
        // =============================

        [Authorize(Roles = "Admin,Waiter")]
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _db.GetCategories();
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] Category category)
        {
            var result = _db.AddCategory(category);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            var result = _db.UpdateCategory(category);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategoryStatus/{categoryId}")]
        public IActionResult UpdateCategoryStatus(int categoryId, [FromBody] bool isActive)
        {
            var result = _db.UpdateCategoryStatus(categoryId, isActive);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory/{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            var result = _db.DeleteCategory(categoryId);
            return Ok(result);
        }



        //categories API ends


    }
}
