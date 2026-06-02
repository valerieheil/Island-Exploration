# 🐾 Island Explorer: Dog Edition — Full Setup Guide

## What's in this package

| Script | What it does |
|---|---|
| `GameManager.cs` | Singleton — tracks bones collected, win state |
| `UIManager.cs` | Bone counter, hint messages, win screen, collect flash |
| `CollectibleBone.cs` | Dog walks near → "Press F to dig!" → digs → collects → celebrate |
| `DogAnimationHelper.cs` | Safely triggers Celebrate/Dig animations on the dog's Animator |
| `DogScentTrail.cs` | Particle system on dog that drifts toward nearest bone (Witcher sense) |
| `GoalTrigger.cs` | Win condition at camp/home — only works once all bones found |

---

## Step 1 — Import the package
`Assets → Import Package → Custom Package → dog_island_scripts.unitypackage`

All scripts land in `Assets/Scripts/`.

---

## Step 2 — Tag the Dog as "Player"
1. Click your **dog GameObject** in the Hierarchy
2. Top of Inspector → **Tag → Player**
3. If "Player" doesn't exist: **Add Tag** → type `Player` → save → come back and set it

> ⚠️ This is critical. Every trigger (bones, goal) checks for the "Player" tag.
> If the dog isn't tagged, nothing will collect.

---

## Step 3 — Create the GameManager Object
1. Right-click Hierarchy → **Create Empty** → name it `GameManager`
2. **Add Component → GameManager**
3. **Add Component → UIManager**
4. **Add Component → DogAnimationHelper**
5. In the **DogAnimationHelper** component:
   - Drag your **dog GameObject** into `Dog Animator`
   - Open the dog's **Animator Controller** — find the exact name of the celebrate trigger
   - Type that name into `Celebrate Trigger` (`Celebrate`)
   - If you're unsure of the name, open the Animator window and look at the parameters tab

---

## Step 4 — Build the Canvas UI

### Create Canvas:
1. Right-click Hierarchy → **UI → Canvas**

### Add bone counter:
2. Right-click **Canvas** → **UI → Text - TextMeshPro** → name it `BoneCounter`
   - Text: `Bones: 0 / 10`
   - Anchor: top-left corner
   - Font size: 36, colour: white

### Add hint text:
3. Right-click **Canvas** → **UI → Text - TextMeshPro** → name it `HintText`
   - Text: *(leave empty)*
   - Anchor: **center** of screen (middle)
   - Font size: 32, colour: yellow
   - Set **Alignment** to Center

### Add collect flash:
4. Right-click **Canvas** → **UI → Image** → name it `CollectFlash`
   - Set **Rect Transform**: Stretch to fill whole screen (anchor all corners)
   - Set **Color** → set **Alpha (A) slider** to **0**

### Add win screen:
5. Right-click **Canvas** → **UI → Panel** → name it `WinScreen`
   - Inside it, add a TextMeshPro: "You found all the bones! Good dog! 🐾"
   - In Inspector **uncheck** WinScreen (disable it at start)

### Add all-found banner:
6. Right-click **Canvas** → **UI → Panel** → name it `AllFoundBanner`
   - Inside it add TMP text: "All bones found! 🏠 Run home!"
   - Small, anchor to **top-center**
   - Uncheck/disable it at start

---

## Step 5 — Wire UIManager in Inspector
1. Click **GameManager** in Hierarchy
2. In the **UIManager** component, drag:

| Field | Drag from Hierarchy |
|---|---|
| Counter Text | `BoneCounter` |
| Hint Text | `HintText` |
| Collect Flash Image | `CollectFlash` |
| Win Screen | `WinScreen` |
| All Found Banner | `AllFoundBanner` |

3. In the **GameManager** component:
   - Drag `GameManager` object itself into `Ui Manager` (or leave empty — auto-finds)

---

## Step 6 — Place Bones Around the Island

For each bone:
1. Right-click Hierarchy → **3D Object → Sphere** (or use a bone mesh if you have one)
2. Name it `Bone_1`, `Bone_2`, etc.
3. Scale it down: **X: 0.3, Y: 0.3, Z: 0.3**
4. Place it somewhere on the island terrain
5. **Add Component → CollectibleBone**
6. The SphereCollider is already there — tick **Is Trigger ✅**
7. In CollectibleBone component:
   - `Dig Duration`: match this to your dog's dig animation length (try 1.5s)
   - `Bob Target`: drag the sphere's own Transform in here (makes it bob up/down)

