using System.Security.Claims;
using EasyWheelsApi.Configuration;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFound.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFound.Email!)
                }
            );

            var tokenService = new TokenConfiguration(_configuration);
            var accessToken = tokenService.GenerateJwtToken(userFound);
            var refreshToken = tokenService.GenerateRefreshToken(userFound);

            var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                "E. America/Sao_Paulo"
            );

            // Obter a hora atual no fuso horário do Brasil
            var brazilTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

            // Adiciona o Access Token como um cookie HTTP-Only
            Response.Cookies.Append(
                "AccessToken",
                accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Deve ser true em produção
                    Expires = brazilTime.AddMinutes(5),
                    SameSite = SameSiteMode.None
                }
            );

            // Adiciona o Refresh Token como um cookie HTTP-Only
            Response.Cookies.Append(
                "RefreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Deve ser true em produção
                    Expires = brazilTime.AddDays(3),
                    SameSite = SameSiteMode.None
                }
            );

            // return Ok(new { Message = "Login successful" });

            // var userFound =
            //     await _userManager.FindByEmailAsync(user.Email)
            //     ?? throw new CustomException(
            //         "User not found",
            //         "No such user was found with those parameters",
            //         StatusCodes.Status404NotFound
            //     );

            // var result = await _signInManager.PasswordSignInAsync(
            //     userFound!,
            //     user.Password,
            //     isPersistent: false,
            //     lockoutOnFailure: false
            // );

            // if (!result.Succeeded)
            //     throw new CustomException(
            //         "Password issue",
            //         "The passwords don't match",
            //         StatusCodes.Status401Unauthorized
            //     );

            // TokenConfiguration token = new(_configuration);

            // var accessToken = token.GenerateJwtToken(userFound!);
            // var refreshToken = token.GenerateRefreshToken(userFound!);

            // await _signInManager.SignInAsync(userFound, isPersistent: true);

            // Response.Cookies.Append(
            //     REFRESH,
            //     refreshToken,
            //     new CookieOptions
            //     {
            //         HttpOnly = true,
            //         Secure = true,
            //         Expires = DateTime.UtcNow.AddDays(3),
            //         SameSite = SameSiteMode.Lax,
            //         Path = "/"
            //     }
            // );

            // await _userManager.UpdateAsync(userFound!);
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

            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            // Response.Cookies.Delete(REFRESH);

            // await _signInManager.SignOutAsync();

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
            var tokenService = new TokenConfiguration(_configuration);

            if (!Request.Cookies.ContainsKey("RefreshToken"))
                return Unauthorized(new { Message = "Refresh token not found" });

            var refreshToken = Request.Cookies["RefreshToken"];

            var principal = tokenService.ValidateRefreshToken(refreshToken!);

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

            // if (!Request.Cookies.ContainsKey(REFRESH))
            // {
            //     throw new CustomException(
            //         "Missing authentication cookie",
            //         "The required authentication cookie is not present. Please log in again.",
            //         StatusCodes.Status401Unauthorized
            //     );
            // }

            // TokenConfiguration token = new(_configuration);

            // var refreshToken = Request.Cookies[REFRESH];
            // var principal = token.ValidateRefreshToken(refreshToken!);

            // var userEmail = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (string.IsNullOrEmpty(userEmail))
            //     throw new CustomException(
            //         "Invalid token",
            //         "Invalid claims in token",
            //         StatusCodes.Status401Unauthorized
            //     );

            // var userFound =
            //     await _userManager.FindByEmailAsync(userEmail!)
            //     ?? throw new CustomException(
            //         "No user found",
            //         "No such user was found with those parameters",
            //         StatusCodes.Status404NotFound
            //     );

            // var newAccessToken = token.GenerateJwtToken(userFound);

            return Ok(
                JsonConvert.SerializeObject(
                    new { AccessToken = "Bearer " + newAccessToken },
                    Formatting.Indented
                )
            );
        }
    }
}
