using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record LesseeResponseDto(
        string Id,
        string FirstName,
        string LastName,
        string Nationality,
        string Profession,
        string Email,
        string Usertype,
        List<Rental>? Rentals
    );
}