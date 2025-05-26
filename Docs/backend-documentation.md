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

### Tenant Management

```
GET /api/tenants
POST /api/tenants
PUT /api/tenants/{id}
DELETE /api/tenants/{id}
GET /api/tenants/{id}
```

### CRM Features

```
GET /api/customers
POST /api/customers
PUT /api/customers/{id}
DELETE /api/customers/{id}
GET /api/customers/{id}
```

## Database Schema

### Tenant

```sql
CREATE TABLE Tenants (
    Id INT PRIMARY KEY,
    Name NVARCHAR(100),
    Domain NVARCHAR(100),
    IsActive BIT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME
)
```

### Customer

```sql
CREATE TABLE Customers (
    Id INT PRIMARY KEY,
    TenantId INT,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
)
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
