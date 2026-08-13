---
paths:
  - "src/**/*.cs"
  - "tests/**/*.cs"
---

# C# Code Style

- Target .NET 10, C# 14. Prefer modern syntax over older equivalents: primary constructors,
  collection expressions (`[]`), required members, file-scoped namespaces, raw string literals,
  plus C# 14 features: `field`-backed properties, extension members (`extension` blocks for
  properties/static members/operators), null-conditional assignment (`obj?.Prop = x`), partial
  constructors/events, implicit `Span<T>`/`ReadOnlySpan<T>` conversions.
- Parameter wrapping: any method/constructor/record declaration or call site with **2 or more**
  parameters always puts every parameter on its own line, indented one level — never on the
  declaration line, regardless of whether it would fit on one line. No exceptions, including
  operator overloads and one-line expression bodies. A single parameter may stay inline.
  ```csharp
  public async Task<bool> Handle(
      CreateOrderCommand message,
      CancellationToken cancellationToken)
  {
      var address = new Address(
          message.Street,
          message.City,
          message.State,
          message.Country,
          message.ZipCode);
  }
  ```
- New GUIDs: `Guid.CreateVersion7()`, never `Guid.NewGuid()` (v4) — applies everywhere.
- SOLID, especially Single Responsibility: one reason to change per class/method.
- Length limits: 120 chars/line, ~30-40 lines/method body, ~200-300 lines/class. Too many
  parameters (>4-5) or deeply nested `if`/`switch` → extract or use a pattern (Strategy, Command).
- High cohesion, low coupling: group tightly related code in one file/folder; minimize coupling
  between unrelated components; no dependency against the architecture direction.
- Clean Code + YAGNI: no over-engineering, no unused abstractions/features.
- Comments: short and plain, one line where possible, and only when the code can't say it itself
  — a non-obvious *why*, a gotcha, a `ponytail:`/`TODO` marker. Never restate what the code does,
  never write architecture rationale or decisions discussed in chat (those belong in
  `.claude/rules/*.md`), never leave template-generated comments. No XML doc comments unless the
  assembly is a published library.
