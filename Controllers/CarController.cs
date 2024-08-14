using EasyWheelsApi.Data;
using EasyWheelsApi.Mappings.CarMapping;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using EasyWheelsApi.Validation.CarValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
            Summary = "Criar um novo Carro",
            Description = "A operação insere no banco de dados um novo Carro que é vinculado ao Locador que vem do Id fornecido."
        )]
        [
            SwaggerResponse(200, "Success", typeof(CarResponseDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
            SwaggerResponse(400, "Bad Request")
        ]
        public async Task<IActionResult> CreateCar(string userId, [FromBody] AddCarDto car)
        {
            car.IsValid();
            var user = await _userManager.FindByIdAsync(userId) ?? throw new CustomException(
                    "User not found",
                    "None user was found with the given Id",
                    StatusCodes.Status404NotFound
                );

            var createdCar = await _service.CreateCarAsync(car, (Lessor)user!);
            return Ok(JsonConvert.SerializeObject(createdCar.ToResponse(), Formatting.Indented));
        }

        [Authorize]
        [HttpPut("{carId}")]
        [SwaggerOperation(
            Summary = "Atualizar informações de um Carro",
            Description = "A operação atualiza um Carro já registrado no banco de dados. A busca é baseada no Id do Carro."
        )]
        [
            SwaggerResponse(200, "Success", typeof(SuccessDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
            SwaggerResponse(400, "Bad Request")
        ]
        public async Task<IActionResult> UpdateCar(Guid carId, [FromBody] AddCarDto updatedCar)
        {
            updatedCar.IsValid();
            Car? actualCar = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == carId) ?? throw new CustomException(
                    "Car not found",
                    "No Car was found with the provided Id",
                    StatusCodes.Status404NotFound
                );

            await _service.UpdateCarAsync(updatedCar, actualCar!);
            return Ok(JsonConvert.SerializeObject(new SuccessDto("The car was successfully updated"), Formatting.Indented));
        }

        [Authorize]
        [HttpDelete("{carId}")]
        [SwaggerOperation(
            Summary = "Deletar um Carro",
            Description = "A operação deleta um Carro já registrado do banco de dados. A busca é baseada no Id do Carro e não retornará erros se o Carro não existir."
        )]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            await _service.DeleteCar(carId);
            return Ok();
        }
    }
}
