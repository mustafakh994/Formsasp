using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FormsManagementApi.Data;
using FormsManagementApi.DTOs;
using FormsManagementApi.Models;

namespace FormsManagementApi.Services;

public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TenantService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<TenantDto>>> GetTenantsAsync(PaginationDto pagination)
    {
        try
        {
            var query = _context.Tenants
                .Include(t => t.Users)
                .Include(t => t.Forms)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(t => t.Name.Contains(pagination.Search) || 
                                       (t.Description != null && t.Description.Contains(pagination.Search)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                switch (pagination.SortBy.ToLower())
                {
                    case "name":
                        query = pagination.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name);
                        break;
                    case "createdat":
                        query = pagination.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt);
                        break;
                    case "isactive":
                        query = pagination.SortDescending ? query.OrderByDescending(t => t.IsActive) : query.OrderBy(t => t.IsActive);
                        break;
                    default:
                        query = query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var tenantDtos = _mapper.Map<List<TenantDto>>(items);
            var pagedResult = new PagedResult<TenantDto>(tenantDtos, totalItems, pagination.Page, pagination.PageSize);

            return ApiResponse<PagedResult<TenantDto>>.SuccessResponse(pagedResult, "Tenants retrieved successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<TenantDto>>.ErrorResponse($"An error occurred while retrieving tenants: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TenantDto>> GetTenantByIdAsync(int id)
    {
        try
        {
            var tenant = await _context.Tenants
                .Include(t => t.Users)
                .Include(t => t.Forms)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
            {
                return ApiResponse<TenantDto>.ErrorResponse("Tenant not found.");
            }

            var tenantDto = _mapper.Map<TenantDto>(tenant);
            return ApiResponse<TenantDto>.SuccessResponse(tenantDto, "Tenant retrieved successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TenantDto>.ErrorResponse($"An error occurred while retrieving tenant: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TenantDto>> CreateTenantAsync(CreateTenantDto createTenantDto)
    {
        try
        {
            // Check if tenant name already exists
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == createTenantDto.Name);
            if (existingTenant != null)
            {
                return ApiResponse<TenantDto>.ErrorResponse("Tenant with this name already exists.");
            }

            var tenant = _mapper.Map<Tenant>(createTenantDto);
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            var tenantDto = _mapper.Map<TenantDto>(tenant);
            return ApiResponse<TenantDto>.SuccessResponse(tenantDto, "Tenant created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TenantDto>.ErrorResponse($"An error occurred while creating tenant: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TenantDto>> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return ApiResponse<TenantDto>.ErrorResponse("Tenant not found.");
            }

            // Check if new name conflicts with existing tenant
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == updateTenantDto.Name && t.Id != id);
            if (existingTenant != null)
            {
                return ApiResponse<TenantDto>.ErrorResponse("Tenant with this name already exists.");
            }

            _mapper.Map(updateTenantDto, tenant);
            await _context.SaveChangesAsync();

            var tenantDto = _mapper.Map<TenantDto>(tenant);
            return ApiResponse<TenantDto>.SuccessResponse(tenantDto, "Tenant updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TenantDto>.ErrorResponse($"An error occurred while updating tenant: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteTenantAsync(int id)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return ApiResponse<bool>.ErrorResponse("Tenant not found.");
            }

            // Check if tenant has active users or forms
            var hasUsers = await _context.Users.AnyAsync(u => u.TenantId == id);
            var hasForms = await _context.Forms.AnyAsync(f => f.TenantId == id);

            if (hasUsers || hasForms)
            {
                return ApiResponse<bool>.ErrorResponse("Cannot delete tenant with existing users or forms. Please transfer or delete them first.");
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Tenant deleted successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"An error occurred while deleting tenant: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<TenantSettingsDto>>> GetTenantSettingsAsync(int tenantId)
    {
        try
        {
            var settings = await _context.TenantSettings
                .Where(ts => ts.TenantId == tenantId)
                .OrderBy(ts => ts.Key)
                .ToListAsync();

            var settingsDtos = _mapper.Map<List<TenantSettingsDto>>(settings);
            return ApiResponse<List<TenantSettingsDto>>.SuccessResponse(settingsDtos, "Tenant settings retrieved successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<TenantSettingsDto>>.ErrorResponse($"An error occurred while retrieving tenant settings: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TenantSettingsDto>> CreateTenantSettingAsync(int tenantId, CreateTenantSettingDto createSettingDto)
    {
        try
        {
            // Check if tenant exists
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return ApiResponse<TenantSettingsDto>.ErrorResponse("Tenant not found.");
            }

            // Check if setting key already exists for this tenant
            var existingSetting = await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == tenantId && ts.Key == createSettingDto.Key);
            if (existingSetting != null)
            {
                return ApiResponse<TenantSettingsDto>.ErrorResponse("Setting with this key already exists for this tenant.");
            }

            var setting = _mapper.Map<TenantSettings>(createSettingDto);
            setting.TenantId = tenantId;

            _context.TenantSettings.Add(setting);
            await _context.SaveChangesAsync();

            var settingDto = _mapper.Map<TenantSettingsDto>(setting);
            return ApiResponse<TenantSettingsDto>.SuccessResponse(settingDto, "Tenant setting created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TenantSettingsDto>.ErrorResponse($"An error occurred while creating tenant setting: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TenantSettingsDto>> UpdateTenantSettingAsync(int tenantId, string key, UpdateTenantSettingDto updateSettingDto)
    {
        try
        {
            var setting = await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == tenantId && ts.Key == key);

            if (setting == null)
            {
                return ApiResponse<TenantSettingsDto>.ErrorResponse("Tenant setting not found.");
            }

            _mapper.Map(updateSettingDto, setting);
            await _context.SaveChangesAsync();

            var settingDto = _mapper.Map<TenantSettingsDto>(setting);
            return ApiResponse<TenantSettingsDto>.SuccessResponse(settingDto, "Tenant setting updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TenantSettingsDto>.ErrorResponse($"An error occurred while updating tenant setting: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteTenantSettingAsync(int tenantId, string key)
    {
        try
        {
            var setting = await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == tenantId && ts.Key == key);

            if (setting == null)
            {
                return ApiResponse<bool>.ErrorResponse("Tenant setting not found.");
            }

            _context.TenantSettings.Remove(setting);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Tenant setting deleted successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"An error occurred while deleting tenant setting: {ex.Message}");
        }
    }
}