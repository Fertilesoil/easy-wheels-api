using System.Security.Claims;
using EasyWheelsApi.Configuration;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration
    ) : ControllerBase
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;

        public sealed record TokenDto(string Token, string Email, string UserType);

        private sealed record TokenAndRefreshDto(string AccessToken, string RefreshToken);

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login de usuários",
            Description = "Endpoint para login de usuários. A operação retorna um token JWT válido para liberar o consumo dos recursos da aplicação."
        )]
        [
            SwaggerResponse(200, "Success", typeof(TokenDto)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto)),
        ]
        public async Task<IActionResult> Login([FromBody] UserLoginDto user)
        {
            var userFound =
                await _userManager.FindByEmailAsync(user.Email)
                ?? throw new CustomException(
                    "User not found",
                    "No such user was found with those parameters",
                    StatusCodes.Status404NotFound
                );

            var result = await _signInManager.CheckPasswordSignInAsync(
                userFound,
                user.Password,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                throw new CustomException(
                    "Password issue",
                    "The passwords don't match",
                    StatusCodes.Status401Unauthorized
                );
            }

            var tokenService = new TokenConfiguration(_configuration);
            var accessToken = tokenService.GenerateJwtToken(userFound);
            var refreshToken = tokenService.GenerateRefreshToken(userFound);

            await _userManager.UpdateAsync(userFound);

            return Ok(
                JsonConvert.SerializeObject(
                    new TokenAndRefreshDto("Bearer " + accessToken, "Bearer " + refreshToken),
                    Formatting.Indented
                )
            );
        }

        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Logout de usuários",
            Description = "Endpoint para deslogar usuários. A operação elimina o cookie que foi fornecido no login."
        )]
        [
            SwaggerResponse(200, "Success", typeof(SuccessDto)),
            SwaggerResponse(400, "Bad Request", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto))
        ]
        public async Task<IActionResult> Logout([FromBody] EmailDto email)
        {
            if (string.IsNullOrEmpty(email.Email))
            {
                throw new CustomException(
                    "Missing info",
                    "Email is required for logout verification.",
                    StatusCodes.Status400BadRequest
                );
            }

            await _signInManager.SignOutAsync();

            return Ok(
                JsonConvert.SerializeObject(
                    new { Message = "Logout successful." },
                    Formatting.Indented
                )
            );
        }

        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Refresh Token",
            Description = "Endpoint para renovar autenticação. A operação retorna um novo token JWT para continuar consumindo os recursos da aplicação."
        )]
        [
            SwaggerResponse(200, "Success", typeof(SuccessDto)),
            SwaggerResponse(401, "Unauthorized", typeof(CustomExceptionDto)),
            SwaggerResponse(404, "Not Found", typeof(CustomExceptionDto)),
            SwaggerResponse(500, "Internal Error", typeof(CustomExceptionDto))
        ]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto receivedToken)
        {
            if (receivedToken == null)
                throw new CustomException(
                    "Missing token",
                    "No token was senti with the request, please insert a valid token",
                    StatusCodes.Status400BadRequest
                );

            var tokenService = new TokenConfiguration(_configuration);

            try
            {
                var principal = tokenService.ValidateRefreshToken(receivedToken.Token!);

                var userEmail = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                    throw new CustomException(
                        "Invalid token",
                        "Invalid claims in token",
                        StatusCodes.Status401Unauthorized
                    );

                var userFound =
                    await _userManager.FindByEmailAsync(userEmail!)
                    ?? throw new CustomException(
                        "No user found",
                        "No such user was found with those parameters",
                        StatusCodes.Status404NotFound
                    );

                var newAccessToken = tokenService.GenerateJwtToken(userFound);

                return Ok(
                    JsonConvert.SerializeObject(
                        new TokenDto("Bearer " + newAccessToken, userFound.Email!, userFound.UserType),
                        Formatting.Indented
                    )
                );
            }
            catch (CustomException exp)
            {
                throw new CustomException(exp.Title, exp.Message, exp.StatusCode);
            }
        }
    }
}
