using FormsManagementApi.DTOs;

namespace FormsManagementApi.Services;

public interface ITenantService
{
    Task<ApiResponse<PagedResult<TenantDto>>> GetTenantsAsync(PaginationDto pagination);
    Task<ApiResponse<TenantDto>> GetTenantByIdAsync(int id);
    Task<ApiResponse<TenantDto>> CreateTenantAsync(CreateTenantDto createTenantDto);
    Task<ApiResponse<TenantDto>> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto);
    Task<ApiResponse<bool>> DeleteTenantAsync(int id);
    Task<ApiResponse<List<TenantSettingsDto>>> GetTenantSettingsAsync(int tenantId);
    Task<ApiResponse<TenantSettingsDto>> CreateTenantSettingAsync(int tenantId, CreateTenantSettingDto createSettingDto);
    Task<ApiResponse<TenantSettingsDto>> UpdateTenantSettingAsync(int tenantId, string key, UpdateTenantSettingDto updateSettingDto);
    Task<ApiResponse<bool>> DeleteTenantSettingAsync(int tenantId, string key);
}