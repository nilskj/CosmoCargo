using CosmoCargo.Models;
using CosmoCargo.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CosmoCargo.Endpoints
{
    public static class AuthEndpoints
    {
        public static async Task<IResult> Login(
            LoginRequest request,
            IUserService userService,
            IConfiguration configuration)
        {
            if (!await userService.ValidateUserCredentialsAsync(request.Email, request.Password))
                return Results.Unauthorized();

            var user = await userService.GetUserByEmailAsync(request.Email);
            if (user == null)
                return Results.Unauthorized();

            var token = GenerateJwtToken(user, configuration);
            return Results.Ok(new { Token = token, User = user });
        }

        private static string GenerateJwtToken(User user, IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"] ?? "defaultDevKeyThatShouldBeReplaced"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(
                configuration["Jwt:ExpiryMinutes"] ?? "60"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
