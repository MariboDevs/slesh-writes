# SleshWrites

A personal tech blog platform built with Clean Architecture principles, featuring a 1980s arcade cabinet aesthetic.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-19-DD0031?logo=angular)
![Azure](https://img.shields.io/badge/Azure-Hosted-0078D4?logo=microsoftazure)
![License](https://img.shields.io/badge/License-MIT-green)

## Overview

SleshWrites is a tech blog where I share insights about software development, architecture, and emerging technologies. The first topic explores **mono repos and how coding agents make them better**.

### Features

- Clean Architecture with SOLID principles
- CQRS pattern using MediatR
- Azure AD B2C authentication
- Retro 1980s arcade cabinet theme (neon colors, pixel fonts, CRT effects)
- Server-side rendering for SEO
- Responsive design

## Tech Stack

### Backend
- .NET 9 / ASP.NET Core
- Entity Framework Core 9
- MediatR (CQRS)
- FluentValidation
- Azure SQL Database

### Frontend
- Angular 19
- NgRx (signals-based)
- TailwindCSS
- Angular Material

### Infrastructure
- Azure App Service
- Azure Static Web Apps
- Azure Blob Storage
- Azure Key Vault
- GitHub Actions CI/CD

## Architecture

```
┌─────────────────────────────────────┐
│  Angular SPA (Static Web Apps)      │
└──────────────┬──────────────────────┘
               │ REST API
┌──────────────▼──────────────────────┐
│  ASP.NET Core API (App Service)     │
├─────────────────────────────────────┤
│  Application Layer (MediatR/CQRS)   │
├─────────────────────────────────────┤
│  Infrastructure (EF Core, Azure)    │
├─────────────────────────────────────┤
│  Domain Layer (Entities, Logic)     │
└─────────────────────────────────────┘
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- [Docker](https://www.docker.com/) (optional, for local development)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/MariboDevs/slesh-writes.git
   cd slesh-writes
   ```

2. **Backend Setup**
   ```bash
   # Restore packages
   dotnet restore

   # Apply database migrations (requires SQL Server)
   dotnet ef database update --project src/SleshWrites.Infrastructure --startup-project src/SleshWrites.API

   # Run the API
   dotnet run --project src/SleshWrites.API
   ```

3. **Frontend Setup**
   ```bash
   cd src/SleshWrites.Web

   # Install dependencies
   npm install

   # Run development server
   ng serve
   ```

4. **Access the application**
   - API: https://localhost:5001
   - Web: http://localhost:4200

### Using Docker

```bash
docker-compose up -d
```

## Project Structure

```
slesh-writes/
├── src/
│   ├── SleshWrites.Domain/         # Core business logic
│   ├── SleshWrites.Application/    # CQRS handlers, DTOs
│   ├── SleshWrites.Infrastructure/ # EF Core, Azure services
│   ├── SleshWrites.API/            # Web API endpoints
│   └── SleshWrites.Web/            # Angular frontend
├── tests/
│   ├── SleshWrites.Domain.Tests/
│   ├── SleshWrites.Application.Tests/
│   └── SleshWrites.API.Tests/
├── infra/                          # Bicep IaC templates
└── .github/workflows/              # CI/CD pipelines
```

## Development with Claude Code

This project is configured for optimal use with [Claude Code](https://claude.ai/code).

### Available Commands

- `/review` - Review code for SOLID principles and Clean Architecture
- `/test` - Run tests and analyze results
- `/deploy` - Deployment guide and commands

### Configuration

See `.claude/settings.json` for project-specific Claude Code settings including:
- File permissions
- Auto-formatting hooks
- Code review guidelines

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Slesh** - [LinkedIn](https://linkedin.com/in/slesh) | [GitHub](https://github.com/siphomaribo)

---

Built with Clean Architecture principles and a love for retro gaming aesthetics.
