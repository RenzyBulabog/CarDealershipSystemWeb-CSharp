using Microsoft.AspNetCore.Mvc;
using CarDealershipAPI.Data;
using CarDealershipAPI.Models;
using System.Linq;

namespace CarDealershipAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 GET ALL SALES
        [HttpGet]
        public IActionResult GetSales()
        {
            return Ok(_context.Sales.ToList());
        }

        // 🔥 ADD SALE + AUTO STOCK DEDUCT
        [HttpPost]
        public IActionResult AddSale([FromBody] Sale sale)
        {
            try
            {
                var car = _context.Cars.FirstOrDefault(c => c.Id == sale.CarId);

                if (car == null)
                    return NotFound("Car not found");

                if (car.Stock <= 0)
                    return BadRequest("Out of stock");

                // 💣 DEDUCT STOCK
                car.Stock--;

                // 💣 AUTO SOLD
                if (car.Stock == 0)
                    car.Status = "Sold";

                // 💣 ADD SALE DATE
                sale.SaleDate = DateTime.Now;

                _context.Sales.Add(sale);
                _context.SaveChanges();

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}