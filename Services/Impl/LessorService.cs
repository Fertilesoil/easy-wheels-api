using EasyWheelsApi.Facade;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Models.Dtos.ViaCep;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Services.Impl
{
    public class LessorService(UserManager<User> userManager, IAddressFacade addressFacade)
        : ILessorService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IAddressFacade _addressFacade = addressFacade;

        public async Task<Lessor> CreateLessorAsync(AddUserDto lessor, string password)
        {
            if (string.IsNullOrEmpty(lessor.Street) || lessor.Street == "string" || string.IsNullOrEmpty(lessor.City) || lessor.City == "string" || string.IsNullOrEmpty(lessor.State) || lessor.State == "string")
            {
                var cep = new CepDto(lessor.PostalCode);
                var addressDto = await _addressFacade.CompleteAddressAsync(cep);

                if(string.IsNullOrEmpty(addressDto.Bairro) || string.IsNullOrEmpty(addressDto.Logradouro) || string.IsNullOrEmpty(addressDto.Cidade) || string.IsNullOrEmpty(addressDto.Estado)) 
                throw new CustomException(
                        "Address not found",
                        "The given addres code does not return any matches, please write the complete address",
                        StatusCodes.Status404NotFound
                    );

                var newLessor = lessor.ToEntity(addressDto);
                await _userManager.CreateAsync(newLessor, password);
                return newLessor;
            }

            var parsedLessor = lessor.ToEntity();
            await _userManager.CreateAsync(parsedLessor, password);
            return parsedLessor;
        }

        public async Task<bool> DeleteLessor(string id)
        {
            var lessor = await _userManager.FindByIdAsync(id);
            if (lessor == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(lessor);
            return result.Succeeded;
        }

        public IQueryable<UserResponseDto> GetAllLessorsAsync()
        {
            return _userManager
                .Users.OfType<Lessor>()
                .Include(c => c.Cars)
                .ThenInclude(r => r.Rentals)
                .AsSplitQuery()
                .AsQueryable()
                .Select(l => new UserResponseDto(
                    l.Id,
                    l.FirstName,
                    l.LastName,
                    l.Nationality,
                    l.Profession,
                    l.Email!,
                    l.UserType,
                    l.Cars.SelectMany(c => c.Rentals!)
                        .Select(r => new RentalInfo(r.Id, r.StartDate, r.EndDate, r.TotalPrice))
                        .ToList(),
                    l.Cars.Select(c => new CarResponseDto(
                            c.Id,
                            c.Brand,
                            c.Model,
                            c.PricePerDay,
                            c.IsRented,
                            c.LessorId
                        ))
                        .ToList()
                ));
        }

        public async Task<Lessor> GetLessorByidAsync(string id)
        {
            var lessor = await _userManager
                .Users.OfType<Lessor>()
                .Include(c => c.Cars)
                .ThenInclude(r => r.Rentals)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id == id);
            return lessor!;
        }

        public async Task UpdateLessorAsync(Lessor updatedLessor, Lessor actualUser)
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
