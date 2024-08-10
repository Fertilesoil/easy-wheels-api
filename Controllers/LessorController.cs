using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessorController(ILessorService service) : ControllerBase
    {
        private readonly ILessorService _service = service;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllLessors()
        {
            var lessors = _service.GetAllLessorsAsync();
            return Ok(await lessors.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id}", Name = "LessorPerId")]
        public async Task<IActionResult> GetLessorById(string id)
        {
            var lessor = await _service.GetLessorByidAsync(id);

            if (lessor is null)
                return NotFound();

            return Ok(lessor.ToSearchResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessor([FromBody] AddUserDto lessor)
        {
            var result = await _service.CreateLessorAsync(lessor, lessor.Password);
            return Ok(lessor.ToResponse(result));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessor(string id, [FromBody] UpdateUserDto updatedLessor)
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
