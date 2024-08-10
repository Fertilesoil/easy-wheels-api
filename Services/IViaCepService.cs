
using EasyWheelsApi.Models.Dtos.ViaCep;

namespace EasyWheelsApi.Services
{
    public interface IViaCepService
    {
        Task<AddressDto> GetAddressByCepAsync(string cep);
    }
}
