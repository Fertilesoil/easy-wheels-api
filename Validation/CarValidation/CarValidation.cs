using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Validation.CarValidation
{
    public static class CarValidation
    {
        public static void IsValid(this AddCarDto car)
        {
            if (string.IsNullOrEmpty(car.Brand) || car.Brand == "string")
                throw new CustomException(
                    "Missing field",
                    "The field Brand cannot not be empty",
                    StatusCodes.Status400BadRequest
                );

            if (string.IsNullOrEmpty(car.Model) || car.Brand == "string")
                throw new CustomException(
                    "Missing field",
                    "The field Model cannot not be empty",
                    StatusCodes.Status400BadRequest
                );

            if (car.PricePerDay <= 0)
                throw new CustomException(
                    "Missing field",
                    "The field PricePerDay cannot not be empty or 0",
                    StatusCodes.Status400BadRequest
                );
        }
    }
}