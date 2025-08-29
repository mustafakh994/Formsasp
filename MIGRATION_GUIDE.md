# Database Schema Migration Guide

This document outlines the major changes made to the Forms Management API to align with the new database structure.

## Major Changes

### 1. Database Provider Change
- **Changed from**: SQL Server
- **Changed to**: PostgreSQL
- **Reason**: Better support for JSONB columns and more flexible data storage

### 2. Primary Key Changes
- **Changed from**: `int` auto-increment IDs
- **Changed to**: `Guid` UUIDs
- **Impact**: All models now use UUID primary keys for better scalability and security

### 3. Model Updates

#### Tenant → Department
- Renamed `Tenant` model to `Department`
- Added `Code` field for unique department identification
- Replaced `TenantSettings` with JSONB `Settings` field
- Updated navigation properties

#### User Model
- Changed to use `DepartmentId` instead of `TenantId`
- Added `RoleId` foreign key
- Added JSONB fields: `Profile`, `CustomPermissions`
- Added `EmailVerifiedAt` and `LastLoginAt` timestamps
- Updated to use `DateTimeOffset` for better timezone handling

#### Form Model
- Added `Code`, `Title` fields
- Added JSONB `Settings` field
- Added `CreatedBy` user reference
- Added `Version` and `Status` fields
- Updated to support form versioning

#### Form Submissions
- Renamed `SubmissionData` to `ResponseData`
- Added `FormVersion` field
- Changed IP address storage to use PostgreSQL `INET` type
- Added `SubmitterEmail` field

### 4. New Models

#### Role-Based Access Control
- **Role**: Department-scoped roles with permissions
- **Permission**: Granular permissions with resource/action structure
- **RolePermission**: Many-to-many relationship between roles and permissions
- **UserPermission**: Override permissions for individual users

#### Audit & Monitoring
- **AuditLog**: Track all system actions with detailed context
- **UsageMetric**: Store usage statistics and metrics

#### Super Admin
- **SuperAdminUser**: Global administrators separate from department users

#### Form Versioning
- **FormSchemaVersion**: Track form schema changes over time

### 5. New Controllers
- `DepartmentsController` (replaces `TenantsController`)
- `RolesController`
- `PermissionsController`
- `SuperAdminUsersController`

### 6. Updated Services
- `IDepartmentService` (replaces `ITenantService`)
- `IRoleService`
- `IPermissionService`
- `ISuperAdminUserService`

## Migration Steps

### 1. Database Setup
1. Install PostgreSQL
2. Create database: `FormsManagementDb`
3. Run the provided `database_schema.sql` script

### 2. Package Updates
The following NuGet packages were updated:
- Removed: `Microsoft.EntityFrameworkCore.SqlServer`
- Added: `Npgsql.EntityFrameworkCore.PostgreSQL`

### 3. Configuration Updates
- Updated connection strings in `appsettings.json` and `appsettings.Development.json`
- Updated `docker-compose.yml` to use PostgreSQL instead of SQL Server

### 4. Code Changes
- All models updated to use `Guid` primary keys
- All DTOs updated to match new model structure
- Controllers updated to handle UUID parameters
- ApplicationDbContext updated with new models and relationships

## Breaking Changes

### API Endpoints
- All ID parameters changed from `int` to `Guid`
- `/api/tenants` → `/api/departments`
- New endpoints for roles and permissions management

### Request/Response Models
- All DTOs updated to use `Guid` IDs
- Date fields now use `DateTimeOffset` instead of `DateTime`
- JSON fields now properly typed as `object` in DTOs

### Database Schema
- Complete schema restructure
- Data migration required from old schema to new schema

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 15+
- Docker (optional)

### Running with Docker
```bash
docker-compose up -d
```

### Running Locally
1. Update connection string in `appsettings.Development.json`
2. Run database migrations or execute `database_schema.sql`
3. Start the application:
```bash
dotnet run --project FormsManagementApi
```

## Data Migration

**Important**: This is a breaking change that requires data migration. The old SQL Server database schema is not compatible with the new PostgreSQL schema.

To migrate existing data:
1. Export data from the old schema
2. Transform data to match new UUID-based structure
3. Import data into new PostgreSQL database

A separate data migration tool should be developed for production environments.

## Security Considerations

- UUID primary keys provide better security than sequential integers
- Role-based access control provides more granular permissions
- Audit logging tracks all system changes
- Super admin users are isolated from department users

## Performance Considerations

- Added indexes on frequently queried fields
- JSONB fields provide efficient JSON storage and querying
- UUID primary keys may have slight performance impact vs integers
- Consider using `uuid_generate_v4()` function for better performance

## Testing

All existing tests will need to be updated to:
- Use `Guid` IDs instead of `int`
- Update model assertions for new fields
- Test new role-based access control
- Test new audit logging functionality