---
paths:
  - "**/*.md"
---

# Doc / Explanation Style

- Short, bullet the main points — avoid long-winded writing, avoid rambling explanatory paragraphs.

## Writing `.claude/rules/*.md` files

- A rule earns its place only if breaking it is **silent**: the compiler doesn't catch it and reading
  the code doesn't reveal it. If a wrong package won't compile, or one existing file shows the
  pattern, there is no rule to write.
- Exclude: why a choice beat its alternatives, library comparisons and license history, credit to
  whoever inspired it, restatements of what the code plainly does, file/folder listings.
- Rationale, surveys and detailed specs → `temp/`, not a rule.
- Say it once. A rule already stated in another rule file is a pointer, not a copy.
