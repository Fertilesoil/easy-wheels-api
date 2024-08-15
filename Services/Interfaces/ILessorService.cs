
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface ILessorService
    {
        IQueryable<UserResponseDto> GetAllLessorsAsync();
        Task<Lessor> GetLessorByidAsync(string id);
        Task UpdateLessorAsync(Lessor updatedLessor, Lessor actualUser);
        Task<Lessor> CreateLessorAsync(AddUserDto lessor, string password);
        Task<bool> DeleteLessor(string id);
        Task<Lessor> GetLessorByEmail(string email);
    }
}