using FormsManagementApi.DTOs;

namespace FormsManagementApi.Services;

public interface IFormService
{
    Task<ApiResponse<PagedResult<FormDto>>> GetFormsAsync(PaginationDto pagination, int? tenantId = null);
    Task<ApiResponse<FormDto>> GetFormByIdAsync(int id);
    Task<ApiResponse<FormDto>> CreateFormAsync(CreateFormDto createFormDto, int tenantId);
    Task<ApiResponse<FormDto>> UpdateFormAsync(int id, UpdateFormDto updateFormDto);
    Task<ApiResponse<bool>> DeleteFormAsync(int id);
    Task<ApiResponse<bool>> ToggleFormStatusAsync(int id);
    Task<ApiResponse<PagedResult<FormSubmissionDto>>> GetFormSubmissionsAsync(int formId, PaginationDto pagination);
    Task<ApiResponse<FormSubmissionDto>> GetFormSubmissionByIdAsync(int submissionId);
    Task<ApiResponse<FormSubmissionDto>> CreateFormSubmissionAsync(int formId, CreateFormSubmissionDto createSubmissionDto, int? userId = null);
    Task<ApiResponse<bool>> DeleteFormSubmissionAsync(int submissionId);
    Task<ApiResponse<List<FormPermissionDto>>> GetFormPermissionsAsync(int formId);
    Task<ApiResponse<FormPermissionDto>> AddFormPermissionAsync(int formId, CreateFormPermissionDto createPermissionDto);
    Task<ApiResponse<bool>> RemoveFormPermissionAsync(int formId, int userId, string permission);
}