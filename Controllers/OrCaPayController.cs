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
        [HttpPost("AddOrder")]
        public IActionResult AddOrder([FromBody] Order order)
        {
            var result = _db.AddOrder(order);
            return Ok(result);
        }
        [HttpDelete("DeleteOrder/{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var result = _db.DeleteOrder(orderId);
            return Ok(result);
        }
        [HttpPut("UpdateOrder")]
        public IActionResult UpdateOrder([FromBody] Order order)
        {
            var result = _db.UpdateOrder(order);
            return Ok(result);
        }

        [HttpGet("GetOrdersByWaiter/{waiterId}")]
        public IActionResult GetOrdersByWaiter(int waiterId)
        {
            var orders = _db.GetOrdersByWaiterId(waiterId);
            return Ok(orders);
        }

        //order related APIs ends

        //payment related APIs
        [HttpGet("GetPaymentsByWaiter/{waiterId}")]
        public IActionResult GetPaymentsByWaiter(int waiterId)
        {
            var payments = _db.GetPaymentsByWaiterId(waiterId);
            return Ok(payments);
        }
        [HttpGet("GetPaymentById/{waiterId}/{paymentId}")]
        public IActionResult GetPaymentById(int waiterId, int paymentId)
        {
            var payment = _db.GetPaymentById(waiterId, paymentId);

            if (payment == null)
                return NotFound("Payment not found");

            return Ok(payment);
        }

        [HttpPost("AddPayment")]
        public IActionResult AddPayment([FromBody] Payment payment)
        {
            var result = _db.AddPayment(payment);
            return Ok(result);
        }

        [HttpPut("UpdatePayment")]
        public IActionResult UpdatePayment([FromBody] Payment payment)
        {
            var result = _db.UpdatePayment(payment);
            return Ok(result);
        }

        [HttpDelete("DeletePayment/{paymentId}")]
        public IActionResult DeletePayment(int paymentId)
        {
            var result = _db.DeletePayment(paymentId);
            return Ok(result);
        }



        //payment related APIs ends

        //categories API start
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _db.GetCategories();
            return Ok(categories);
        }

        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] Category category)
        {
            var result = _db.AddCategory(category);
            return Ok(result);
        }
        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            var result = _db.UpdateCategory(category);
            return Ok(result);
        }

        [HttpDelete("DeleteCategory/{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            var result = _db.DeleteCategory(categoryId);
            return Ok(result);
        }



        //categories API ends


    }
}
