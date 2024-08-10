using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWheelsApi.Models.Dtos
{
    public record UserLoginDto([Required] string Email, [Required] string Password);
}
