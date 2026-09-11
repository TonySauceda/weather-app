# Agent Guidelines

## Project context

- This project is a .NET 10 ASP.NET Core Blazor Web App with interactive server components, nullable reference types, and implicit usings enabled.
- Preserve the existing framework, project settings, and conventions unless the requested work requires changing them.
- Apply these guidelines to all new and modified code. Refactor existing template code when relevant to the task; avoid unrelated rewrites.

## Git workflow

- Never work directly on `main`.
- Before editing files for a new task, create and switch to a dedicated branch using one of these patterns:
  - `fix/<brief-bug-description>` for bug fixes.
  - `feature/<brief-feature-description>` for new features and enhancements to existing functionality.
  - `task/<brief-task-description>` for small tasks that are neither features nor bug fixes, such as documentation, maintenance, or configuration updates.
- Use short, descriptive, lowercase, hyphen-separated branch names in English.
- Continue on an existing task branch when working on the same task.
- Never create a commit unless the user explicitly requests it. Do not push, merge, or rewrite history without authorization.
- Preserve the user's uncommitted changes and keep each change focused on the requested task.

## Architecture: vertical slices and CQRS

- Organize application behavior by feature and use case, using vertical slices rather than application-wide technical layers.
- Use `Features/<Feature>/<UseCase>/` for each slice. Keep its request, handler, response, validation, and other use-case-specific types together in separate files.
- Apply CQRS: commands change state; queries read data without changing business state. Keep command and query requests and handlers distinct.
- CQRS does not require separate databases, event sourcing, or additional projects. Add complexity only for a concrete requirement.
- Keep Blazor components and HTTP endpoints thin: handle presentation or transport concerns and dispatch application requests through the mediator.
- Keep business rules in the relevant handler or domain type, not in UI components or endpoints.
- Share code only when there is a demonstrated need. Keep shared infrastructure small and avoid coupling one slice to another slice's implementation.

## Custom mediator and dependency injection

- Implement a small, strongly typed custom mediator with explicit request and handler contracts. Do not introduce MediatR or another mediator library.
- Dispatch each request to its corresponding handler through the custom mediator. Support asynchronous operations and propagate `CancellationToken` through handlers and I/O calls.
- Use the built-in .NET dependency injection container when dependencies need composition or lifetime management. Prefer constructor injection and explicit registrations.
- Keep handler resolution inside the mediator; do not use service location in components or business logic.
- Add mediator pipeline behaviors or other extensions only when a concrete cross-cutting requirement justifies them.
- Do not introduce a service layer by default. Add a focused service only for a required integration, reusable capability, or responsibility that does not belong in a handler or domain type.
- Do not add interfaces, factories, or abstractions without a clear purpose. Never capture scoped dependencies in singletons.

## Persistence, when required

- Do not add a database or persistence packages until a feature requires persistence.
- When a database is needed, use SQL Server with Entity Framework Core and Code First migrations, using versions compatible with the project's target framework.
- Do not introduce repositories, generic repositories, or custom unit-of-work wrappers. Use EF Core directly from the relevant handlers; `DbContext` already supplies change tracking and unit-of-work behavior.
- Keep the `DbContext`, entity types, entity configurations, and migrations in individual files. Place shared database infrastructure in a focused location such as `Infrastructure/Persistence/`.
- Use `IEntityTypeConfiguration<T>` for nontrivial entity mappings and keep each configuration in its own file.
- For Blazor server execution, prefer `IDbContextFactory<TContext>` and create and dispose a context per operation. Do not hold a context for the lifetime of a circuit or share one across concurrent operations.
- Use asynchronous EF Core APIs with cancellation, project queries into the required response types, and use `AsNoTracking()` for read-only entity queries. Avoid unbounded results and N+1 queries.
- Define required constraints, indexes, relationships, and concurrency handling according to the use case. Use explicit transactions when multiple writes must succeed atomically beyond a single `SaveChangesAsync` call.
- Generate and review Code First migrations for schema changes. Keep migrations and the model snapshot under source control when commits are authorized; do not replace migrations with `EnsureCreated`.
- Do not rewrite migrations that have already been applied to shared environments. Review data-loss implications and deployment requirements before applying schema changes.
- Store connection strings and credentials in environment configuration or development user secrets, never in committed source files. Do not apply migrations to shared or production databases without explicit authorization.

## Individual files and Blazor conventions

- Use one declared type per file, including classes, records, interfaces, enums, requests, handlers, responses, validators, and entity configurations. Name each file after its type; do not group supporting types inside another type or file.
- Keep each Razor component in its own `.razor` file and its C# implementation in a matching `.razor.cs` partial class. Avoid inline `@code` blocks for new or modified component logic.
- Put component-specific styles and JavaScript in separate `.razor.css` and `.razor.js` files when needed. Do not create empty companion files.
- Respect framework-generated files and partial-type conventions; do not manually split or edit generated outputs.
- Follow Blazor lifecycle, render-mode, and prerendering requirements. Handle loading, empty, and error states, and dispose subscriptions and resources appropriately.
- Keep layouts, routing, and application composition in their established locations. Feature-specific UI may live with its slice when compatible with existing routing conventions.

## Engineering quality

- Use idiomatic C#, clear names, nullable annotations, and the simplest design that meets the requirement. Avoid speculative abstractions and unnecessary dependencies.
- Validate input at the appropriate boundary and enforce business invariants where the behavior is implemented.
- Use consistent error handling and structured logging. Do not swallow exceptions or expose secrets and internal exception details to users.
- Enforce authorization on the server for protected operations; hiding UI elements is not an authorization boundary.
- Prefer asynchronous I/O throughout; avoid blocking on tasks and unnecessary `Task.Run` calls for I/O.
- Keep changes scoped. Do not edit generated `bin/` or `obj/` outputs or upgrade unrelated packages.
- For behavior changes, add or update meaningful tests for relevant outcomes and failure paths. When SQL Server behavior matters, validate against SQL Server rather than assuming an in-memory provider behaves equivalently.
- Build the affected project and run relevant tests for code changes, following repository conventions and applicable skills. For documentation-only changes, review content and the diff; a build is not required.
- Report what changed, what was verified, and any remaining limitations. Never claim a check passed if it was not run.

## Installed .NET skills

- Use installed .NET skills that apply to the actual task and this project's stack. Read their `SKILL.md` instructions before applying them.
- Relevant skills include .NET development, ASP.NET Core, Blazor component authoring, input handling, data access, dependency coordination, prerendering, and JavaScript interop as needed.
- When persistence, diagnostics, builds, upgrades, or tests are involved, use the corresponding installed EF Core, diagnostic, MSBuild, migration, or testing skills. Read the applicable test-running skill before running .NET tests.
- Apply only relevant skills; do not add dependencies or change architecture merely because a skill is installed.
- If a relevant skill is unavailable, say so when it affects the work and proceed using the project conventions and official documentation. User instructions take precedence over skill guidance.
