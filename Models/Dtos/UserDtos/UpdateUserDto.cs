using System.ComponentModel.DataAnnotations;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record UpdateUserDto(
        string FirstName,
        string LastName,
        string Nationality,
        string Profession,
        [Required] [EmailAddress] string Email,
        string City,
        string State,
        string Street,
        int HouseNumber,
        string PostalCode
    );
}