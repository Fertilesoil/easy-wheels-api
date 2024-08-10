using EasyWheelsApi.Facade;
using EasyWheelsApi.Models.Dtos.ViaCep;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CepController(IAddressFacade viaCepService) : ControllerBase
    {
        private readonly IAddressFacade _viaCepService = viaCepService;

        [Authorize]
        [HttpPost("{cep}")]
        public async Task<IActionResult> GetAddress(string cep)
        {
            var cepResult = new CepDto(cep);
            var addressDto = await _viaCepService.CompleteAddressAsync(cepResult);
            return Ok(addressDto);
        }
    }
}