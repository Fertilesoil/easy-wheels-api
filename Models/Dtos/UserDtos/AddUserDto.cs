using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWheelsApi.Models.Dtos
{
    public record AddUserDto(
        string Username,
        [Required] [MaxLength(20)] string FirstName,
        [Required] [MaxLength(50)] string LastName,
        [Required] [MaxLength(20)] string Nationality,
        [Required] [MaxLength(30)] string Profession,
        [Required] [MaxLength(30)] [EmailAddress] string Email,
        [Required] string Password,
        [Required] [MaxLength(9)] string Rg,
        [Required] [MaxLength(11)] string Cpf,
        [Required] int HouseNumber,
        [Required] [MaxLength(8)] string PostalCode
    );
}
