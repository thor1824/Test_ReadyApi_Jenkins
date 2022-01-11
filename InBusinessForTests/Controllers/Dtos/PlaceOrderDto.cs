using System.Collections.Generic;

namespace InBusinessForTests.Controllers.Dtos
{
    public class PlaceOrderDto
    {
        public int CustomerId { get; set; }
        public IList<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();
    }
    public class OrderLineDto
    {
        public int ProductId { get; set; }
        public int ToBeReserved { get; set; }
    }
    
}