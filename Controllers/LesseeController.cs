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
    public class LesseeController(ILesseeService service) : ControllerBase
    {
        private readonly ILesseeService _service = service;
        private sealed record SuccessDto(string Message);

        [Authorize]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar todos os Locatários",
            Description = "A operação retorna uma lista de Locatários cadastrados ou uma lista vazia."
        )]
        [
            SwaggerResponse(200, "Success", typeof(List<LesseeResponseDto>)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> GetAllLessess()
        {
            var lessess = _service.GetAllLesseesAsync();
            return Ok(
                JsonConvert.SerializeObject(await lessess.ToListAsync(), Formatting.Indented)
            );
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Buscar um Locatário por Id",
            Description = "A operação retorna um Locatário cadastrado."
        )]
        [
            SwaggerResponse(200, "Success", typeof(LesseeSearchDto)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> GetLesseeById(string id)
        {
            var lessee =
                await _service.GetLesseeByidAsync(id)
                ?? throw new CustomException(
                    "No Lessee found",
                    "None Lessee was found with those parameters",
                    StatusCodes.Status404NotFound
                );
            return Ok(JsonConvert.SerializeObject(lessee.ToSearchResponse(), Formatting.Indented));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar um novo Locatário",
            Description = "A operação insere no banco de dados um novo Locatário não incluindo ainda seus contratos, apenas informações pessoais. A propriedade username, a primeira do objeto, deve sempre ser preenchida com letras minúsculas e sem espaços."
        )]
        [
            SwaggerResponse(200, "Success", typeof(LesseeResponseDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> CreateLessee([FromBody] AddUserDto lessee)
        {
            var result = await _service.CreateLesseeAsync(lessee, lessee.Password);
            return Ok(
                JsonConvert.SerializeObject(lessee.ToResponseLessee(result), Formatting.Indented)
            );
        }

        [Authorize]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualizar informações de um Locatário",
            Description = "A operação atualiza um Locatário já registrado no banco de dados. A busca é baseada no Id do Locatário."
        )]
        [
            SwaggerResponse(200, "Success", typeof(SuccessDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> UpdateLessee(
            string id,
            [FromBody] UpdateUserDto updatedLessee
        )
        {
            var actualUser =
                await _service.GetLesseeByidAsync(id)
                ?? throw new CustomException(
                    "No Lessee found",
                    "None Lessee was found with those parameters",
                    StatusCodes.Status404NotFound
                );
            await _service.UpdateLesseeAsync(updatedLessee.ToEntityLessee(actualUser), actualUser);
            return Ok(JsonConvert.SerializeObject(new SuccessDto("The Lessee was successfully updated"), Formatting.Indented));
        }

        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletar um Locatário",
            Description = "A operação deleta um Locatário já registrado do banco de dados. A busca é baseada no Id do Locatário e não retornará erros se o Locatário não existir."
        )]
        [SwaggerResponse(200, "Success", typeof(UserResponseDto))]
        public async Task<IActionResult> DeleteLessee(string id)
        {
            await _service.DeleteLessee(id);
            return Ok();
        }
    }
}
