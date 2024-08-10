
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface ILessorService
    {
        IQueryable<UserResponseDto> GetAllLessorsAsync();
        Task<Lessor> GetLessorByidAsync(string id);
        Task UpdateLessorAsync(Lessor updatedLessor, Lessor actualUser);
        Task<Lessor> CreateLessorAsync(AddUserDto lessor, string password);
        Task<bool> DeleteLessor(string id);
    }
}