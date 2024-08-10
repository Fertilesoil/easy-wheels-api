using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface IRentalService
    {
        Task<Rental> CreateRentalAsync(AddRentalDto rental);
        Task<RentalResponseDto> GetRentalByIdAsync(Guid id);
    }
}