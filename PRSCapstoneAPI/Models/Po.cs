namespace PRSCapstoneAPI.Models
{
    public class Po
    {
        public Vendor? Vendor { get; set; } = null!;
        public IEnumerable<Poline> Polines { get; set; } = null!;
        public decimal PoTotal { get; set; }
    }
}
