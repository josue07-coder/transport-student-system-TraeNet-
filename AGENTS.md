# Project Instructions

This is a .NET Clean Architecture backend.

Architecture:
- Domain contains entities, value objects, enums and domain exceptions.
- Application contains CQRS features, Commands, Queries, DTOs, Validators and repository interfaces.
- Infrastructure contains EF Core DbContext, configurations and repository implementations.
- API contains controllers, middleware and service extensions.

Rules:
- Use CQRS with MediatR.
- Do not use AutoMapper.
- Do not expose Domain entities directly from Controllers.
- Use ResponseDto classes for API output.
- Use Commands as input models.
- Use repository interfaces from Application.
- Implement repositories in Infrastructure.
- Keep EF Core configurations explicit.
- Avoid duplicate FK columns like SectorId1, GradeId1 or GuardianId1.
- Respect existing architecture.
- Do not modify unrelated modules.
- Run dotnet build after changes.