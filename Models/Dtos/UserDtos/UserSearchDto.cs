
using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.RentalDtos;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record UserSearchDto(
        string Id,
        string FirstName,
        string LastName,
        string Nationality,
        string Profession,
        string Email,
        string City,
        string State,
        string Street,
        int HouseNumber,
        string PostalCode,
        string UserType,
        List<RentalInfo>? Rentals,
        List<CarResponseDto>? Cars
    );
}