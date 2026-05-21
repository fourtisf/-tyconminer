# Gold Mine Tycoon

Mobile idle tycoon game (Unity 6 LTS, URP). Isometric low-poly, Play Store launch.
Visual benchmark: **Lumber Inc**. See the Visual Brief & Production Guide for the full spec.

## Status

Project scaffold — folder structure, core gameplay systems (P0), and P1 stubs are in
place. Asset import, scenes, prefabs, and SDK wiring are still TODO.

## Project layout (`Assets/_Project/`)

```
Scripts/
  Core/          GameManager, SaveSystem, SaveData, CurrencyManager, EventBus, CurrencyType
  Gameplay/      Station, ProductionSystem, ManagerService, IdleCalculator, Worker, TutorialManager
  UI/            UIManager
  Monetization/  IAPManager, AdsManager, AnalyticsManager   (SDK-free stubs)
  ScriptableObjects/  StationData, ManagerData, WorkerData, LayerConfig
Resources/       Stations/ Managers/ Workers/ Layers/  (drop authored .asset configs here)
Art/ Animations/ Audio/ Prefabs/ Scenes/ Plugins/
```

## Architecture notes

- **Production math is plain C#, not MonoBehaviours.** `Station`, `ProductionSystem`,
  and `IdleCalculator` run headlessly so offline earnings and unit tests are
  deterministic. MonoBehaviour views mirror this state for visuals.
- **EventBus** decouples gameplay from UI — no direct references from systems to screens.
- **Production chain:** Mine → Crusher → Smelter → Shipping. A station auto-produces only
  when a manager is hired for it; otherwise the player taps. Shipping converts items to cash.
- **Config-driven balancing:** costs/output/capacity curves live on `StationData`
  ScriptableObjects loaded from `Resources/Stations`, so designers tune numbers without code.
- **Save:** AES-encrypted blob under `persistentDataPath`. The key only deters casual save
  editing — IAP-granted currency must still be validated server-side.

## What's stubbed (P1, needs SDK import)

- `IAPManager` — wire Unity IAP package; product IDs already match the brief / Play Console.
- `AdsManager` — wire Google Mobile Ads SDK; rewarded + interstitial only, no banners.
- `AnalyticsManager` — wire Firebase Analytics; event methods already match the brief.

Stubs compile and log without the SDKs so the project opens cleanly before SDK setup.

## Getting started

1. Open with **Unity 6 LTS** (URP). Package Manager will resolve `Packages/manifest.json`.
2. Add the registry/Asset Store deps noted in `manifest.json` (Firebase, AdMob, DOTween).
3. Author `StationData` / `ManagerData` assets under `Resources/` (right-click →
   Create → GoldMineTycoon).
4. Build the Main scene, drop a `GameManager` into it, and assign UI screens to `UIManager`.

Target build: Android API 24+, ARM64, IL2CPP, ASTC, <80MB initial.
