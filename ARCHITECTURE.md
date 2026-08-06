# ARCHITECTURE.md

## Layers

**Domain** (`EchoGrid.Domain`) — `CardDefinition`, `BoardDefinition`, `CardRuntimeState`, `BoardGenerator`.
Plain C#, no Unity/MonoBehaviour dependency. Owns the gameplay state machine
(`CardState`: FaceDown → Revealing → FaceUp → Resolving → Returning/Matched) and the deterministic
board layout (`BoardGenerator.Generate` shuffles with a locally-seeded `System.Random`, not the shared
`UnityEngine.Random`, so the same `(rows, columns, seed)` always reproduces the same board).

**Matching** (`EchoGrid.Matching`) — `MatchEvaluator`, `RevealOperation`.
Stateless evaluation over two `CardRuntimeState` instances. No side effects of its own; it only classifies
a pair as Match / Mismatch / Echo. `RevealOperation` guards against a pair being resolved twice.

**Scoring** (`EchoGrid.Scoring`) — `ScoreService`.
Tracks score/combo/matches and exposes `Restore()` so a loaded save can re-seed it without replaying gameplay.

**Persistence** (`EchoGrid.Persistence`) — `SaveData`, `SaveService`.
`SaveData` is a flat, serializable snapshot (rows/columns/seed/score/combo/matches/matched card ids) — it
never holds a reference to runtime objects. `SaveService` is a thin `PlayerPrefs` + `JsonUtility` wrapper.

**Presentation** (`EchoGrid.Presentation`) — `CardView`, `CardAnimator`, `BoardView`.
Unity-facing glue. These classes render Domain state; they must never *be* the source of truth for it.
`CardAnimator` owns the visual flip (rotation + timed front/back swap) and now exposes `SnapTo()` for
callers that need to set final visual state instantly (e.g. restoring from a save) without route through
the animated coroutine.

**Orchestration** (`EchoGrid.Core`) — `GameSessionController`.
Wires the layers together: reads input, drives `BoardGenerator`, listens to `CardView` clicks, resolves
`RevealOperation`s, updates `ScoreService`, and calls `SaveService`/`BoardGenerator` on save/load.

## Data flow

**Play:** `GameSessionController` (click) → `CardRuntimeState.TryReserve()` → `CardView.Reveal()` (animates,
sets sprite) → on second card, `RevealOperation` → `MatchEvaluator.Evaluate()` → `ScoreService` update →
`CardView.ShowMatched()` / `Hide()`.

**Save:** `GameSessionController.SaveGame()` reads current `rows/columns/seed`, `ScoreService`, and every
`CardView` in `Matched` state → builds a flat `SaveData` → `SaveService.Save()` (JSON via `PlayerPrefs`).

**Load:** `SaveService.Load()` → restore `rows/columns/seed` → `ScoreService.Restore()` →
`BoardGenerator.Generate()` with the saved seed (deterministic → same layout as when it was saved) →
`BuildBoard()` (fresh `CardRuntimeState`/`CardView` instances, all FaceDown) → for each saved matched
card id, `CardRuntimeState.MarkMatched()` + `CardView.ShowMatchedInstant(sprite)` (explicitly re-applies
the sprite and snaps the animator's rotation, since neither is implied by the domain state alone).

## Trade-off I'd revisit given more time

Right now, `GameSessionController` reconstructs matched-card visual state (sprite + rotation) manually in
`LoadGame()`, duplicating a little of what `Reveal()` does for live play. A cleaner version would have
`CardView` expose a single `ApplyState(CardRuntimeState state, Sprite sprite)` that both the live-play path
and the load path call, so there's exactly one place that knows "what a card should look like for a given
domain state" instead of two paths that can drift apart again the next time someone adds a new visual
detail to a card.
