using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWheelsApi.Models.Dtos
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