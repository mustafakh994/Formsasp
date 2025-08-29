using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FormsManagementApi.DTOs;
using FormsManagementApi.Services;
using FormsManagementApi.Middleware;

namespace FormsManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;

    public FormsController(IFormService formService)
    {
        _formService = formService;
    }

    /// <summary>
    /// Get all forms with optional tenant filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FormDto>>>> GetForms([FromQuery] PaginationDto pagination)
    {
        int? tenantId = null;
        
        // SuperAdmin can see all forms, others can only see forms from their tenant
        if (!HttpContext.IsSuperAdmin())
        {
            tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
            {
                return Forbid("You must be associated with a tenant to view forms.");
            }
        }

        var result = await _formService.GetFormsAsync(pagination, tenantId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get form by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FormDto>>> GetForm(int id)
    {
        var result = await _formService.GetFormByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        // Check authorization - users can only view forms from their tenant (except SuperAdmin)
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (result.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only access forms from your tenant.");
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new form (TenantAdmin or users with create permission)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<FormDto>>> CreateForm([FromBody] CreateFormDto createFormDto)
    {
        var tenantId = HttpContext.GetTenantId();
        if (!tenantId.HasValue && !HttpContext.IsSuperAdmin())
        {
            return Forbid("You must be associated with a tenant to create forms.");
        }

        // SuperAdmin must specify tenant in a different way or use a separate endpoint
        if (HttpContext.IsSuperAdmin() && !tenantId.HasValue)
        {
            return BadRequest("SuperAdmin must specify tenant when creating forms.");
        }

        var result = await _formService.CreateFormAsync(createFormDto, tenantId!.Value);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetForm), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update form (TenantAdmin or users with update permission)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<FormDto>>> UpdateForm(int id, [FromBody] UpdateFormDto updateFormDto)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(id);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only update forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only update forms in your own tenant.");
            }
        }

        var result = await _formService.UpdateFormAsync(id, updateFormDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete form (TenantAdmin or users with delete permission)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteForm(int id)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(id);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only delete forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only delete forms in your own tenant.");
            }
        }

        var result = await _formService.DeleteFormAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Toggle form active status (TenantAdmin)
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleFormStatus(int id)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(id);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only toggle forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only manage forms in your own tenant.");
            }
        }

        var result = await _formService.ToggleFormStatusAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get form submissions
    /// </summary>
    [HttpGet("{formId}/submissions")]
    public async Task<ActionResult<ApiResponse<PagedResult<FormSubmissionDto>>>> GetFormSubmissions(int formId, [FromQuery] PaginationDto pagination)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(formId);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // Users can only view submissions from forms in their tenant (except SuperAdmin)
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only view submissions from forms in your tenant.");
            }
        }

        var result = await _formService.GetFormSubmissionsAsync(formId, pagination);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get form submission by ID
    /// </summary>
    [HttpGet("submissions/{submissionId}")]
    public async Task<ActionResult<ApiResponse<FormSubmissionDto>>> GetFormSubmission(int submissionId)
    {
        var result = await _formService.GetFormSubmissionByIdAsync(submissionId);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        // Check authorization through form's tenant
        var formResult = await _formService.GetFormByIdAsync(result.Data!.FormId);
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (formResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only view submissions from forms in your tenant.");
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Create form submission (public endpoint for form filling)
    /// </summary>
    [HttpPost("{formId}/submissions")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<FormSubmissionDto>>> CreateFormSubmission(int formId, [FromBody] CreateFormSubmissionDto createSubmissionDto)
    {
        // Get user ID if authenticated
        int? userId = null;
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            userId = HttpContext.GetUserId();
        }

        // Set IP address and user agent
        createSubmissionDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        createSubmissionDto.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var result = await _formService.CreateFormSubmissionAsync(formId, createSubmissionDto, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetFormSubmission), new { submissionId = result.Data!.Id }, result);
    }

    /// <summary>
    /// Delete form submission (TenantAdmin)
    /// </summary>
    [HttpDelete("submissions/{submissionId}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFormSubmission(int submissionId)
    {
        // Get submission to check authorization
        var submissionResult = await _formService.GetFormSubmissionByIdAsync(submissionId);
        if (!submissionResult.Success)
        {
            return NotFound(submissionResult);
        }

        // Check authorization through form's tenant
        var formResult = await _formService.GetFormByIdAsync(submissionResult.Data!.FormId);
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (formResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only delete submissions from forms in your tenant.");
            }
        }

        var result = await _formService.DeleteFormSubmissionAsync(submissionId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get form permissions
    /// </summary>
    [HttpGet("{formId}/permissions")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<List<FormPermissionDto>>>> GetFormPermissions(int formId)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(formId);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only view permissions for forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only view permissions for forms in your own tenant.");
            }
        }

        var result = await _formService.GetFormPermissionsAsync(formId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Add form permission
    /// </summary>
    [HttpPost("{formId}/permissions")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<FormPermissionDto>>> AddFormPermission(int formId, [FromBody] CreateFormPermissionDto createPermissionDto)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(formId);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only manage permissions for forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only manage permissions for forms in your own tenant.");
            }
        }

        var result = await _formService.AddFormPermissionAsync(formId, createPermissionDto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetFormPermissions), new { formId }, result);
    }

    /// <summary>
    /// Remove form permission
    /// </summary>
    [HttpDelete("{formId}/permissions/{userId}/{permission}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFormPermission(int formId, int userId, string permission)
    {
        // Get current form info to check authorization
        var currentFormResult = await _formService.GetFormByIdAsync(formId);
        if (!currentFormResult.Success)
        {
            return NotFound(currentFormResult);
        }

        // TenantAdmin can only manage permissions for forms in their own tenant
        if (!HttpContext.IsSuperAdmin())
        {
            var userTenantId = HttpContext.GetTenantId();
            if (currentFormResult.Data!.TenantId != userTenantId)
            {
                return Forbid("You can only manage permissions for forms in your own tenant.");
            }
        }

        var result = await _formService.RemoveFormPermissionAsync(formId, userId, permission);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}