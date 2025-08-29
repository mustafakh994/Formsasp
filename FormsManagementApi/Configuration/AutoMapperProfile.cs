using AutoMapper;
using FormsManagementApi.DTOs;
using FormsManagementApi.Models;
using System.Text.Json;

namespace FormsManagementApi.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Name : null))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.UserPermissions.Select(p => p.Permission).ToList()));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UserPermissions, opt => opt.Ignore());

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UserPermissions, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Tenant mappings
        CreateMap<Tenant, TenantDto>()
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count))
            .ForMember(dest => dest.FormCount, opt => opt.MapFrom(src => src.Forms.Count));

        CreateMap<CreateTenantDto, Tenant>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateTenantDto, Tenant>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // TenantSettings mappings
        CreateMap<TenantSettings, TenantSettingsDto>();
        CreateMap<CreateTenantSettingDto, TenantSettings>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateTenantSettingDto, TenantSettings>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Key, opt => opt.Ignore());

        // Form mappings
        CreateMap<Form, FormDto>()
            .ForMember(dest => dest.FormSchema, opt => opt.MapFrom(src => DeserializeJson(src.FormSchema)))
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
            .ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src => src.FormSubmissions.Count));

        CreateMap<CreateFormDto, Form>()
            .ForMember(dest => dest.FormSchema, opt => opt.MapFrom(src => SerializeJson(src.FormSchema)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateFormDto, Form>()
            .ForMember(dest => dest.FormSchema, opt => opt.MapFrom(src => SerializeJson(src.FormSchema)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore());

        // FormSubmission mappings
        CreateMap<FormSubmission, FormSubmissionDto>()
            .ForMember(dest => dest.SubmissionData, opt => opt.MapFrom(src => DeserializeJson(src.SubmissionData)))
            .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null));

        CreateMap<CreateFormSubmissionDto, FormSubmission>()
            .ForMember(dest => dest.SubmissionData, opt => opt.MapFrom(src => SerializeJson(src.SubmissionData)))
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UserPermission mappings
        CreateMap<UserPermission, UserPermissionDto>();

        // FormPermission mappings
        CreateMap<FormPermission, FormPermissionDto>()
            .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<CreateFormPermissionDto, FormPermission>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // WebhookEndpoint mappings
        CreateMap<WebhookEndpoint, WebhookEndpointDto>()
            .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Headers) ? DeserializeJson(src.Headers) : null))
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name));

        CreateMap<CreateWebhookEndpointDto, WebhookEndpoint>()
            .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers != null ? SerializeJson(src.Headers) : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateWebhookEndpointDto, WebhookEndpoint>()
            .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers != null ? SerializeJson(src.Headers) : null))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore());
    }

    private static object? DeserializeJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        
        try
        {
            return JsonSerializer.Deserialize<object>(json);
        }
        catch
        {
            return json; // Return as string if deserialization fails
        }
    }

    private static string SerializeJson(object obj)
    {
        if (obj == null)
            return string.Empty;
        
        try
        {
            return JsonSerializer.Serialize(obj);
        }
        catch
        {
            return obj.ToString() ?? string.Empty;
        }
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }
}