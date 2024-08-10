using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface ILesseeService
    {
        IQueryable<LesseeResponseDto> GetAllLesseesAsync();
        Task<Lessee> GetLesseeByidAsync(string id);
        Task UpdateLesseeAsync(Lessee updatedLessor, Lessee actualUser);
        Task<Lessee> CreateLesseeAsync(AddUserDto lessee, string password);
        Task<bool> DeleteLessee(string id);
    }
}