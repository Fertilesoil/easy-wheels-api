using EasyWheelsApi.Facade;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Models.Dtos.ViaCep;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Services.Impl
{
    public class LesseeService(UserManager<User> userManager, IAddressFacade addressFacade)
        : ILesseeService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IAddressFacade _addressFacade = addressFacade;

        public async Task<Lessee> CreateLesseeAsync(AddUserDto lessee, string password)
        {
            var cep = new CepDto(lessee.PostalCode);
            var addressDto = await _addressFacade.CompleteAddressAsync(cep);

            var newLessee = lessee.ToEntityLessee(addressDto);
            await _userManager.CreateAsync(newLessee, password);
            return newLessee!;
        }

        public async Task<bool> DeleteLessee(string id)
        {
            var lessee = await _userManager.FindByIdAsync(id);
            
            if (lessee == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(lessee);
            return result.Succeeded;
        }

        public IQueryable<LesseeResponseDto> GetAllLesseesAsync()
        {
            return _userManager
                .Users
                .OfType<Lessee>()
                .Include(r => r.Rentals)
                .Select(l => l.ToResponseLessee());
        }

        public async Task<Lessee> GetLesseeByidAsync(string id)
        {
            var lessee = await _userManager.Users
                .OfType<Lessee>()
                .Include(r => r.Rentals)
                .FirstOrDefaultAsync(l => l.Id == id);
            return lessee!;
        }

        public async Task UpdateLesseeAsync(Lessee updatedLessor, Lessee actualUser)
        {
            actualUser.FirstName = updatedLessor.FirstName;
            actualUser.LastName = updatedLessor.LastName;
            actualUser.Nationality = updatedLessor.Nationality;
            actualUser.Profession = updatedLessor.Profession;
            actualUser.Email = updatedLessor.Email;
            actualUser.City = updatedLessor.City;
            actualUser.State = updatedLessor.State;
            actualUser.Street = updatedLessor.Street;
            actualUser.HouseNumber = updatedLessor.HouseNumber;
            actualUser.PostalCode = updatedLessor.PostalCode;

            await _userManager.UpdateAsync(actualUser);
        }
    }
}
