---
description: "Review code for SOLID principles and Clean Architecture compliance"
allowed-tools: Read, Grep, Glob
---

Please review the following code or file for:

## SOLID Principles Check
1. **Single Responsibility**: Does each class have only one reason to change?
2. **Open/Closed**: Can we extend without modifying existing code?
3. **Liskov Substitution**: Can derived types substitute base types?
4. **Interface Segregation**: Are interfaces small and focused?
5. **Dependency Inversion**: Do high-level modules depend on abstractions?

## Clean Architecture Check
1. **Layer Dependencies**: Do dependencies point inward only?
2. **Domain Purity**: Is the domain layer free of infrastructure concerns?
3. **Use Case Isolation**: Is each use case in its own handler?
4. **Repository Pattern**: One repository per aggregate root?

## Code Quality Check
1. **Naming**: Are names clear and intention-revealing?
2. **Error Handling**: Using Result<T> pattern, not exceptions for flow?
3. **Async**: Async all the way, no blocking calls?
4. **Testing**: Is the code testable with proper DI?

$ARGUMENTS
