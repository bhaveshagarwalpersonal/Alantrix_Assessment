# DEVLOG.md

## Key decisions

- **Persistence stores a flat snapshot, not runtime references.** `SaveData` only holds primitives and a
  `List<int>` of matched card ids — never a `CardRuntimeState`/`CardView`. This keeps `JsonUtility`
  serialization simple and means load doesn't need to "revive" old objects, just rebuild the board fresh
  and re-apply matched status.

- **Load rebuilds the board from the saved seed instead of saving card positions.** Since `BoardGenerator`
  is deterministic for a given `(rows, columns, seed)`, saving the seed is enough to reproduce the exact
  same card layout — no need to persist per-cell data. This only holds because the shuffle uses a locally
  seeded `System.Random`; I checked that explicitly before relying on it.

- **Visual restore is explicit, not implicit.** A restored `CardView` doesn't "inherit" its sprite or
  rotation from anywhere — `LoadGame()` has to hand both to it directly. This was the source of the two
  bugs described below, and fixing it by making the restore path explicit (rather than hoping shared state
  carries over) is the approach I kept.

## Approach I tried and abandoned

I initially tried forcing a restored matched card's rotation directly via
`transform.localRotation = Quaternion.Euler(0, 180, 0)` in `LoadGame()`, bypassing `CardAnimator` entirely,
on the assumption that the animator was "just a helper" I could skip for a static restore. I abandoned this
because `CardAnimator` keeps its own rotation state privately (inside its flip coroutine) — setting the
transform from outside doesn't tell the animator anything changed, so the next time that card was flipped
(or even just inspected), the animator's internal assumption about "where the rotation currently is" was
wrong, producing a visibly incorrect flip. I replaced it with `CardAnimator.SnapTo(showFront)`, which stops
any in-flight coroutine and sets rotation through the animator itself, so there's exactly one place that
owns rotation state instead of two that can disagree.

## Known limitation left as-is

`SaveGame()` only persists cards in the `Matched` state. If you save mid-resolution (two cards face-up but
not yet resolved), that in-progress reveal is silently dropped on load and those cards come back FaceDown.
I considered persisting `pendingSelection`/in-flight `RevealOperation`s too, but decided it added
meaningful complexity (needing to serialize *and* replay an in-flight coroutine-based animation) for a
case that's a minor UX inconvenience rather than a correctness bug, so I left it as a known limitation
rather than solving it in this pass.
