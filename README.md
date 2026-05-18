# 🎰 Slot Machine Game — Unity

A classic slot machine game built in Unity 2D as part of a game development assignment.

---

## 🎮 Game Overview

A fully playable slot machine game featuring:
- 3 spinning reels with 4 unique symbols (Seven, Cherry, Bell, BAR)
- Smooth reel spin and snap animations
- Win detection when all 3 center symbols match
- Weighted RNG system for fair and exciting outcomes
- Near-miss bias for added excitement
- Payout system with multipliers per symbol
- Balance and bet tracking UI
- Win popup with jackpot message
- Animated machine frame and lever states

---

## 🕹️ How to Play

1. Open the WebGL build in your browser (link below)
2. Click the **lever** on the right side of the machine to spin
3. Wait for all 3 reels to stop
4. If all 3 center symbols match — **YOU WIN!**
5. Payout is added to your balance automatically

---

## 🌐 How to Run WebGL Build

1. Navigate to the `/Build/WebGL/` folder in this repository
2. Open `index.html` in a modern browser (Chrome recommended)
3. If it doesn't load locally due to CORS, upload to a server or use:
   ```
   python -m http.server 8000
   ```
   Then open `http://localhost:8000` in your browser

---

## 💰 Payout Table

| Symbol  | Multiplier |
|---------|------------|
| 🍒 Cherry  | 5×  |
| 🔔 Bell    | 10× |
| 💎 Diamond | 20× |
| 7️⃣ Seven   | 50× |

---

## ✨ Bonus Features

- **Weighted RNG** — Common symbols (Cherry, Bell) appear more frequently for balanced gameplay
- **Near-miss bias** — First two reels often match to build excitement before the third stops
- **Lever animation** — Lever visually pulls down on spin and resets on result
- **Staggered reel stops** — Reels stop one by one for dramatic effect
- **Symbol masking** — Center symbol is clearly focused with partial symbols visible above and below

---

## 🧠 Thought Process & Approach

### Architecture
The game is split into two core scripts:
- **`ReelController.cs`** — Handles individual reel spinning, sprite swapping, and snapping logic
- **`SlotMachineManager.cs`** — Manages game state, balance, bet, win evaluation, and UI

### Reel Animation
Instead of physically scrolling a long strip of sprites (which caused alignment issues), I used a **sprite-swap approach** — a fixed set of 5 image slots where sprites are cycled in and out as the strip scrolls. This ensures symbols always snap perfectly to center regardless of which symbol is selected.

### Win Detection
Win is determined by comparing the **result index** of each reel rather than sprite or string comparison, making it reliable and cheat-proof.

### RNG & Fairness
A **weighted random system** gives different probabilities to each symbol:
- Cherry: 40% | Bell: 30% | Diamond: 20% | Seven: 10%

A **near-miss bias** (35% chance) makes the first two reels match more often, creating excitement before the final reel stops.

---

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── ReelController.cs
│   ├── SlotMachineManager.cs
│   ├── SymbolData.cs
│   └── MachineAnimator.cs
├── Sprites/
│   ├── Symbols/
│   ├── Machine/
│   └── UI/
├── Prefabs/
├── Animations/
├── Scenes/
└── Sounds/
```

---

## 🔧 Built With

- **Unity 2021.3 LTS**
- **C#**
- **TextMeshPro**
- **Unity UI (uGUI)**

---

## 👤 Author

**ninaaad**  
GitHub: [github.com/ninaaad](https://github.com/ninaaad)
