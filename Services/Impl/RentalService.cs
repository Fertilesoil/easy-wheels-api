using EasyWheelsApi.Data;
using EasyWheelsApi.Mappings.RentalMapping;
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Dtos.UserMapping;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Services.Impl
{
    public class RentalService(RentalDbContext dbContext) : IRentalService
    {
        private readonly RentalDbContext _dbContext = dbContext;

        public async Task<Rental> CreateRentalAsync(AddRentalDto rental)
        {
            var newRental = await _dbContext.Rentals.AddAsync(rental.ToEntity());
            await _dbContext.SaveChangesAsync();
            return newRental.Entity;
        }

        public async Task<RentalResponseDto> GetRentalByIdAsync(Guid id)
        {
            var rental = await _dbContext
                .Rentals.Select(r => new
                {
                    r.Id,
                    RentalInfo = new
                    {
                        r.Id,
                        r.StartDate,
                        r.EndDate,
                        r.TotalPrice
                    },
                    LessorInfo = new
                    {
                        r.Lessor.Id,
                        r.Lessor.FirstName,
                        r.Lessor.LastName,
                        r.Lessor.Nationality,
                        r.Lessor.Profession,
                        r.Lessor.Email,
                        r.Lessor.City,
                        r.Lessor.State,
                        r.Lessor.Street,
                        r.Lessor.HouseNumber,
                        r.Lessor.PostalCode
                    },
                    LesseeInfo = new
                    {
                        r.Lessee.Id,
                        r.Lessee.FirstName,
                        r.Lessee.LastName,
                        r.Lessee.Nationality,
                        r.Lessee.Profession,
                        r.Lessee.Email,
                        r.Lessee.City,
                        r.Lessee.State,
                        r.Lessee.Street,
                        r.Lessee.HouseNumber,
                        r.Lessee.PostalCode
                    },
                    CarInfo = new
                    {
                        r.Car.Id,
                        r.Car.Brand,
                        r.Car.Model,
                        r.Car.PricePerDay,
                        r.Car.IsRented
                    }
                })
                .FirstOrDefaultAsync(r => r.Id == id);
            // RentalResponseDto newRental = new RentalResponseDto();
            var rentalInfo = new RentalInfo(
                rental.RentalInfo.Id,
                rental.RentalInfo.StartDate,
                rental.RentalInfo.EndDate,
                rental.RentalInfo.TotalPrice
            );

            var lessorInfo = new LessorRentalDto(
                        rental.LessorInfo.Id,
                        rental.LessorInfo.FirstName,
                        rental.LessorInfo.LastName,
                        rental.LessorInfo.Nationality,
                        rental.LessorInfo.Profession,
                        rental.LessorInfo.Email,
                        rental.LessorInfo.City,
                        rental.LessorInfo.State,
                        rental.LessorInfo.Street,
                        rental.LessorInfo.HouseNumber,
                        rental.LessorInfo.PostalCode
            );

            var lesseeInfo = new LessorRentalDto(
                        rental.LesseeInfo.Id,
                        rental.LesseeInfo.FirstName,
                        rental.LesseeInfo.LastName,
                        rental.LesseeInfo.Nationality,
                        rental.LesseeInfo.Profession,
                        rental.LesseeInfo.Email,
                        rental.LesseeInfo.City,
                        rental.LesseeInfo.State,
                        rental.LesseeInfo.Street,
                        rental.LesseeInfo.HouseNumber,
                        rental.LesseeInfo.PostalCode
            );

            var carInfo = new CarRentalDto(
                        rental.CarInfo.Id,
                        rental.CarInfo.Brand,
                        rental.CarInfo.Model,
                        rental.CarInfo.PricePerDay,
                        rental.CarInfo.IsRented
            );
            
            return new(
                rentalInfo,
                lessorInfo,
                lesseeInfo,
                carInfo
            );
        }
    }
}
