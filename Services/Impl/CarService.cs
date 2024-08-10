using EasyWheelsApi.Data;
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Services.Impl
{
    public class CarService(RentalDbContext dbContext) : ICarService
    {
        private readonly RentalDbContext _dbContext = dbContext;

        public async Task<Car> CreateCarAsync(AddCarDto car, Lessor lessor)
        {
            Car createdCar =
                new()
                {
                    Brand = car.Brand,
                    Model = car.Model,
                    PricePerDay = car.PricePerDay,
                    IsRented = car.IsRented,
                    LessorId = lessor.Id
                };

            lessor.Cars.Add(createdCar);
            await _dbContext.Cars.AddAsync(createdCar);
            await _dbContext.SaveChangesAsync();
            return createdCar;
        }

        public async Task<bool> DeleteCar(Guid carId)
        {
            var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null)
                return false;

            _dbContext.Cars.Remove(car);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task UpdateCarAsync(AddCarDto updatedCar, Car actualCar)
        {
            actualCar.Brand = updatedCar.Brand;
            actualCar.Model = updatedCar.Model;
            actualCar.PricePerDay = updatedCar.PricePerDay;
            actualCar.IsRented = updatedCar.IsRented;

            await _dbContext.SaveChangesAsync();
        }
    }
}
