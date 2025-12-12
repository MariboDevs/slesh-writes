# SleshWrites Blog Platform

## Overview
Personal tech blog with Clean Architecture, .NET 9 backend, Angular 19 frontend.
Azure-hosted with Azure AD B2C authentication. Features a 1980s arcade cabinet theme
(Pac-Man, Space Invaders style with neon colors, pixel fonts, CRT scanlines).

## Key Commands

### Backend (.NET)
```bash
# Build
dotnet build src/SleshWrites.API

# Run API
dotnet run --project src/SleshWrites.API

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/SleshWrites.Domain.Tests

# Apply EF migrations
dotnet ef database update --project src/SleshWrites.Infrastructure --startup-project src/SleshWrites.API

# Add new migration
dotnet ef migrations add <MigrationName> --project src/SleshWrites.Infrastructure --startup-project src/SleshWrites.API
```

### Frontend (Angular)
```bash
# Install dependencies
cd src/SleshWrites.Web && npm install

# Run dev server
cd src/SleshWrites.Web && ng serve

# Build production
cd src/SleshWrites.Web && ng build --configuration production

# Run tests
cd src/SleshWrites.Web && ng test

# Run linting
cd src/SleshWrites.Web && ng lint
```

### Infrastructure
```bash
# Deploy Azure resources (requires Azure CLI)
az deployment group create --resource-group slesh-writes-rg --template-file infra/main.bicep --parameters infra/parameters/dev.bicepparam
```

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  SleshWrites.Web (Angular 19 SPA)                           │
│  Azure Static Web Apps                                       │
└────────────────────────┬────────────────────────────────────┘
                         │ REST API
┌────────────────────────▼────────────────────────────────────┐
│  SleshWrites.API (ASP.NET Core Minimal APIs)                │
│  Azure App Service                                           │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│  SleshWrites.Application (CQRS with MediatR)                │
│  Commands, Queries, Handlers, Validators, DTOs              │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│  SleshWrites.Infrastructure (EF Core, Azure Services)       │
│  Repositories, DbContext, External Services                  │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│  SleshWrites.Domain (Core Business Logic)                   │
│  Entities, Value Objects, Domain Events, Interfaces         │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure
- `src/SleshWrites.Domain/` - Core entities, no external dependencies
- `src/SleshWrites.Application/` - CQRS with MediatR, DTOs, validators
- `src/SleshWrites.Infrastructure/` - EF Core, Azure services implementation
- `src/SleshWrites.API/` - Minimal APIs, controllers for complex endpoints
- `src/SleshWrites.Web/` - Angular 19 with NgRx signals
- `tests/` - Unit and integration tests
- `infra/` - Bicep templates for Azure infrastructure

## SOLID Principles (Enforced)
- **Single Responsibility**: One class, one reason to change
- **Open/Closed**: Use strategies and interfaces for extension
- **Liskov Substitution**: Subtypes must be substitutable
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: Depend on abstractions

## Coding Conventions
- Use `record` for DTOs, Commands, and Queries
- Async all the way (no sync over async)
- Use `Result<T>` pattern for operation results (no exceptions for business logic)
- Repository per aggregate root
- One handler per command/query
- Validators using FluentValidation

## Domain Model

### BlogPost (Aggregate Root)
- Id, Title, Slug, Content (Markdown), Excerpt
- FeaturedImage, Status (Draft/Published/Archived)
- PublishedAt, CreatedAt, UpdatedAt
- AuthorId, Tags, Category, MetaData (SEO)

### Supporting Entities
- Category (Id, Name, Slug, Description, DisplayOrder)
- Tag (Id, Name, Slug)
- Author (Id, AzureAdB2CId, DisplayName, Bio, AvatarUrl, SocialLinks)

## Testing Strategy
- Domain: Unit tests for value objects, domain logic
- Application: Unit tests for handlers with mocked repositories
- API: Integration tests with WebApplicationFactory
- E2E: Playwright tests for critical user flows

## Environment Variables
- `ConnectionStrings__DefaultConnection` - Azure SQL connection string
- `AzureAdB2C__Instance` - B2C tenant URL
- `AzureAdB2C__ClientId` - Application client ID
- `AzureAdB2C__Domain` - B2C domain
- `Azure__StorageConnectionString` - Blob storage connection
