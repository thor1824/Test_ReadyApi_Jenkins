using System.Threading.Tasks;
using InBusinessForTests.Data.Managers;
using InBusinessForTests.Data.Managers.Facade;
using Microsoft.AspNetCore.Mvc;

namespace InBusinessForTests.Controllers
{
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;

        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody]AddProductDto dto)
        {
            var products = await _productManager.AddAsync(dto);
            return Ok(products);
        }
        
        [HttpPut("{id:int}/stock")]
        public async Task<IActionResult> SetStock(int id, [FromBody] SetStockDto dto)
        {
            var products = await _productManager.SetStockAsync(id, dto);
            return Ok(products);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productManager.GetAll();
            return Ok(products);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var products = await _productManager.GetAll();
            return Ok(products);
        }
        
    }

    public class AddProductDto
    {
        public string Name { get; set; }
        public int Stock { get; set; } 
        public double Price { get; set; }
        public double DiscountInPercentage { get; set; }
        public int AmountBoughtForDiscount { get; set; }
    }

    public class SetStockDto
    {
        public int NewStock { get; set; }
    }
}