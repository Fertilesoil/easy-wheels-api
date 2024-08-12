using System.ComponentModel.DataAnnotations;

namespace EasyWheelsApi.Models.Dtos.UserDtos
{
    public record UserLoginDto([Required] string Email, [Required] string Password);
}
