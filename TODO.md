### Tutorials
# Water Shader
[Creating a Water Shader](https://www.youtube.com/watch?v=gRq-IdShxpU)

# Unity Terrain
[Getting Started with Terrain Tools in Unity Part 1: Interface and Mesh Sculpting](https://medium.com/nerd-for-tech/getting-started-with-terrain-tools-in-unity-part-1-interface-and-mesh-sculpting-f8348308fa8d)
[Getting Started with Terrain Tools in Unity Part 2: Textures and Trees](https://medium.com/nerd-for-tech/getting-started-with-terrain-tools-in-unity-part-2-textures-and-trees-5f6b2f57393c)


### 1 – Import the package
`Assets → Import Package → Custom Package → island_explorer_enhanced.unitypackage`

### 2 – Add a Player
- Go to **Package Manager → Unity Registry → Starter Assets – First Person Character Controller** → Install
- Drag the **FirstPersonController** prefab into the scene
- Set its position to **(30, 10, 30)** (above spawn point – terrain height will settle it)
- Tag the Player root as **"Player"** (`Inspector → Tag → Player`)
- If not using Starter Assets, add an empty GO with:
  - `CharacterController` component
  - `PlayerController.cs` script
  - Child Camera inside it

### 3 – Wire up UIManager references
- Select **GameManager** in Hierarchy
- In **UIManager** component, drag in:
  - Your Canvas Text objects → `itemCounterText`, `hintText`
  - A full-screen Image (alpha 0) → `collectFlash`
  - Panel GameObjects → `startScreen`, `winScreen`, `allCollectedBanner`
- In **GameManager** component:
  - Drag your **UIManager** component → `uiManager` field

### 4 – Create minimal UI Canvas
1. `GameObject → UI → Canvas`
2. Add a **Text** ("Crystals: 0 / 10") – assign to `itemCounterText`
3. Add a full-screen **Image** (alpha 0) – assign to `collectFlash`
4. Add an **Empty Panel** with a "Start" button – assign to `startScreen`
   - Button calls `PlayerController.OnStartButtonPressed()`
5. Add another Panel – "You Win!" – assign to `winScreen` (start disabled)
6. Add small banner – "All crystals found!" – assign to `allCollectedBanner` (start disabled)

### 5 – Crystals (already placed, adjust heights)
The 10 crystals are placed at approximate heights. Since your terrain sculpt is unique, **select each Crystal_* in the hierarchy** and adjust Y so it floats slightly above the ground (Y ≈ terrain height + 1.5).

You can check heights quickly by pressing **Play** and looking at where they are.

### 6 – Goal Tower
- **GoalTower** is at (480, 5, 480) – a tall white cylinder.
- Adjust its Y so its base sits on the terrain.
- **Replace it** with a proper asset (ruin, lighthouse, signal fire) from the Asset Store for bonus marks!
- The **GoalBeacon** yellow light above it is already set to range 200 – visible from the whole map.

### 7 – Day/Night Cycle
Already wired to your Directional Light. Default full cycle = **5 minutes**.
To slow it down: select **DayNightCycleController → DayNightCycle → dayDuration = 600** (10 min).

### 8 – Wind Indicator (optional but 🔥)
1. `GameObject → Effects → Particle System`
2. Name it **WindIndicator**
3. Attach `WindIndicator.cs` to it
4. Set particles to be small, low-speed, white/blue wisps
5. Works out of the box – blows toward the nearest crystal, then toward the tower when all collected.

### 9 – Collectible Sounds (optional)
- Add **AudioSource** components to the GameManager GO
- Assign short sound clips to `collectSound` and `winSound` in UIManager