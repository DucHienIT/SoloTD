# AI Agent Context - SoloTD

## 0) Mandatory Update Rule

**IF THERE IS ANY CHANGE IN THE CORE GAME SYSTEM, THIS FILE MUST BE UPDATED IMMEDIATELY.**

## 1) Vision

Build a vertical mobile tower defense shooter:
- One or more heroes stand near the bottom (castle zone).
- Enemy waves spawn from top and move downward to attack the castle.
- Heroes auto-fire toward nearest/priority enemies.
- Gameplay feel: fast, dense enemy crowd, many hit numbers, skill bursts.

## 2) Core Gameplay Loop

1. Start wave.
2. Spawn enemies by lane/group from top area.
3. Enemies move down toward castle line.
4. Heroes auto-attack and cast skills.
5. Enemy reaches castle line -> castle HP reduced.
6. Clear wave -> rewards, upgrades, next wave.
7. Castle HP <= 0 -> fail.

## 3) Camera + Screen Rules (Portrait)

- Orientation: Portrait only.
- Playfield flow: Top -> Bottom.
- UI zones:
  - Top: wave timer, stage, energy.
  - Right: active skills.
  - Bottom: hero/castle status.
- Keep combat readable with many entities on screen.

## 4) ECS Architecture Scope

### Components (IComponentData)

...

### Authoring/Bakers (MonoBehaviour only for conversion)

...

### Systems (ISystem first)

...

## 5) Combat Rules (MVP)

- Enemy path: straight down by lane.
- Hero targeting priority: nearest to castle, then nearest distance.
- Projectile damage: single target.
- Skill examples (phase 2): chain lightning, lane barrage, freeze.
- Difficulty scaling per wave:
  - +enemy count
  - +enemy HP
  - occasional elite units

## 6) Data Design

Use ScriptableObject for tuning only, convert to ECS at bake/runtime bootstrap:
- EnemyStatTable
- HeroStatTable
- WaveTable
- SkillTable

Rules:
- No managed references in runtime components.
- Use BlobAsset or Native containers for read-only tables when needed.

## 7) Performance Rules

- Burst compile hot systems.
- Avoid structural changes in tight loops.
- Use EntityCommandBuffer for create/destroy/tag changes.
- No LINQ in runtime systems.
- Keep temporary allocations out of frame loop.
- Prefer `ISystem` + `SystemAPI.Query`.

## 8) Folder Contract

## 9) MVP Roadmap

Phase 1 (Playable Core):
- One hero auto-shoot.
- One enemy type.
- Wave spawn + enemy move + castle HP.
- Win/lose condition.

Phase 2 (Depth):
- Multi-hero support.
- 2-3 skills.
- Elite enemy + simple resist/armor.

Phase 3 (Polish):
- Damage text, hit VFX, screen shake (light).
- Better balancing and pacing.
- Mobile optimization pass.

## 10) Current Focus Checklist

- [ ] Setup portrait scene + lanes + castle line
- [ ] Implement EnemyMoveSystem
- [ ] Implement WaveSpawnSystem
- [ ] Implement HeroAttackSystem
- [ ] Implement DamageApplySystem
- [ ] Hook minimal UI values (wave, HP, energy)

## 11) AI Coding Instructions

When generating code:
- Strict ECS separation by folder.
- Keep systems small and single-purpose.
- Provide compile-ready C# files.
- Mention where each file should be placed.

When reviewing code:
- Check Burst/Jobs compatibility.
- Check allocation/GC risk.
- Check structural change safety.
- Check deterministic combat behavior.

