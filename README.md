# Forms Management API

A comprehensive ASP.NET Core Web API for managing forms in a multi-tenant environment. This API provides robust functionality for SuperAdmins to manage tenants and for tenant administrators to manage forms, users, and form submissions.

## Features

### Multi-Tenant Architecture
- **SuperAdmin Dashboard**: Complete control over tenants, users, and system-wide settings
- **Tenant Dashboard**: Tenant-specific management of forms, users, and submissions
- **Tenant Isolation**: Data segregation and security at the tenant level

### Core Functionality
- **User Management**: Create, update, delete users with role-based access control
- **Form Management**: Dynamic form creation with JSON schema support
- **Form Submissions**: Collect and manage form responses
- **Webhook Integration**: Real-time notifications for form submissions
- **Permission System**: Granular permissions for forms and users

### Security Features
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: SuperAdmin, TenantAdmin, and User roles
- **Tenant Isolation**: Middleware ensures data separation between tenants
- **Password Hashing**: BCrypt encryption for secure password storage

## Database Schema

The API uses the following main entities:

- **Tenants**: Organizations using the system
- **Users**: System users with different roles
- **Forms**: Dynamic forms with JSON schema
- **FormSubmissions**: User responses to forms
- **TenantSettings**: Configurable tenant-specific settings
- **UserPermissions**: Granular user permissions
- **FormPermissions**: Form-specific access control
- **WebhookEndpoints**: Webhook configurations for notifications

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/change-password` - Change password
- `GET /api/auth/profile` - Get user profile

### Tenants (SuperAdmin only)
- `GET /api/tenants` - List all tenants
- `GET /api/tenants/{id}` - Get tenant by ID
- `POST /api/tenants` - Create new tenant
- `PUT /api/tenants/{id}` - Update tenant
- `DELETE /api/tenants/{id}` - Delete tenant
- `GET /api/tenants/{id}/settings` - Get tenant settings
- `POST /api/tenants/{id}/settings` - Create tenant setting
- `PUT /api/tenants/{id}/settings/{key}` - Update tenant setting
- `DELETE /api/tenants/{id}/settings/{key}` - Delete tenant setting

### Users
- `GET /api/users` - List users (filtered by tenant)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `PATCH /api/users/{id}/toggle-status` - Toggle user status
- `GET /api/users/{id}/permissions` - Get user permissions
- `POST /api/users/{id}/permissions` - Add user permission
- `DELETE /api/users/{id}/permissions/{permission}` - Remove user permission

### Forms
- `GET /api/forms` - List forms (filtered by tenant)
- `GET /api/forms/{id}` - Get form by ID
- `POST /api/forms` - Create new form
- `PUT /api/forms/{id}` - Update form
- `DELETE /api/forms/{id}` - Delete form
- `PATCH /api/forms/{id}/toggle-status` - Toggle form status
- `GET /api/forms/{id}/submissions` - Get form submissions
- `POST /api/forms/{id}/submissions` - Submit form (public endpoint)
- `GET /api/forms/submissions/{id}` - Get submission by ID
- `DELETE /api/forms/submissions/{id}` - Delete submission
- `GET /api/forms/{id}/permissions` - Get form permissions
- `POST /api/forms/{id}/permissions` - Add form permission
- `DELETE /api/forms/{id}/permissions/{userId}/{permission}` - Remove form permission

### Webhooks
- `GET /api/webhooks` - List webhook endpoints
- `GET /api/webhooks/{id}` - Get webhook by ID
- `POST /api/webhooks` - Create webhook endpoint
- `PUT /api/webhooks/{id}` - Update webhook endpoint
- `DELETE /api/webhooks/{id}` - Delete webhook endpoint
- `PATCH /api/webhooks/{id}/toggle-status` - Toggle webhook status

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd FormsManagementApi
   ```

2. **Configure Database Connection**
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FormsManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Configure JWT Settings**
   Update JWT settings in `appsettings.json`:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
       "Issuer": "FormsManagementAPI",
       "Audience": "FormsManagementClients",
       "ExpirationInMinutes": 60,
       "RefreshTokenExpirationInDays": 7
     }
   }
   ```

4. **Install Dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

6. **Access Swagger Documentation**
   Navigate to `https://localhost:7000` (or the configured port) to access the Swagger UI.

