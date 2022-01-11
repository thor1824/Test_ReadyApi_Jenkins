using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBusinessForTests.Controllers;
using InBusinessForTests.Data.Managers.Facade;
using InBusinessForTests.Data.Model;
using InBusinessForTests.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace InBusinessForTests.Data.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly Repository<Product> _repository;

        public ProductManager(Repository<Product> repository)
        {
            _repository = repository;
        }
        
        public async Task<IList<Product>> GetAll()
        {
            return (await _repository.GetAll()).ToList();
        }

        public async Task<Product> SetStockAsync(int id, SetStockDto dto)
        {
            var product = await _repository.GetAsync(id);
            product.Stock = dto.NewStock;
            await _repository.UpdateAsync(product);
            return product;
        }

        public async Task<Product> AddAsync(AddProductDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                Reserved = 0,
                DiscountInPercentage = dto.DiscountInPercentage,
                AmountBoughtForDiscount = dto.AmountBoughtForDiscount,
            };
            return await _repository.AddAsync(newProduct);
        }

        public async Task<Product> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
    }
}