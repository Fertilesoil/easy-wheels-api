using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LesseeController(ILesseeService service) : ControllerBase
    {
        private readonly ILesseeService _service = service;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllLessess()
        {
            var lessess = _service.GetAllLesseesAsync();
            return Ok(await lessess.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesseeById(string id)
        {
            var lessee = await _service.GetLesseeByidAsync(id);

            if (lessee is null)
                return NotFound();

            return Ok(lessee.ToSearchResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessee([FromBody] AddUserDto lessee)
        {
            var result = await _service.CreateLesseeAsync(lessee, lessee.Password);
            return Ok(lessee.ToResponseLessee(result));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessee(
            string id,
            [FromBody] UpdateUserDto updatedLessee
        )
        {
            var actualUser = await _service.GetLesseeByidAsync(id);

            if (actualUser is null)
                return NotFound();

            await _service.UpdateLesseeAsync(updatedLessee.ToEntityLessee(actualUser), actualUser);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessee(string id)
        {
            await  _service.DeleteLessee(id);
            return Ok();
        }
    }
}