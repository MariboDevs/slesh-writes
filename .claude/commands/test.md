---
description: "Run tests and analyze results"
allowed-tools: Bash
---

Please run the tests for the SleshWrites project.

## Backend Tests (.NET)
```bash
dotnet test --verbosity normal
```

## Frontend Tests (Angular)
```bash
cd src/SleshWrites.Web && ng test --watch=false --browsers=ChromeHeadless
```

## Coverage Report
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Analyze the results and report:
1. Number of tests passed/failed
2. Any failing tests with details
3. Code coverage summary
4. Recommendations for improving coverage

$ARGUMENTS
