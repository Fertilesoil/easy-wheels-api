using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Mappings.CarMapping
{
    public static class CarMapping
    {
        public static CarResponseDto ToResponse(this Car car)
        {
            return new(car.Id, car.Brand, car.Model, car.PricePerDay, car.IsRented, car.LessorId);
        }
    }
}
