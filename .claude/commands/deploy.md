---
description: "Deployment guide and commands for Azure"
allowed-tools: Bash, Read
---

# SleshWrites Deployment Guide

## Prerequisites
- Azure CLI installed and authenticated (`az login`)
- .NET 9 SDK installed
- Node.js 20+ installed
- GitHub CLI authenticated (`gh auth login`)

## Environment Setup

### 1. Create Azure Resources (First Time Only)
```bash
# Create resource group
az group create --name slesh-writes-rg --location eastus

# Deploy infrastructure
az deployment group create \
  --resource-group slesh-writes-rg \
  --template-file infra/main.bicep \
  --parameters infra/parameters/dev.bicepparam
```

### 2. Configure GitHub Secrets
Required secrets in GitHub repository settings:
- `AZURE_CREDENTIALS` - Service principal credentials
- `AZURE_SQL_CONNECTION_STRING` - Database connection string
- `AZURE_WEBAPP_PUBLISH_PROFILE` - App Service publish profile
- `AZURE_STATIC_WEB_APPS_API_TOKEN` - Static Web Apps deployment token

## Deployment Commands

### Deploy to Staging
```bash
# Via GitHub Actions (recommended)
gh workflow run cd-staging.yml

# Manual deployment
dotnet publish src/SleshWrites.API -c Release -o ./publish
az webapp deploy --resource-group slesh-writes-rg --name slesh-writes-api-staging --src-path ./publish
```

### Deploy to Production
```bash
# Via GitHub Actions with approval
gh workflow run cd-production.yml

# Manual slot swap (zero-downtime)
az webapp deployment slot swap \
  --resource-group slesh-writes-rg \
  --name slesh-writes-api \
  --slot staging \
  --target-slot production
```

### Database Migrations
```bash
# Apply pending migrations
dotnet ef database update \
  --project src/SleshWrites.Infrastructure \
  --startup-project src/SleshWrites.API \
  --connection "$(az webapp config connection-string list --name slesh-writes-api --resource-group slesh-writes-rg --query '[0].value' -o tsv)"
```

## Rollback
```bash
# Rollback to previous deployment
az webapp deployment slot swap \
  --resource-group slesh-writes-rg \
  --name slesh-writes-api \
  --slot staging \
  --target-slot production
```

$ARGUMENTS
