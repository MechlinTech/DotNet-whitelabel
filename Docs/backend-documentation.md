# Backend Documentation

## Architecture Overview

The backend is built using .NET Core with a clean architecture approach, implementing the following layers:

### 1. Controllers Layer

- RESTful API endpoints
- Request/Response handling
- Input validation
- Authentication middleware integration

### 2. Services Layer

- Business logic implementation
- Data processing
- External service integration
- Error handling

### 3. Data Access Layer

- Entity Framework Core implementation
- Database context management
- Repository pattern
- Migration handling

## API Endpoints

### Authentication

```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
POST /api/auth/logout
```

### SSO Authentication

```
POST /api/sso/google
POST /api/sso/microsoft
```

### Role Management

```
POST /api/role/init                    # Initialize user role (Anonymous)
POST /api/role                         # Create new role (Admin only)
DELETE /api/role/{roleName}           # Delete role (Admin only)
POST /api/role/assign                 # Assign role to user (Admin only)
POST /api/role/remove                 # Remove role from user (Admin only)
GET /api/role                         # Get all roles (Admin only)
GET /api/role/by-role/{roleName}/users # Get users in role (Admin only)
GET /api/role/by-user/{userId}        # Get user roles (Admin only)
```

### Tenant Management

```
GET /api/tenants
POST /api/tenants
PUT /api/tenants/{id}
DELETE /api/tenants/{id}
GET /api/tenants/{id}
```

### CRM Features

#### Deals (Admin, Manager, Sales)

```
GET /api/deals                        # Get all deals
GET /api/deals/customer/{customerId}  # Get deals by customer
GET /api/deals/stage/{stage}         # Get deals by stage
GET /api/deals/{id}                  # Get deal by ID
POST /api/deals                      # Create new deal (Admin, Manager, Sales)
PUT /api/deals/{id}                  # Update deal (Admin, Manager)
DELETE /api/deals/{id}               # Delete deal (Admin only)
```

#### Contacts (Admin, Manager, Sales)

```
GET /api/contacts                     # Get all contacts
GET /api/contacts/customer/{customerId} # Get contacts by customer
GET /api/contacts/{id}               # Get contact by ID
POST /api/contacts                   # Create new contact (Admin, Manager, Sales)
PUT /api/contacts/{id}               # Update contact (Admin, Manager)
DELETE /api/contacts/{id}            # Delete contact (Admin only)
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WhiteLabel;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpiryInMinutes": 60
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-domain",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc"
  }
}
```

## Middleware Components

### 1. Authentication Middleware

- JWT token validation
- Role-based authorization
- Tenant context resolution

### 2. Exception Handling Middleware

- Global error handling
- Custom exception types
- Error response formatting

### 3. Logging Middleware

- Request/Response logging
- Performance monitoring
- Error tracking

## Security

### Authentication Methods

1. JWT Token Authentication

   - Token generation
   - Token validation
   - Refresh token mechanism

2. Azure AD Integration
   - OAuth 2.0 flow
   - OpenID Connect
   - Role-based access control

### Authorization

- Role-based access control (RBAC)
- Policy-based authorization
- Resource-based authorization

## Development Guidelines

### Code Structure

```
backend/
├── Controllers/
│   ├── AuthController.cs
│   ├── TenantController.cs
│   └── CustomerController.cs
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Models/
│   ├── Entities/
│   └── DTOs/
├── DBContext/
│   └── ApplicationDbContext.cs
└── Middleware/
    ├── AuthenticationMiddleware.cs
    └── ExceptionHandlingMiddleware.cs
```

### Best Practices

1. Use async/await for all I/O operations
2. Implement proper exception handling
3. Use dependency injection
4. Follow SOLID principles
5. Write unit tests for business logic
6. Use proper logging
7. Implement proper validation

## Deployment

### Production Deployment

1. Configure production environment
2. Set up SSL certificates
3. Configure database
4. Set up CI/CD pipeline
5. Configure monitoring and logging

### Environment Variables

- ASPNETCORE_ENVIRONMENT
- ConnectionStrings\_\_DefaultConnection
- JwtSettings\_\_SecretKey
- AzureAd\_\_ClientId
- AzureAd\_\_TenantId

## Monitoring and Logging

### Application Insights

- Performance monitoring
- Error tracking
- Usage analytics
- Custom events

### Logging

- Structured logging
- Log levels
- Log storage
- Log rotation

## API Documentation

### Swagger Integration

- API documentation
- Request/Response examples
- Authentication requirements
- API versioning

## Testing

### Unit Testing

- Service layer testing
- Controller testing
- Repository testing

### Integration Testing

- API endpoint testing
- Database integration testing
- External service integration testing

## Performance Optimization

### Caching

- Response caching
- Memory caching
- Distributed caching

### Database Optimization

- Index optimization
- Query optimization
- Connection pooling

## Backup and Recovery

### Database Backup

- Automated backups
- Point-in-time recovery
- Backup verification

### Disaster Recovery

- Recovery procedures
- Data restoration
- Service continuity

## Environment Configuration

### Development Settings (appsettings.Development.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=Your-Server;Database=WhiteLableDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "MasterConnection": "Server=Your-Server;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere123!@asdf$%^&*yt4-making-it-longer-so-it-works",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "ExpiryInMinutes": 1000
  },
  "Authentication": {
    "Google": {
      "ClientId": "" // Configure your Google Client ID
    },
    "Microsoft": {
      "Instance": "https://login.microsoftonline.com/",
      "ClientId": "", // Configure your Microsoft Client ID
      "TenantId": "", // Configure your Microsoft Tenant ID
      "Scopes": {
        "Read": ["Expense.Read", "Expense.Write"],
        "Write": ["Expense.Write"]
      }
    }
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-gmail@gmail.com",
    "SmtpPassword": "", // Configure your SMTP password
    "FromEmail": "your-gmail@gmail.com",
    "FromName": "WhiteLabel"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "FrontendUrl": "http://localhost:4200",
  "AllowedHosts": "*",
  "AllowedOrigins": "http://localhost:4200"
}
```

### Required Environment Variables

1. Database Configuration:

   - DefaultConnection: Main database connection string
   - MasterConnection: Master database connection string

2. JWT Configuration:

   - Jwt:Key: Secret key for JWT token generation
   - Jwt:Issuer: Token issuer
   - Jwt:Audience: Token audience
   - Jwt:ExpiryInMinutes: Token expiration time

3. Authentication:

   - Google:ClientId: Google OAuth client ID
   - Microsoft:ClientId: Microsoft OAuth client ID
   - Microsoft:TenantId: Microsoft tenant ID

4. Email Configuration:

   - EmailSettings:SmtpServer: SMTP server address
   - EmailSettings:SmtpPort: SMTP port
   - EmailSettings:SmtpUsername: SMTP username
   - EmailSettings:SmtpPassword: SMTP password
   - EmailSettings:FromEmail: Sender email
   - EmailSettings:FromName: Sender name

5. CORS Configuration:
   - FrontendUrl: Frontend application URL
   - AllowedOrigins: Allowed CORS origins

### Security Considerations

1. JWT Configuration:

   - Use a strong, unique secret key
   - Set appropriate token expiration time
   - Configure proper issuer and audience

2. Authentication:

   - Configure OAuth providers with proper redirect URIs
   - Set up appropriate scopes for Microsoft authentication
   - Secure SMTP credentials

3. Database:

   - Use trusted connections
   - Enable SSL/TLS for database connections
   - Use appropriate database permissions

4. CORS:
   - Limit allowed origins to specific domains
   - Configure appropriate CORS policies
   - Use secure protocols (HTTPS)
