
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface ILesseeService
    {
        IQueryable<LesseeResponseDto> GetAllLesseesAsync();
        Task<Lessee> GetLesseeByidAsync(string id);
        Task<Lessee> GetLesseeByEmailAsync(string Email);
        Task UpdateLesseeAsync(Lessee updatedLessor, Lessee actualUser);
        Task<Lessee> CreateLesseeAsync(AddUserDto lessee, string password);
        Task<bool> DeleteLessee(string id);
    }
}