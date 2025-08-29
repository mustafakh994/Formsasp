using FormsManagementApi.DTOs;
using FormsManagementApi.Models;

namespace FormsManagementApi.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId);
    Task RevokeRefreshTokenAsync(string refreshToken);
}