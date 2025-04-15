using CosmoCargo.Models;
using CosmoCargo.Services;
using Microsoft.AspNetCore.Http;
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
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(user, configuration);
            return Results.Ok(new { Token = token, User = user });
        }

        public static async Task<IResult> Register(
            RegisterRequest request,
            IUserService userService)
        {
            var existingUser = await userService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Results.BadRequest("User with this email already exists");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = request.Role
            };

            await userService.CreateUserAsync(user, request.Password);
            return Results.Created($"/api/users/{user.Id}", user);
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

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
} 