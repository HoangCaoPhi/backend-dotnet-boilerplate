---
paths:
  - "src/**/*.cs"
---

# Architecture: Onion + DDD + CQRS

Layering is `Api → Infrastructure → Application → Domain → SharedKernel`, enforced by project
references.

- Application defines ports, Infrastructure implements them — except repository contracts, which
  live in Domain.
- Auth is declared explicitly per endpoint, never implied by folder location.
- The outbox carries one row per destination: an `IIntegrationEvent` leaves the process, an
  `IEventualCommand` stays in it. The marker interface *is* the destination — never gate dispatch
  on a mode flag or a discriminator column.
- Enrich an integration event where it is enqueued, not where it is relayed — the relay must stay a
  dumb loop.
- Integration events stay plain DTOs: no id or timestamp fields, identity lives in the envelope.
- Delivery is at-least-once and unordered; consumers must dedupe.
