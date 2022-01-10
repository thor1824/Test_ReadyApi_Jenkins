using System.Threading.Tasks;

namespace InBusinessForTests.Data.Managers.Facade
{
    public interface ICustomerManager
    {
        Task<BusinessResponse<Customer>> DeleteAsync(int id);
        Task<BusinessResponse<Customer>> CreateAsync(string name);
    }
}