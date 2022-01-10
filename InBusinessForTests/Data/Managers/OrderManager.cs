using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBusinessForTests.Controllers;
using InBusinessForTests.Controllers.Dtos;
using InBusinessForTests.Data.Managers.Facade;
using InBusinessForTests.Data.Repository;

namespace InBusinessForTests.Data.Managers
{
    public class OrderInvoice
    {
        public string Currency { get; set; } = "N/A";
        public double DeliveryCost { get; set; } = 0.0;
        public double ProductPriceAfterDiscount { get; set; } = 0.0;
        public double ProductPriceTotal { get; set; } = 0.0;
        public IList<InvoiceOrderLine> ProductsOrdered { get; set; } = new List<InvoiceOrderLine>();

        public double InvoiceTotalPrice => ProductPriceAfterDiscount + DeliveryCost;

        public DateTime TimeOfPurchase { get; set; }
    }

    public class OrderManager : IOrderManager
    {
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<Customer> _customerRepository;
        private readonly Repository<Product> _productRepository;

        public OrderManager(
            Repository<Order> orderRepository,
            Repository<Customer> customerRepository,
            Repository<Product> productRepository
        )
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<BusinessResponse<OrderInvoice>> PlaceOrder(PlaceOrderDto placeOrderDto)
        {
            var errors = new List<BusinessError>();

            await ValidateItems(placeOrderDto, errors);

            await ValidateCustomer(placeOrderDto, errors);

            if (errors.Count > 0)
            {
                return BusinessResponse<OrderInvoice>.Failed(errors.ToArray());
            }

            var orderLines = await FinalizePurchase(placeOrderDto);

            var newOrder = new Order
            {
                CustomerId = placeOrderDto.CustomerId,
                OrderLines = orderLines,
                TimeOfPurchase = DateTime.UtcNow
            };
            var order = await _orderRepository.AddAsync(newOrder);
            var invoice = ToOrderInvoice(order);
            return BusinessResponse<OrderInvoice>.SuccessWithEntity(invoice);
        }

        public async Task<IList<Order>> GetFromCustomer(int customerId)
        {
            var orders = await _orderRepository
                .GetWhereAndIncludeThenIncludeAsync(
                    x => x.CustomerId == customerId,
                    x => x.OrderLines,
                    x => x.Product
                );
            return orders;
        }

        public async Task<IList<OrderInvoice>> GetFromCustomerAsInvoice(int customerId)
        {
            var orders = await GetFromCustomer(customerId);
            return orders.Select(ToOrderInvoice).ToList();
        }

        private OrderInvoice ToOrderInvoice(Order order)
        {
            var (price, priceWithDiscount) = CalculatePriceAsync(order.OrderLines);

            var invoice = new OrderInvoice
            {
                ProductPriceTotal = price,
                ProductPriceAfterDiscount = priceWithDiscount,
                DeliveryCost = EligibleForNoDeliveryCost(priceWithDiscount) ? 0 : 60,
                Currency = "DKK",
                ProductsOrdered = order.OrderLines.Select(ToInvoiceOrderLine).ToList(),
                TimeOfPurchase = order.TimeOfPurchase
            };
            return invoice;
        }

        private InvoiceOrderLine ToInvoiceOrderLine(OrderLine orderline)
        {
            return new InvoiceOrderLine
            {
                OrderId = orderline.OrderId,
                ProductId = orderline.ProductId,
                ProductName = orderline.Product is null ? "N/A" : orderline.Product.Name,
                Amount = orderline.StockReserved,
                Discount = orderline.DiscountAtTimeOfPurchase,
                Price = orderline.PriceAtTimeOfPurchase,
                DiscountAmount = orderline.DiscountAmountAtTimeOfPurchase,
            };
        }

        private bool EligibleForNoDeliveryCost(double price)
        {
            return price >= 500;
        }

        private (double totalPrice, double totalPriceWithDiscount) CalculatePriceAsync(List<OrderLine> orderlines)
        {
            var totalPriceWithDiscount = 0.0;
            var totalPrice = 0.0;
            foreach (var orderLine in orderlines)
            {
                var discount = orderLine.DiscountAtTimeOfPurchase;
                var price = orderLine.PriceAtTimeOfPurchase * orderLine.StockReserved;
                /*totalPriceWithDiscount += price - (price / 100 * discount);
                totalPrice += price;#1#*/
                if (orderLine.HadDiscount && orderLine.DiscountAmountAtTimeOfPurchase <= orderLine.StockReserved)
                {
                    
                    totalPriceWithDiscount +=  price -(price / 100 * discount);
                    totalPrice += price; 
                    continue;
                }
                totalPriceWithDiscount += price;
                totalPrice += price;
            }

            return (totalPrice, totalPriceWithDiscount);
        }

        private async Task<List<OrderLine>> FinalizePurchase(PlaceOrderDto placeOrderDto)
        {
            var productToBeUpdated = new List<Product>();
            var orderLines = new List<OrderLine>();
            foreach (var orderLine in placeOrderDto.OrderLines)
            {
                var product = await _productRepository.GetAsync(orderLine.ProductId);
                product.Stock -= orderLine.ToBeReserved;
                product.Reserved += orderLine.ToBeReserved;
                productToBeUpdated.Add(product);
                orderLines.Add(new OrderLine
                {
                    ProductId = product.Id,
                    StockReserved = orderLine.ToBeReserved,
                    PriceAtTimeOfPurchase = product.Price,
                    DiscountAtTimeOfPurchase = product.DiscountInPercentage,
                    DiscountAmountAtTimeOfPurchase = product.AmountBoughtForDiscount
                });
            }

            await _productRepository.UpdateBatchAsync(productToBeUpdated);
            return orderLines;
        }

        private async Task ValidateCustomer(PlaceOrderDto placeOrderDto, List<BusinessError> errors)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(placeOrderDto.CustomerId);

                if (customer.Credit < 0)
                {
                    errors.Add(new BusinessError
                    {
                        Code = "customer-unpaid-order",
                        Description = $"Customer, {customer.Id},{customer.Name}, Has unpaid Orders"
                    });
                }
            }
            catch (ArgumentException e)
            {
                errors.Add(new BusinessError
                {
                    Code = "customer-not-found",
                    Description = $"Customer, {placeOrderDto.CustomerId}, does not exists"
                });
            }
        }

        private async Task ValidateItems(PlaceOrderDto placeOrderDto, List<BusinessError> errors)
        {
            foreach (var orderLine in placeOrderDto.OrderLines)
            {
                if (0 >= orderLine.ToBeReserved)
                {
                    errors.Add(new BusinessError
                    {
                        Code = "item-reservation-invalid",
                        Description = $"Invalid Reservation amount: {orderLine.ToBeReserved}"
                    });
                }
                
                try
                {
                    var product = await _productRepository.GetAsync(orderLine.ProductId);

                    if (product.Stock < orderLine.ToBeReserved)
                    {
                        errors.Add(new BusinessError
                        {
                            Code = "item-not-in-stock",
                            Description = $"Item, {product.Id}:{product.Name}, was not in stock. stock: {product.Stock}, requested: {orderLine.ToBeReserved}"
                        });
                    }
                }
                catch (ArgumentException e)
                {
                    errors.Add(new BusinessError
                    {
                        Code = "item-not-found",
                        Description = $"Item, {orderLine.ProductId}, does not exists"
                    });
                }
            }
        }
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