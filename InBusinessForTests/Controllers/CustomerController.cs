using System.Linq;
using System.Threading.Tasks;
using InBusinessForTests.Data.Managers;
using InBusinessForTests.Data.Managers.Facade;
using Microsoft.AspNetCore.Mvc;

namespace InBusinessForTests.Controllers
{
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerManager _customerManager;

        public CustomerController(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var result = await _customerManager.CreateAsync(dto.Name);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.ToArray()
                });
            }
            var newlyCreated = result.Entity;
            return Created("" + newlyCreated.Id, newlyCreated);
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            var result = await _customerManager.DeleteAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result);
                
            }
            return NoContent();
        }
    }
}