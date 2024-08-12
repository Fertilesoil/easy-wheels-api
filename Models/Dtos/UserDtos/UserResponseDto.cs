
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.RentalDtos;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record UserResponseDto(
        string Id,
        string FirstName,
        string LastName,
        string Nationality,
        string Profession,
        string Email,
        string Usertype,
        List<RentalInfo>? Rentals,
        List<CarResponseDto>? Cars
    );
}