
using EasyWheelsApi.Models.Dtos.ViaCep;

namespace EasyWheelsApi.Facade
{
    public interface IAddressFacade
    {
        Task<AddressDto> CompleteAddressAsync(CepDto cepDto);
    }
}