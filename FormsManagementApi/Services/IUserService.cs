using FormsManagementApi.DTOs;

namespace FormsManagementApi.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(PaginationDto pagination, int? tenantId = null);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
    Task<ApiResponse<bool>> ToggleUserStatusAsync(int id);
    Task<ApiResponse<List<UserPermissionDto>>> GetUserPermissionsAsync(int userId);
    Task<ApiResponse<UserPermissionDto>> AddUserPermissionAsync(int userId, string permission);
    Task<ApiResponse<bool>> RemoveUserPermissionAsync(int userId, string permission);
}