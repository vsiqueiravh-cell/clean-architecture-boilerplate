# Architecture

This boilerplate is a production-oriented starting point for enterprise APIs.

```mermaid
flowchart LR
    Client["API Consumer"] --> Api["CleanArchitecture.Api"]
    Api --> Application["CleanArchitecture.Application"]
    Application --> Domain["CleanArchitecture.Domain"]
    Application --> Shared["CleanArchitecture.Shared"]
    Infrastructure["CleanArchitecture.Infrastructure"] --> Application
    Infrastructure --> Domain
    Infrastructure --> Db["PostgreSQL"]
```

## Dependency Rule

- `Domain` has no dependency on other layers.
- `Application` depends on `Domain` and `Shared`.
- `Infrastructure` implements Application abstractions.
- `Api` composes the application and exposes HTTP endpoints.

## Included Patterns

- CQRS with MediatR request handlers
- FluentValidation pipeline behavior
- EF Core DbContext behind an application abstraction
- JWT authentication and role-based authorization
- Auditable entities with current-user context
- Centralized exception handling with Problem Details
- Docker Compose for local Postgres