### Default SuperAdmin Account
The system automatically creates a default SuperAdmin account:
- **Email**: `superadmin@system.com`
- **Password**: `SuperAdmin123!`

**Important**: Change these credentials immediately after first login in production.

## Configuration

### JWT Settings
Configure JWT authentication in `appsettings.json`:
- `SecretKey`: Secret key for JWT signing (minimum 32 characters)
- `Issuer`: Token issuer
- `Audience`: Token audience
- `ExpirationInMinutes`: Token expiration time
- `RefreshTokenExpirationInDays`: Refresh token expiration time

### Database Configuration
The application uses Entity Framework Core with SQL Server. For production:
1. Update connection string
2. Use migrations instead of `EnsureCreated()`
3. Configure proper backup and recovery

### CORS Configuration
Update CORS policy in `Program.cs` for production:
```csharp
options.AddPolicy("Production", policy =>
{
    policy.WithOrigins("https://yourdomain.com")
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

## Usage Examples

### Authentication
```javascript
// Login
const loginResponse = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'superadmin@system.com',
    password: 'SuperAdmin123!'
  })
});

const { data } = await loginResponse.json();
const token = data.token;

// Use token in subsequent requests
const headers = {
  'Authorization': `Bearer ${token}`,
  'Content-Type': 'application/json'
};
```

### Creating a Tenant (SuperAdmin)
```javascript
const tenantResponse = await fetch('/api/tenants', {
  method: 'POST',
  headers,
  body: JSON.stringify({
    name: 'Acme Corporation',
    description: 'A sample tenant organization',
    isActive: true
  })
});
```

### Creating a Form (TenantAdmin)
```javascript
const formResponse = await fetch('/api/forms', {
  method: 'POST',
  headers,
  body: JSON.stringify({
    name: 'Contact Form',
    description: 'A simple contact form',
    formSchema: {
      fields: [
        {
          name: 'name',
          type: 'text',
          label: 'Full Name',
          required: true
        },
        {
          name: 'email',
          type: 'email',
          label: 'Email Address',
          required: true
        },
        {
          name: 'message',
          type: 'textarea',
          label: 'Message',
          required: true
        }
      ]
    },
    isActive: true
  })
});
```

### Submitting a Form (Public)
```javascript
const submissionResponse = await fetch('/api/forms/1/submissions', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    submissionData: {
      name: 'John Doe',
      email: 'john@example.com',
      message: 'Hello, this is a test message!'
    }
  })
});
```

## Architecture

### Multi-Tenancy
The application implements tenant isolation through:
- **Tenant Middleware**: Extracts tenant information from JWT tokens
- **Data Filtering**: Automatic filtering of data by tenant ID
- **Authorization**: Role-based access control with tenant context

### Security
- **JWT Authentication**: Stateless authentication with configurable expiration
- **Password Security**: BCrypt hashing with salt
- **Authorization**: Role and permission-based access control
- **Input Validation**: FluentValidation for request validation

### Data Layer
- **Entity Framework Core**: ORM with Code First approach
- **Automatic Timestamps**: CreatedAt and UpdatedAt fields
- **Soft Deletes**: Configurable soft delete functionality
- **Relationships**: Proper foreign key relationships with cascade rules

## Deployment

### Production Considerations
1. **Security**:
   - Use HTTPS only
   - Configure proper CORS policies
   - Use strong JWT secret keys
   - Enable request rate limiting

2. **Database**:
   - Use SQL Server or Azure SQL Database
   - Configure connection pooling
   - Set up backup strategies
   - Use migrations for schema updates

3. **Monitoring**:
   - Configure logging (Serilog recommended)
   - Set up health checks
   - Monitor API performance
   - Track webhook delivery success rates

4. **Scalability**:
   - Consider Redis for caching
   - Implement background job processing
   - Use load balancing for multiple instances

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation at `/swagger`