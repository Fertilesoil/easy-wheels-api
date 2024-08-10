using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Services.Interfaces
{
    public interface ICarService
    {
        Task UpdateCarAsync(AddCarDto updatedCar, Car actualCar);
        Task<Car> CreateCarAsync(AddCarDto car, Lessor lessor);
        Task<bool> DeleteCar(Guid carId);
    }
}