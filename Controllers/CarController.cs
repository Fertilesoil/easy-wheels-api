using EasyWheelsApi.Data;
using EasyWheelsApi.Mappings.CarMapping;
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController(
        UserManager<User> userManager,
        ICarService service,
        RentalDbContext dbContext
    ) : ControllerBase
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ICarService _service = service;
        private readonly RentalDbContext _dbContext = dbContext;

        [Authorize]
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateCar(string userId, [FromBody] AddCarDto car)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound(new { message = "The user was not found." });

            var createdCar = await _service.CreateCarAsync(car, (Lessor)user!);
            return Ok(createdCar.ToResponse());
        }

        [Authorize]
        [HttpPut("{carId}")]
        public async Task<IActionResult> UpdateCar(Guid carId, [FromBody] AddCarDto updatedCar)
        {
            Car? actualCar = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == carId);

            if (actualCar is null)
                return NotFound(new { description = "No Car was found with the provided Id" });

            await _service.UpdateCarAsync(updatedCar, actualCar!);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{carId}")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            await _service.DeleteCar(carId);
            return Ok();
        }
    }
}
