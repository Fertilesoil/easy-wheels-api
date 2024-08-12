using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessorController(ILessorService service) : ControllerBase
    {
        private readonly ILessorService _service = service;

        // private record

        [Authorize]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar todos os Locadores",
            Description = "A operação retorna uma lista de Locadores cadastrados ou uma lista vazia."
        )]
        [
            SwaggerResponse(200, "Success", typeof(List<UserResponseDto>)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> GetAllLessors()
        {
            var lessors = _service.GetAllLessorsAsync();

            return Ok(
                JsonConvert.SerializeObject(await lessors.ToListAsync(), Formatting.Indented)
            );
        }

        [Authorize]
        [HttpGet("{id}", Name = "LessorPerId")]
        [SwaggerOperation(
            Summary = "Buscar um Locador por Id",
            Description = "A operação retorna um Locador cadastrado."
        )]
        [
            SwaggerResponse(200, "Success", typeof(UserSearchDto)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> GetLessorById(string id)
        {
            var lessor = await _service.GetLessorByidAsync(id) ?? throw new CustomException (
                    "Lessor not found",
                    "None lessor was found with those parameters",
                    StatusCodes.Status404NotFound
                );
            return Ok(JsonConvert.SerializeObject(lessor.ToSearchResponse(), Formatting.Indented));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar um novo Locador",
            Description = "A operação insere no banco de dados um novo Locador não incluindo ainda seus carros ou contratos, apenas informações pessoais. A propriedade username, a primeira do objeto, deve sempre ser preenchida com letras minúsculas e sem espaços."
        )]
        [
            SwaggerResponse(200, "Success", typeof(UserResponseDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> CreateLessor([FromBody] AddUserDto lessor)
        {
            var result = await _service.CreateLessorAsync(lessor, lessor.Password);
            return Ok(JsonConvert.SerializeObject(lessor.ToResponse(result), Formatting.Indented));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessor(
            string id,
            [FromBody] UpdateUserDto updatedLessor
        )
        {
            var actualUser = await _service.GetLessorByidAsync(id);

            if (actualUser is null)
                return NotFound();

            await _service.UpdateLessorAsync(updatedLessor.ToEntity(actualUser), actualUser);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessor(string id)
        {
            await _service.DeleteLessor(id);
            return Ok();
        }
    }
}
