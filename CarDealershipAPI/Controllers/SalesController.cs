using Microsoft.AspNetCore.Mvc;
using CarDealershipAPI.Data;
using CarDealershipAPI.Models;

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

        [HttpGet]
        public IActionResult GetSales()
        {
            return Ok(_context.Sales.ToList());
        }

        [HttpPost]
        public IActionResult AddSale(Sale sale)
        {
            _context.Sales.Add(sale);
            _context.SaveChanges();
            return Ok("Sale added");
        }
    }
}