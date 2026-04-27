namespace CarDealershipAPI.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;

        // 🔥 IMPORTANT FIX (nullable to avoid error)
        public string? Status { get; set; }
    }
}