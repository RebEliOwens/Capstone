namespace PRSCapstoneAPI.Models
{
    public class Poline
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }

        public Poline(string product, int quantity, decimal price, decimal lineTotal) 
        {
            Product = product;
            Quantity = quantity;
            Price = price;
            LineTotal = lineTotal;
        }
        public Poline() { }
    }
 
}
