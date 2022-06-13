namespace PRSCapstoneAPI.Models
{
    public class Poline
    {
        public string Product { get; set; }
        public string PartNbr { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }

        public Poline(string product, string partNbr, int quantity, decimal price, decimal lineTotal) 
        {
            Product = product;
            PartNbr = partNbr;
            Quantity = quantity;
            Price = price;
            LineTotal = lineTotal;
        }
        public Poline() { }
    }
 
}
