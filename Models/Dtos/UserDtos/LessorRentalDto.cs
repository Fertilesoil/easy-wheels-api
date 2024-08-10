
namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record LessorRentalDto(
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
        string PostalCode
    );
}