using Authentication.Api.Model.Dto;

namespace Authentication.Api.Services.Interface
{
    public interface IUserService
    {
        Task<ApiResponse> RegisterUserAsync(RegisterRequest request);
        Task<ApiResponse> AuthenticateUserAsync(LoginRequest request);
    }
}
