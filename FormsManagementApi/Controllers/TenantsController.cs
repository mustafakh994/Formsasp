using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FormsManagementApi.DTOs;
using FormsManagementApi.Services;
using FormsManagementApi.Middleware;

namespace FormsManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Get all tenants (SuperAdmin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<PagedResult<TenantDto>>>> GetTenants([FromQuery] PaginationDto pagination)
    {
        var result = await _tenantService.GetTenantsAsync(pagination);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get tenant by ID (SuperAdmin or tenant members)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> GetTenant(int id)
    {
        // Check authorization - SuperAdmin can view any tenant, users can only view their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (userTenantId != id)
            {
                return Forbid("You can only access your own tenant information.");
            }
        }

        var result = await _tenantService.GetTenantByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new tenant (SuperAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> CreateTenant([FromBody] CreateTenantDto createTenantDto)
    {
        var result = await _tenantService.CreateTenantAsync(createTenantDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetTenant), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update tenant (SuperAdmin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> UpdateTenant(int id, [FromBody] UpdateTenantDto updateTenantDto)
    {
        var result = await _tenantService.UpdateTenantAsync(id, updateTenantDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete tenant (SuperAdmin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTenant(int id)
    {
        var result = await _tenantService.DeleteTenantAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get tenant settings (SuperAdmin or tenant members)
    /// </summary>
    [HttpGet("{tenantId}/settings")]
    public async Task<ActionResult<ApiResponse<List<TenantSettingsDto>>>> GetTenantSettings(int tenantId)
    {
        // Check authorization
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (userTenantId != tenantId)
            {
                return Forbid("You can only access your own tenant settings.");
            }
        }

        var result = await _tenantService.GetTenantSettingsAsync(tenantId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create tenant setting (SuperAdmin or TenantAdmin)
    /// </summary>
    [HttpPost("{tenantId}/settings")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<TenantSettingsDto>>> CreateTenantSetting(int tenantId, [FromBody] CreateTenantSettingDto createSettingDto)
    {
        // Check authorization for TenantAdmin
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (userTenantId != tenantId)
            {
                return Forbid("You can only manage your own tenant settings.");
            }
        }

        var result = await _tenantService.CreateTenantSettingAsync(tenantId, createSettingDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetTenantSettings), new { tenantId }, result);
    }

    /// <summary>
    /// Update tenant setting (SuperAdmin or TenantAdmin)
    /// </summary>
    [HttpPut("{tenantId}/settings/{key}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<TenantSettingsDto>>> UpdateTenantSetting(int tenantId, string key, [FromBody] UpdateTenantSettingDto updateSettingDto)
    {
        // Check authorization for TenantAdmin
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (userTenantId != tenantId)
            {
                return Forbid("You can only manage your own tenant settings.");
            }
        }

        var result = await _tenantService.UpdateTenantSettingAsync(tenantId, key, updateSettingDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete tenant setting (SuperAdmin or TenantAdmin)
    /// </summary>
    [HttpDelete("{tenantId}/settings/{key}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTenantSetting(int tenantId, string key)
    {
        // Check authorization for TenantAdmin
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (userTenantId != tenantId)
            {
                return Forbid("You can only manage your own tenant settings.");
            }
        }

        var result = await _tenantService.DeleteTenantSettingAsync(tenantId, key);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}