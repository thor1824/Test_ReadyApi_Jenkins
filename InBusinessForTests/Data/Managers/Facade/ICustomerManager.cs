using System.Threading.Tasks;
using InBusinessForTests.Data.Model;

namespace InBusinessForTests.Data.Managers.Facade
{
    public interface ICustomerManager
    {
        Task<BusinessResponse<Customer>> DeleteAsync(int id);
        Task<BusinessResponse<Customer>> CreateAsync(string name);
    }
}