namespace InBusinessForTests.Data.Model
{
    public class Product: Entity
    {
        public string Name { get; set; } = "N/A";
        public int Stock { get; set; } 
        public int Reserved { get; set; }
        public double Price { get; set; } = 100.0;
        public bool HasDiscount => DiscountInPercentage != 0.0;
        public double DiscountInPercentage { get; set; } = 0.0;
        public int AmountBoughtForDiscount { get; set; } = 0;
    }
}