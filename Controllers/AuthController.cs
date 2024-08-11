using System.Net;
using EasyWheelsApi.Configuration;
using EasyWheelsApi.Models.Dtos;
using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto user)
        {
            throw new Exception("Não vai fazer login irmão");
            
            var userFound = await _userManager.FindByEmailAsync(user.Email);

            if (userFound is null)
                return Unauthorized(
                    new IdentityError
                    {
                        Code = "401",
                        Description = "No such user was found with those parameters"
                    }
                );

            var result = await _signInManager.PasswordSignInAsync(
                userFound!,
                user.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                return Unauthorized();

            TokenConfiguration token = new TokenConfiguration(_configuration);

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
                    SameSite = SameSiteMode.Strict
                }
            );

            await _userManager.UpdateAsync(userFound!);
            return Ok(new { AccessToken = "Bearer " + accessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] EmailDto email)
        {
            if (string.IsNullOrEmpty(email.Email))
            {
                return BadRequest("Email is required for logout verification.");
            }

            var refreshToken = Request.Cookies[REFRESH];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Ok(new { Message = "Logout successful, no refresh token found" });
            }

            await _signInManager.SignOutAsync();
            Response.Cookies.Delete(REFRESH);

            return Ok(new { Message = "Logout successful" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] EmailDto email)
        {
            if (!Request.Cookies.TryGetValue(REFRESH, out var refreshtoken))
                return Unauthorized();

            var userFound = await _userManager.FindByEmailAsync(email.Email);

            if (userFound is null)
                return Unauthorized(
                    new IdentityError
                    {
                        Code = "401",
                        Description = "No such user was found with those parameters"
                    }
                );

            TokenConfiguration token = new TokenConfiguration(_configuration);

            var newAccessToken = token.GenerateJwtToken(userFound);

            return Ok(new { AccessToken = "Bearer " + newAccessToken });
        }
    }
}
