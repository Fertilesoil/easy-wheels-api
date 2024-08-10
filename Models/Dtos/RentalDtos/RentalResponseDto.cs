using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.UserDtos;

namespace EasyWheelsApi.Models.Dtos.RentalDtos
{
    public record RentalResponseDto(
        RentalInfo rentalInfo,
        LessorRentalDto LessorInfo,
        LessorRentalDto LesseeInfo,
        CarRentalDto CarInfo
    );
}