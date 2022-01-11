using System;
using System.Threading.Tasks;
using InBusinessForTests.Data.Managers.Facade;
using InBusinessForTests.Data.Model;
using InBusinessForTests.Data.Repository;

namespace InBusinessForTests.Data.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private readonly Repository<Customer> _repository;

        public CustomerManager(Repository<Customer> repository)
        {
            _repository = repository;
        }

        public async Task<BusinessResponse<Customer>> DeleteAsync(int id)
        {
            try
            {
                var customer = await _repository.GetAsync(id);
                await _repository.DeleteAsync(customer);
                return BusinessResponse<Customer>.Success;
            }
            catch (Exception e)
            {
                return BusinessResponse<Customer>.Failed(new BusinessError { Code = "?", Description = e.Message });
            }
        }

        public async Task<BusinessResponse<Customer>> CreateAsync(string name)
        {
            try
            {
                var entity = new Customer { Name = name };
                var created = await _repository.AddAsync(entity);
                return BusinessResponse<Customer>.SuccessWithEntity(created);
            }
            catch (Exception e)
            {
                return BusinessResponse<Customer>.Failed(new BusinessError { Code = "?", Description = e.Message });
            }
        }
    }
}