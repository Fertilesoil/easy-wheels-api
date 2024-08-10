using EasyWheelsApi.Models.Dtos.ViaCep;
using EasyWheelsApi.Services;

namespace EasyWheelsApi.Facade
{
    public class AddressFacade : IAddressFacade
    {
        private readonly IViaCepService _viaCepService;

        public AddressFacade(IViaCepService viaCepService)
        {
            _viaCepService = viaCepService;
        }

        public async Task<AddressDto> CompleteAddressAsync(CepDto cepDto)
        {
            var addressDto = await _viaCepService.GetAddressByCepAsync(cepDto.Cep);
            return addressDto;
        }
    }
}
