
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Models.Dtos
{
    public record UserResponseDto(
        string Id,
        string FirstName,
        string LastName,
        string Nationality,
        string Profession,
        string Email,
        string Usertype,
        List<Rental>? Rentals,
        List<Car>? Cars
    );
}