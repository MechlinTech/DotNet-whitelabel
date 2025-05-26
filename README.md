# White Label Solution

A modern, scalable white label solution built with Angular and .NET Core, featuring a robust frontend and backend architecture.

## 🚀 Technology Stack

### Frontend

- Angular 19.1.0
- TailwindCSS 4.1.7
- Azure MSAL Authentication
- Social Login Integration
- Progressive Web App (PWA) Support
- RxJS for Reactive Programming

### Backend

- .NET Core
- Entity Framework Core
- RESTful API Architecture
- Authentication & Authorization
- Database Migrations
- DTO Pattern Implementation
- Added Tenant CURD
- Added basic CRM
- Feature wise controller and services

## 📋 Prerequisites

- Node.js (Latest LTS version)
- .NET Core SDK
- Angular CLI
- SQL Server
## 🛠️ Installation

### Frontend Setup

```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Enviornment file
Setup the enviornment.development.ts

# Start development server
ng serve
```

### Backend Setup

```bash
# Install IDE
Install Visual Studio

# Enviornment File
Setup the appsettings.development.json

# Starting server
Run It from the top
```

## 🔧 Configuration

### Frontend Configuration

- Environment variables are managed through Angular's environment files
- Azure AD configuration in `app.module.ts`
- TailwindCSS configuration in `.postcssrc.json`

### Backend Configuration

- Database connection strings in `appsettings.json`
- Authentication settings in `Program.cs`
- Environment-specific settings in `appsettings.Development.json`

## 📁 Project Structure

### Frontend

```
frontend/
├── src/
│   ├── app/
│   ├── assets/
│   └── environments/
├── public/
└── angular.json
```

### Backend

```
backend/
├── Controllers/
├── Models/
├── DTOs/
├── Services/
├── DBContext/
├── Middleware/
└── Migrations/
```

## 🔐 Authentication

The application supports multiple authentication methods:

- Azure AD Authentication
- Social Login Integration
- JWT Token-based Authentication

## 🚀 Deployment

### Frontend Deployment

```bash
# Build for production
cd frontend && ng build --configuration production

# The build artifacts will be stored in the `dist/` directory
```

### Backend Deployment

```bash
# Publish the application
Running the App within Visual studio
```


## 📞 Support

For support, please contact the development team or raise an issue in the repository.
