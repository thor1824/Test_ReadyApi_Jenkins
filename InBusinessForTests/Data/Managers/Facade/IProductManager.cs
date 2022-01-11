using System.Collections.Generic;
using System.Threading.Tasks;
using InBusinessForTests.Controllers;
using InBusinessForTests.Data.Model;

namespace InBusinessForTests.Data.Managers.Facade
{
    public interface IProductManager
    {
        Task<IList<Product>> GetAll();
        Task<Product> SetStockAsync(int id, SetStockDto dto);
        Task<Product> AddAsync(AddProductDto dto);
        Task<Product> GetAsync(int id);
    }
}