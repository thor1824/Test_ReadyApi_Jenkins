using System;
using System.Collections.Generic;

namespace InBusinessForTests.Data.Model
{
    public class OrderInvoice
    {
        public int OrderId { get; set; }
        public string Currency { get; set; } = "N/A";
        public double DeliveryCost { get; set; } = 0.0;
        public double ProductPriceAfterDiscount { get; set; } = 0.0;
        public double ProductPriceTotal { get; set; } = 0.0;
        public IList<InvoiceOrderLine> ProductsOrdered { get; set; } = new List<InvoiceOrderLine>();

        public double InvoiceTotalPrice => ProductPriceAfterDiscount + DeliveryCost;

        public DateTime TimeOfPurchase { get; set; }
    }
    
    public class InvoiceOrderLine
    {
        public int Amount { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrderId { get; set; }
        public int DiscountAmount { get; set; }
        public bool HadDiscount => Discount != 0.0;
        public bool WasQualifiedForDiscount => Discount != 0.0;

        public double Discount { get; set; }
        public double Price { get; set; }
    }
}