using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models
{
    public class Sale
    {
        public int id { get; set; }
        public int carId { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public DateTime saleDate { get; set; }
    }
}
