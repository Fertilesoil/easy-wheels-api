using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EasyWheelsApi.Models.Entities
{
    public abstract class User : IdentityUser
    {
        [MaxLength(20)]
        public required string FirstName { get; set; }
        [StringLength(30), MaxLength(50)]
        public required string LastName { get; set; }
        [StringLength(30), MaxLength(20)]
        public required string Nationality { get; set; }
        [StringLength(30), MaxLength(30)]
        public required string Profession { get; set; }
        [StringLength(9), MaxLength(9)]
        public required string Rg { get; set; }
        [StringLength(11), MaxLength(11)]
        public required string Cpf { get; set; }
        [StringLength(30), MaxLength(30)]
        public string? City { get; set; }
        [StringLength(2), MaxLength(2)]
        public string? State { get; set; }
        
        [StringLength(100), MaxLength(100)]
        public string? Street { get; set; }
        public required int HouseNumber { get; set; }
        [StringLength(8), MaxLength(8)]
        public required string PostalCode { get; set; }
        public required string UserType { get; set; }
    }
}
