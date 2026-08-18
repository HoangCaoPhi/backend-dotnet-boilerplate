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
- Comments: default to none. Every comment is a small failure to express something in code, so only
  add one when the code truly cannot say it itself. Litmus test before writing one — is this
  explaining a fact about the world the code can't express (keep), or justifying why this design
  was chosen over an alternative (delete, it's PR/rule-doc material, not code)? If the comment
  would still make sense as a bullet in a PR description, it doesn't belong in the file.
  - Never: restate what the code does, explain *what* instead of *why*, justify a design decision
    or tradeoff discussed in chat (put that in `.claude/rules/*.md` instead, or nowhere), leave
    template-generated comments, or add a comment a future reader could derive by re-reading the
    two lines around it.
  - Only: a hidden constraint or invariant the code can't self-document, a workaround for a
    specific library/runtime quirk, a warning about a non-obvious consequence (e.g. "this test is
    slow", "this order matters because X"), or a `ponytail:`/`TODO` marker.
  - When one is warranted: one line, plain, no restating the code, no XML doc comments unless the
    assembly is a published library.
