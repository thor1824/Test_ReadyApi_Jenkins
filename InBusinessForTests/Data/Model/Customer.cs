using System.Collections.Generic;

namespace InBusinessForTests.Data.Model
{
    public class Customer: Entity
    {
        public string Name { get; set; }
        public double Credit { get; set; }
        public List<Order> Orders { get; } = new List<Order>();
    }
}