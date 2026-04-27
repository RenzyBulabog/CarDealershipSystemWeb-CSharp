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
                // 🔥 ALWAYS SET DATE
                sale.SaleDate = DateTime.Now;

                // 🔥 CUSTOMER REQUEST → ALWAYS PENDING
                if (sale.Status != "Approved")
                {
                    sale.Status = "Pending";
                }

                // 🔥 ADMIN DIRECT SALE → APPROVED
                if (sale.Status == "Approved")
                {
                    var car = _context.Cars.FirstOrDefault(c => c.Id == sale.CarId);

                    if (car == null)
                        return NotFound("Car not found");

                    if (car.Stock <= 0)
                        return BadRequest("Out of stock");

                    car.Stock--;

                    if (car.Stock == 0)
                        car.Status = "Sold";
                }

                _context.Sales.Add(sale);
                _context.SaveChanges();

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSale(int id, Sale updated)
        {
            var sale = _context.Sales.FirstOrDefault(s => s.Id == id);

            if (sale == null)
                return NotFound("Sale not found");

            // 🔥 PREVENT DOUBLE APPROVE
            if (sale.Status == "Approved")
                return BadRequest("Already approved");

            sale.Status = updated.Status;

            if (updated.Status == "Approved")
            {
                var car = _context.Cars.FirstOrDefault(c => c.Id == sale.CarId);

                if (car == null)
                    return NotFound("Car not found");

                if (car.Stock <= 0)
                    return BadRequest("Out of stock");

                car.Stock--;

                if (car.Stock == 0)
                    car.Status = "Sold";

                sale.SaleDate = DateTime.Now;
            }

            _context.SaveChanges();

            return Ok(sale);
        }
    }
}