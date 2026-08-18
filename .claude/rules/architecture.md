---
paths:
  - "src/**/*.cs"
---

# Architecture: Onion + DDD + CQRS

Layering is `Api → Infrastructure → Application → Domain → SharedKernel`, enforced by project
references. DDD tactical rules: `.claude/rules/domain-model.md`. Follow the folder/file pattern of a
feature that already has real code (e.g. `TodoLists`) rather than a spec here.

- Application defines ports, Infrastructure implements them — except repository contracts, which
  live in Domain: `domain-model.md`. SharedKernel knows no aggregate.
- Auth is declared explicitly per endpoint, never implied by folder location.
- Business errors (validation, not found, conflict) → `Result`/`Result<T>` (SharedKernel), carrying
  an `Error`. Never throw for these; exceptions are reserved for domain invariant violations that
  should never happen once Application has validated.
- A domain event is never serialized and never leaves the transaction; anything crossing the process
  boundary is an integration event.
- Never gate outbox dispatch on an execute-mode flag — if durable *in-process* work is needed, give
  it its own notification type.
- Integration events stay plain DTOs: no id or timestamp fields, identity lives in the envelope.
- Delivery is at-least-once and unordered; consumers must dedupe.
