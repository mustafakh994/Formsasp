using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FormsManagementApi.Configuration;
using FormsManagementApi.Data;
using FormsManagementApi.DTOs;
using FormsManagementApi.Models;
using Microsoft.Extensions.Options;
using AutoMapper;

namespace FormsManagementApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public AuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings, IMapper mapper)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Tenant)
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("User account is deactivated.");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // Store refresh token (in production, use a separate table)
            // For simplicity, we'll use a simple in-memory approach or cache

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Permissions = user.UserPermissions.Select(p => p.Permission).ToList();

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userDto,
                ExpiresAt = expiresAt
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful.");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse($"An error occurred during login: {ex.Message}");
        }
    }

    public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User with this email already exists.");
            }

            // Validate tenant if specified
            if (registerDto.TenantId.HasValue)
            {
                var tenant = await _context.Tenants.FindAsync(registerDto.TenantId.Value);
                if (tenant == null || !tenant.IsActive)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Invalid or inactive tenant.");
                }
            }

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                TenantId = registerDto.TenantId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto, "User registered successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.ErrorResponse($"An error occurred during registration: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<bool>.ErrorResponse("Current password is incorrect.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"An error occurred while changing password: {ex.Message}");
        }
    }

    public Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        // In production, implement proper refresh token validation
        // For now, return error as refresh token storage is not implemented
        return Task.FromResult(ApiResponse<LoginResponseDto>.ErrorResponse("Refresh token functionality not implemented."));
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("IsActive", user.IsActive.ToString())
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId)
    {
        // In production, validate against stored refresh tokens
        return Task.FromResult(false);
    }

    public Task RevokeRefreshTokenAsync(string refreshToken)
    {
        // In production, remove refresh token from storage
        return Task.CompletedTask;
    }
}