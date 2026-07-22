---
name: improve-codebase-architecture
description: Scan the codebase for architectural deepening opportunities — refactors that turn shallow modules into deep ones, improving testability and locality. Presents a candidate list, then walks through the chosen candidate. Read-only during exploration; writes only a report to the OS temp directory.
disable-model-invocation: true
---

# Improve codebase architecture

Surface **deepening opportunities**: refactors where the interface is nearly as wide as the implementation, or where logic leaks across a seam that should be tight. The aim is testability and clarity — not novelty.

## Design vocabulary (use these exact terms — no substitutes)

- **Module** — a unit of code with an interface and an implementation. Not "component," "service," or "layer."
- **Interface** — the surface a caller sees. Not "API" or "signature."
- **Depth** — implementation complexity minus interface complexity. Deep modules hide a lot behind a small surface. Shallow modules expose almost as much as they hide.
- **Seam** — the boundary where two modules meet. Not "boundary" or "edge."
- **Adapter** — an implementation of a seam. One adapter is a hypothetical seam; two (e.g. prod HTTP + in-memory for tests) proves the seam earns its keep.
- **Leverage** — one interface serving many call sites.
- **Locality** — related complexity concentrated in one place; a bug in behaviour X lives in module X.

**Deletion test**: for anything suspected of being shallow, ask "if I deleted this module, would complexity *concentrate* elsewhere (good — it was pulling its weight) or just *move* (bad — it was a shim)?"

## Process

### 1. Scope

Deepening pays off in hot spots. Look at recently changed code first.

- If the user named a target (a module, subsystem, pain point) — take it, skip inference.
- Otherwise, read `git log --oneline -n 50` (widen if needed). Files that keep coming up get priority.

Then read the project's authoritative docs:

- `CLAUDE.md` — operational conventions. Everything you propose must fit these.
- `PLAN.md` — roadmap and rationale. Don't propose refactors that fight the current phase.
- `DECISIONS.md` — non-obvious architectural choices already made. **Do not re-litigate a decision here** unless the friction is concrete enough to warrant reopening the decision — and if you do, say so explicitly.

If any of these files are missing, note it and continue with what exists.

### 2. Explore

Use the `Explore` agent (`Agent` tool with `subagent_type=Explore`) to walk the codebase. Do not follow a rigid checklist — look for friction:

- Where does understanding one concept require jumping between many small files?
- Which modules have an interface nearly as wide as the implementation?
- Where has logic been extracted "for testability" but the real bugs live in how it's *called*?
- Where do tightly-coupled modules leak state or types across their seam?
- What's untested — or tested via awkward setup that the current interface forces?

Apply the deletion test to every suspect.

### 3. Report — Markdown, to temp dir, never in the repo

Write a Markdown report to the OS temp directory. Resolve the path from `$TMPDIR`, `$TEMP`, or `%TEMP%`, falling back to `/tmp`. Name: `architecture-review-<yyyymmdd-hhmmss>.md`. **Do not** write into the working tree.

Report structure:

```
# Architecture review — <repo>
<date>

## Legend
<short definitions of Strong / Worth exploring / Speculative>

## Candidates

### 1. <Title — names the deepening, e.g. "Collapse the Transaction write pipeline">
- **Strength:** Strong | Worth exploring | Speculative
- **Files:** `path/to/one.cs`, `path/to/two.cs`
- **Problem:** <one sentence — what hurts today>
- **Solution:** <one sentence — what changes>
- **Before / After:** <small ASCII or Mermaid diagram>
- **Wins:**
  - <≤6 words, glossary terms — e.g. "locality: bugs concentrate in one module">
  - <e.g. "deletes 3 shallow wrappers">
  - <e.g. "interface shrinks; tests hit one method">
- **Conflict with DECISIONS.md?** <flag + one-line justification if yes; omit otherwise>

### 2. ...

## Top recommendation

<one paragraph — which candidate to tackle first, and why>
```

Print the absolute path of the report to the user. Do not open it automatically.

**Do not**:
- Write HTML with CDN scripts (Tailwind CDN, Mermaid CDN, or anything else that fetches at open time).
- Write files anywhere in the working tree.
- Depend on external skills (`/codebase-design`, `/grilling`, `/domain-modeling`) — they are not present in this environment.

### 4. Walk the chosen candidate

Once the user picks a candidate, work through it interactively:

- What are the real constraints? (deployment, in-flight work, `DECISIONS.md`)
- What is the shape of the deepened module — its interface, what sits behind it?
- Which tests survive the refactor as-is? Which get simpler? Which get deleted?
- What's the smallest first commit that starts the deepening?

If the user rejects the candidate with a **durable** reason (one a future review would need to avoid re-suggesting the same thing), offer: *"Should I record this in DECISIONS.md so future architecture reviews don't re-suggest it?"* Skip ephemeral reasons ("not worth it right now") and self-evident ones.

## Style

- Plain English. No hedging, no throat-clearing.
- Use the glossary vocabulary exactly. If a term isn't in the glossary, reach for one that is before inventing a new one.
- One sentence per Problem / Solution. If a diagram needs a paragraph to explain, redraw the diagram.
- **Wins bullets** name the gain in glossary terms: *"locality: bugs concentrate here"*, *"leverage: one interface, N call sites"*, *"deletes 4 shallow wrappers"*. Do **not** write *"cleaner code"*, *"easier to maintain"*, or *"more scalable"* — those aren't in the glossary and don't earn their place.

## Do not

- Do not scope-creep candidates into whole-repo rewrites. Each candidate should be independently shippable (a few hundred diff lines — matches this project's per-task target in `CLAUDE.md`).
- Do not propose refactors that contradict `DECISIONS.md` without explicitly flagging and justifying the reopening.
- Do not touch production code during the review — this skill is read-only until the user commits to a candidate.
