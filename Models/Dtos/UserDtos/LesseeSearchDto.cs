using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record LesseeSearchDto(
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
        List<Rental> Rentals
    );
}
