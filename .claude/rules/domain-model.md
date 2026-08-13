---
paths:
  - "src/**/*.cs"
---

# Domain Model (DDD tactical patterns)

## Entity & Aggregate

- Entity identity is defined by Id, not by attributes.
- Domain entities carry both behavior (methods) and data — avoid anemic domain models
  (getter/setter bags) except for genuinely simple CRUD aggregates.
- No public setters — every mutation goes through an explicit method named after the ubiquitous
  language, never assigned directly.
- Child collections are exposed as `IReadOnlyCollection<T>` backed by a private field; mutated
  only through aggregate root/entity methods, never directly by callers.
- Aggregate boundary is defined by transactional consistency needs, not by convenience grouping.
- Aggregate root is the sole entry point and consistency guardian — no creating/mutating a child
  entity from outside the aggregate.
- No direct navigation between aggregates — cross-aggregate references are foreign-key IDs only,
  no EF navigation property crossing an aggregate boundary.
- Mark aggregate roots with the `IAggregateRoot` marker interface.
- Domain project has no hard reference to EF Core or any ORM (Persistence Ignorance).

## Value Object

- No identity; immutable; equality by value (`GetEqualityComponents`), not by reference — overload
  `==`/`!=`.
- Use `private set` only if deserialization requires it (message queue, EF Core); otherwise never
  mutate after construction.
- Persist via EF Core Owned Entity Types (`OwnsOne`/`[Owned]`), no separate Id column.

## Enum vs Enumeration

- Plain `enum` for a small, closed, static set of values.
- Using `enum` to drive complex control flow/behavior is a smell — model it as an `Enumeration`
  class instead when it needs behavior.

## Invariants & validation

- Invariants are enforced by the entity itself (constructor/methods) — an entity must never exist
  in an invalid state. Throw on violation (`?? throw new ArgumentNullException(...)`), and never
  leave a partial mutation applied before throwing.
- This is separate from `Result<T>` (SharedKernel, used from Application): domain exceptions guard
  invariants (should-never-happen), `Result<T>` carries expected business outcomes (validation
  failure, not found, conflict) back to the caller as an `Error` — see architecture.md.
- FluentValidation checks the command DTO at the field level (`ValidationBehavior` in the Mediator
  pipeline); domain invariants are checked again inside the entity — both layers validate, for
  different reasons.
- Client-side validation is UX only; the server (domain model) is the single source of truth and
  must always re-validate independently.

## CQRS: query bypasses the aggregate

- Aggregate boundaries apply to writes only. Queries bypass them freely — a query DTO can join
  data across multiple aggregates/tables via `IReadApplicationDbContext` LINQ projection.

## Domain events

- Naming: past-tense verb (`OrderStartedDomainEvent`), immutable data holder. Folder placement:
  see architecture.md.
- Dispatched from the `SaveChanges` interceptor (deferred: raised on the entity first, dispatched
  around commit) — keeps the side effect in the same transaction.
- Handlers live in the Application layer, never in Domain.
- One command → exactly one handler. One domain event → zero or more handlers. When an event has
  2+ handlers, name each by the action it performs (`<Action>When<Event>DomainEventHandler`), not
  by the event name (that would collide) — a single-handler event may keep the short
  `<Event>DomainEventHandler` form.

## Repository — one per aggregate root

- Exactly one repository per aggregate root — never a repository per table/child entity.
- Repository is the **only** write channel: a command handler loads the aggregate through
  `I<Aggregate>Repository`, mutates it via its own methods (never a child entity directly), and
  does **not** call `SaveChangesAsync` itself — `UnitOfWorkBehavior` (Mediator pipeline) commits
  after the handler returns, only when the result is a success.
- Repository contracts live in **Domain**, not Application (verified against dotnet/eShop:
  `IRepository<T>`/`IOrderRepository` are both under `Ordering.Domain`) — the contract is part of
  the ubiquitous language, only the EF Core implementation is an Infrastructure concern. Generic
  `IRepository<T> where T : IAggregateRoot` base (`AddAsync`) in `Domain/Common/`; each aggregate
  adds `I<Aggregate>Repository : IRepository<Aggregate>` next to it with its own load methods (e.g.
  `GetByIdWithItemsAsync`).
- Query bypasses the repository entirely — reads go through `IReadApplicationDbContext` (CQRS),
  never through the repository.
