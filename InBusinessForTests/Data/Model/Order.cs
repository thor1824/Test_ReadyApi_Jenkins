using System;
using System.Collections.Generic;

namespace InBusinessForTests.Data.Model
{
    public class Order : Entity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime TimeOfPurchase { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }

    public class OrderLine : Entity
    {
        public int StockReserved { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public int DiscountAmountAtTimeOfPurchase { get; set; }
        public bool HadDiscount => DiscountAtTimeOfPurchase != 0.0;

        public double DiscountAtTimeOfPurchase { get; set; }
        public double PriceAtTimeOfPurchase { get; set; }
    }
}