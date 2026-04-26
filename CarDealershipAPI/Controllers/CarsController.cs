using Microsoft.AspNetCore.Mvc;
using CarDealershipAPI.Data;
using CarDealershipAPI.Models;

namespace CarDealershipAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetCars()
        {
            return Ok(_context.Cars.ToList());
        }

        // ADD
        [HttpPost]
        public IActionResult AddCar(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
            return Ok("Car added");
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult UpdateCar(int id, Car updatedCar)
        {
            var car = _context.Cars.Find(id);
            if (car == null) return NotFound();

             // 🔥 SAFE UPDATE (ONLY IF PROVIDED)
            if (!string.IsNullOrEmpty(updatedCar.Brand))
                car.Brand = updatedCar.Brand;

            if (!string.IsNullOrEmpty(updatedCar.Model))
                car.Model = updatedCar.Model;

            if (updatedCar.Year != 0)
                car.Year = updatedCar.Year;

            if (updatedCar.Price != 0)
                car.Price = updatedCar.Price;

            if (!string.IsNullOrEmpty(updatedCar.Status))
                car.Status = updatedCar.Status;

             // 🔥 THIS IS YOUR TARGET
                car.Stock = updatedCar.Stock;

                _context.SaveChanges();

            return Ok(car);
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult DeleteCar(int id)
        {
            var car = _context.Cars.Find(id);

            if (car == null)
                return NotFound("Car not found");

            _context.Cars.Remove(car);
            _context.SaveChanges();

            return Ok("Car deleted");
        }

        [HttpGet("{id}")]
        public IActionResult GetCar(int id)
        {
            var car = _context.Cars.Find(id);

            if (car == null)
                return NotFound("Car not found");

            return Ok(car);
        }
    }
}