---
paths:
  - "src/**/*.cs"
---

# Architecture: Onion + DDD + CQRS

DDD tactical rules: `.claude/rules/domain-model.md`. This file is decisions/rationale, not a
directory map — once a feature has real code (e.g. `TodoLists`), follow its actual folder/file
pattern for new features instead of a spec here.

- Dependency direction: `Api → Infrastructure → Application → Domain → SharedKernel`. Domain never
  references Infrastructure/Application. SharedKernel depends on nothing and knows no aggregate.
  Application defines most ports (`IApplicationDbContext`, client ports, `IEventBus`), Infrastructure
  implements them — except repository contracts, which live in Domain: `domain-model.md`.
- Minimal API, not MVC controllers. Endpoints auto-register via an `IEndpointGroup` reflection
  scan — no `Program.cs` edit per feature. Auth is declared explicitly per endpoint, never implied
  by folder location. Three trust levels exist: public (no auth), app (JWT user), integration
  (same-platform service calling in, client-credentials/API-key, not public).
- CQRS via the `Mediator` library (martinothamar/Mediator, MIT) — not MediatR, whose license went
  commercial in 2025.
- Exactly one repository per aggregate root, never per table. It's the only write channel; commit
  is a cross-cutting concern handled by `UnitOfWorkBehavior` (Mediator pipeline) after the handler
  returns, not by the handler itself — same hook point future cross-cutting concerns (audit log,
  etc.) attach to. Reads never go through a repository — `IReadApplicationDbContext` bypasses
  aggregates freely for CQRS projections.
- Business errors (validation, not found, conflict) → `Result`/`Result<T>` (SharedKernel), carrying
  an `Error`. Never throw for these; exceptions are reserved for domain invariant violations that
  should never happen once Application has validated.
- Domain events are in-process, dispatched from the `SaveChanges` interceptor in the same
  transaction. Integration events cross a service boundary (RabbitMQ + MassTransit) — don't
  conflate the two; naming/handler-placement rules for domain events: `domain-model.md`.
- Outbox and idempotency both piggyback on that same `SaveChanges` transaction — the point is
  atomicity with the business write, not a separately committed side effect.
