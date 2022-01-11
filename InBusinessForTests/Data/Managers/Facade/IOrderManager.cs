using System.Collections.Generic;
using System.Threading.Tasks;
using InBusinessForTests.Controllers;
using InBusinessForTests.Controllers.Dtos;
using InBusinessForTests.Data.Model;

namespace InBusinessForTests.Data.Managers.Facade
{
    public interface IOrderManager
    {
        Task<BusinessResponse<OrderInvoice>> PlaceOrder(PlaceOrderDto placeOrderDto);
        Task<IList<Order>> GetFromCustomer(int customerId);
        Task<IList<OrderInvoice>> GetFromCustomerAsInvoice(int customerId);
        Task<Order> GetAsync(int id);
        Task PayOrderAsync(Order order);
    }
}