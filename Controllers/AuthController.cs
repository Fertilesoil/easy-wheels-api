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
        private const string REFRESH = "refreshtoken";

        private sealed record TokenDto(string AccessToken);

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

            var result = await _signInManager.PasswordSignInAsync(
                userFound!,
                user.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                throw new CustomException(
                    "Password issue",
                    "The passwords don't match",
                    StatusCodes.Status401Unauthorized
                );

            TokenConfiguration token = new(_configuration);

            var accessToken = token.GenerateJwtToken(userFound!);
            var refreshToken = token.GenerateRefreshToken(userFound!);

            Response.Cookies.Delete(REFRESH);

            Response.Cookies.Append(
                REFRESH,
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(3),
                    SameSite = SameSiteMode.None
                }
            );

            await _userManager.UpdateAsync(userFound!);
            return Ok(
                JsonConvert.SerializeObject(
                    new TokenDto("Bearer " + accessToken),
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

            var refreshToken = Request.Cookies[REFRESH];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Ok(
                    JsonConvert.SerializeObject(
                        new { Message = "Logout successful, no refresh token found." },
                        Formatting.Indented
                    )
                );
            }

            await _signInManager.SignOutAsync();
            Response.Cookies.Delete(REFRESH);

            return Ok(
                JsonConvert.SerializeObject(
                    new { Message = "Logout successful." },
                    Formatting.Indented
                )
            );
        }

        [HttpGet("refresh-token")]
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
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue(REFRESH, out var refreshToken))
                throw new CustomException(
                    "Missing refresh token",
                    "No valid refresh tokens were found, please try log in again",
                    StatusCodes.Status401Unauthorized
                );

            ClaimsPrincipal principal;
            TokenConfiguration token = new(_configuration);

            principal =
                token.GetPrincipalFromExpiredToken(refreshToken)
                ?? throw new CustomException(
                    "Invalid token",
                    "The provided refresh token is invalid",
                    StatusCodes.Status401Unauthorized
                );

            var userEmail = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userEmail == null)
                throw new CustomException(
                    "Invalid token",
                    "The provided token does not contain a valid user ID",
                    StatusCodes.Status401Unauthorized
                );

            var userFound =
                await _userManager.FindByEmailAsync(userEmail)
                ?? throw new CustomException(
                    "No user found",
                    "No such user was found with those parameters",
                    StatusCodes.Status404NotFound
                );

            var newAccessToken = token.GenerateJwtToken(userFound);

            return Ok(
                JsonConvert.SerializeObject(
                    new { AccessToken = "Bearer " + newAccessToken },
                    Formatting.Indented
                )
            );
        }
    }
}