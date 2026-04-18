namespace CarDealershipAPI.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
    }
}