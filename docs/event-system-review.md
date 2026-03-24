# Event System Review

Date: 2026-03-24

## Scope Reviewed

- `GameEvents` global save/load events.
- Domain event buses: `CombatEvents`, `TownEvents`, `NavalInteractionEvent`, `MissionEvents`.
- Mission task event wiring and post-combat event flow.

## What works well

1. Event wrappers consistently use null-safe invocation (`?.Invoke`) across the custom event buses.
2. Most MonoBehaviours that subscribe to `GameEvents` unsubscribe in `OnDestroy`/`OnDisable`, which is good hygiene.
3. `PostCombatFlowService` includes a fallback path to avoid a combat soft-lock when no UI listener is present.

## High-impact issues / likely bugs

### 1) `GameEvents.ClearEvents()` can silently break save/load listeners during runtime

`GameEvents.ClearEvents()` sets both static delegates to `null`.
This is called from scene-transfer and battle flow code.
If objects that previously subscribed are still alive (especially because several systems use `DontDestroyOnLoad` or the return scene is already loaded), they *will not automatically re-subscribe* and future saves/loads can stop updating important state.

Evidence paths:
- Clear implementation: `Assets/GerneralScripts/GameEvents.cs`
- Called from: `Assets/GerneralScripts/SceneTransfer.cs`, `Assets/GerneralScripts/BattleManagement/BattleManager.cs`

Suggestion:
- Avoid global `ClearEvents` in normal flow.
- Prefer lifecycle-based subscribe/unsubscribe only.
- If reset is needed for tests/dev tooling, gate it behind an explicit debug flag and call it only during a full system reset.

### 2) `CombatEvents.DefeatFleet` appears never raised

`DefeatNationsFleet` task instances subscribe to `CombatEvents.DefeatFleet`, but no production code invokes `CombatEvents.InvokeDefeatFleet(...)`.
This likely means these mission tasks cannot progress via normal gameplay.

Evidence paths:
- Subscriber: `Assets/GerneralScripts/MissionSystem/Tasks/Instances/DefeatNationsFleet.cs`
- Event declaration: `Assets/Combat/Scripts/CombatEvents.cs`
- No invocation usages found outside the event class.

Suggestion:
- Emit `CombatEvents.InvokeDefeatFleet(enemyFleet)` from the confirmed battle-result application point (single source of truth), not from multiple combat frontends.

### 3) `NavalInteractionEvent.AttackedFleet` appears never raised

`NationalityOpinionSystem` subscribes to `NavalInteractionEvent.AttackedFleet`, but there are no observed calls to `NavalInteractionEvent.InvokeAttackedFleet(...)`.
As a result, opinion penalties for player-initiated attacks may never occur.

Evidence paths:
- Subscriber: `Assets/GerneralScripts/Nation/NationalityOpinionSystem.cs`
- Event declaration/invoker: `Assets/MapMode/Scripts/NavalInteractionEvent.cs`
- No invoker usage found outside the event class.

Suggestion:
- Raise the event at the single point where a hostile encounter is confirmed.

## Medium-priority design improvements

### 4) Event bus naming and structure are inconsistent

Current buses mix naming styles and class semantics:
- `GameEvents` uses public static `Action` fields (not `event`).
- `CombatEvents`/`MissionEvents`/`TownEvents` use `event` and invoke wrappers.
- `NavalInteractionEvent` is non-static class containing static members.

Suggestion:
- Standardize on: `public static class XEvents` + `public static event Action<T> SomethingHappened` + `RaiseSomethingHappened(...)`.
- Keep direct delegate fields private; expose only `event` to prevent accidental reassignment from outside.

### 5) Mission task subscription duplication risk

`DefeatNationsFleet` subscribes in both constructor and `Initialize()`.
After load/initialize flows this can lead to duplicate handlers and double-count progress.

Suggestion:
- Subscribe in exactly one place.
- Add idempotent guards (e.g., local `_isSubscribed` flag) if re-initialization is expected.

## Low-priority clarity improvements

### 6) Remove unused event buses or integrate them

`MissionEvents` exists but mission state changes currently appear to be managed directly through `MissionSystem` and task callbacks.
If no system consumes `MissionEvents`, either wire it into mission transitions or remove it to reduce cognitive overhead.

### 7) Add lightweight event tracing in development builds

A small debug-only event logger (publisher + payload summary) would make it much easier to diagnose event-chain failures in live play sessions.

## Recommended implementation order

1. Fix missing event emissions (`CombatEvents`, `NavalInteractionEvent`).
2. Stop using `GameEvents.ClearEvents()` as a runtime cleanup mechanism.
3. Remove duplicate subscription patterns in mission tasks.
4. Normalize event conventions and naming.
5. Add debug tracing utilities.
