using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InBusinessForTests.Controllers.Dtos;
using InBusinessForTests.Data.Managers.Facade;
using Microsoft.AspNetCore.Mvc;

namespace InBusinessForTests.Controllers
{
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrderController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var result = await _orderManager.PlaceOrder(dto);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.ToArray() });
            }

            return Ok(result.Entity);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([Required, FromQuery] int customerId,
            [FromQuery] bool asInvoice = false)
        {
            if (asInvoice)
            {
                var invoices = await _orderManager.GetFromCustomerAsInvoice(customerId);
                return Ok(invoices);
            }

            var orders = await _orderManager.GetFromCustomer(customerId);
            return Ok(orders);
        }

        [HttpDelete]
        public IActionResult CancelOrder([FromRoute] int id)
        {
            // from customer
            // is paid for
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PayOrder([FromRoute] int id)
        {
            var order = await _orderManager.GetAsync(id);
            await _orderManager.PayOrderAsync(order);
            return NoContent();
        }
    }
}