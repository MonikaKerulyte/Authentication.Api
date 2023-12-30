using Authentication.Api.Model.Dto;
using Authentication.Api.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Api.Services
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;
        private ILogger<UserService> _logger;

        public UserService(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse> AuthenticateUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new ApiResponse
                {
                    Message = "User was not found with this email.",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return new ApiResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };
            }

            var claims = new[]
            {
                new Claim("Email", request.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
#pragma warning restore CS8604 // Possible null reference argument.

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ApiResponse
            {
                Message = tokenString,
                IsSuccess = true,
                ExpireDate = token.ValidTo,
            };
        }

        public async Task<ApiResponse> RegisterUserAsync(RegisterRequest request)
        {
            if (request == null) throw new NullReferenceException(nameof(request));

            if (request.Password != request.ConfirmPassword)
            {
                _logger.LogDebug($"Password '{request.Password}' does not match ConfirmPasword '{request.ConfirmPassword}'");
                return new ApiResponse
                {
                    Message = "Passwords do not match.",
                    IsSuccess = false,
                };
            }

            var identityUser = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, request.Password);

            if (result.Succeeded)
            {
                return new ApiResponse
                {
                    Message = "User registered successfully!",
                    IsSuccess = true,
                };
            }

            foreach (string e in result.Errors.Select(e => e.Description))
                _logger.LogDebug(e);

            return new ApiResponse
            {
                Message = "User could not be registered.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