**Repeat x10** — spread them around: beach, forest, mountains, cave area, etc.

### Suggested positions (adjust Y to match your terrain height):
```
Bone 1:  (100, +2, 120)  — near spawn/beach
Bone 2:  (200, +4, 180)  — forest entrance
Bone 3:  (280, +6, 90)   — by the water edge
Bone 4:  (150, +3, 320)  — forest deep
Bone 5:  (380, +8, 200)  — hillside
Bone 6:  (450, +12, 300) — mountain path
Bone 7:  (320, +5, 420)  — northern shore
Bone 8:  (500, +15, 250) — near summit
Bone 9:  (220, +4, 480)  — lake area
Bone 10: (480, +5, 480)  — near the goal
```

---

## Step 7 — Set Up the Goal (Home / Camp)

1. Right-click Hierarchy → **3D Object → Cube** → name it `Home_Goal`
   - Scale: **(5, 3, 5)** — big enough for the dog to walk into
   - Position it somewhere visible and meaningful (beach, camp, a special rock)
2. **Add Component → GoalTrigger**
3. In the **BoxCollider**: tick **Is Trigger ✅**
4. Optional: add a **Point Light** above it as a beacon (warm yellow, range 80)

---

## Step 8 — Dog Scent Trail (the cool Witcher-sense guide)

1. Click on your **dog GameObject** in Hierarchy
2. Right-click it → **Effects → Particle System** — this creates a child particle system
3. Name it `ScentTrail`
4. Position it at roughly the dog's nose (adjust in local space)
5. **Add Component → DogScentTrail**
6. Tune the particle system to look like scent wisps:
   - Start Size: **0.1**
   - Start Speed: **0** (script controls velocity)
   - Start Lifetime: **1.5**
   - Color: light blue / white with alpha fade
   - Emission rate: **15**
   - Renderer → Material: Default-Particle or a soft glow material

The particles will automatically drift toward the nearest bone!

---

## Step 9 — Find the Celebrate Trigger Name

This is the most important step for the dog animations:

1. Select your **dog GameObject**
2. In Inspector find the **Animator** component
3. Click the **Animator Controller** asset (the circle icon)
4. Go to **Window → Animation → Animator** to open the Animator window
5. Click the **Parameters** tab (left side)
6. Look for any Trigger type parameter that sounds like celebrate/happy/win
7. Copy the **exact name** (case-sensitive!)
8. Paste it into **DogAnimationHelper → Celebrate Trigger** field

If there's no celebrate trigger, you can leave the field blank — the collect still works, the dog just won't do a special animation on pickup.

---

## Step 10 — Test Run Checklist

Before pressing Play, verify:
- [ ] Dog tagged as **"Player"**
- [ ] At least 3 bones placed with **Is Trigger ✅** and **CollectibleBone** script
- [ ] **GameManager** object exists with all 3 scripts
- [ ] **UIManager** fields are wired up
- [ ] **GoalTrigger** object exists with **Is Trigger ✅**

Press **Play** → walk dog near a bone → hint should say "Press F to dig!" → press F → bone counter goes up.

---

## Grading Checklist ✅

| Feature | Status |
|---|---|
| Terrain (well-designed) | ✅ Already done — looks amazing! |
| Player movement | ✅ Dog controller already working |
| Collectible items with interaction | ✅ CollectibleBone.cs — dig to collect |
| Collect counter UI | Wire up UIManager |
| Clear goal / win condition | ✅ GoalTrigger.cs |
| Win screen | Wire up UIManager |
| Day/night cycle | Add DayNightCycle.cs (from previous package) |
| Visual guide (Witcher-sense) | ✅ DogScentTrail.cs |
| Animations on collect | ✅ DogAnimationHelper.cs |
| Connected to the world (not flying!) | ✅ Dog digs up bones from the ground |

---

## Quick Troubleshooting

| Problem | Fix |
|---|---|
| Pressing F does nothing near bone | Check dog is tagged "Player"; check CollectibleBone Is Trigger is on |
| Celebrate never plays | Open Animator window, find exact trigger name, paste in DogAnimationHelper |
| Counter doesn't update | Wire `counterText` in UIManager component |
| Scent particles don't move | Make sure GameManager is in scene; check DogScentTrail is on a ParticleSystem |
| Dog falls through terrain | Increase Y position of dog spawn; check terrain has a collider |
