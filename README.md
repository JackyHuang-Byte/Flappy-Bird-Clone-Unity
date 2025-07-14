# 🐤 Flappy Bird Clone (Unity, C#)

A simple 2D game project where the player taps to keep a bird flying while avoiding obstacles.

This project was built to practice core Unity features and strengthen my understanding of interactive systems, input control, and simple UI design.  

---

## 🛠️ What It Does

- Basic jump control triggered by space key or right-click
- Score tracking and game-over detection using collision events
- Pause and resume functions with time scale control handled by a custom time manager
- Multi-scene transitions between start, game, and settings screens
- Settings menu with working volume sliders, resolution options, and mute toggle
- Animated bird movement and layered background with particle system (cloud particles)

---

## 🧩 How It’s Structured

- Code is organized into small managers like `BirdManager`, `PipeManager`, and `TimeManager`
- UI was created using Unity’s built-in canvas system and basic animation tools
- Player preferences (like high scores or settings) are saved using `PlayerPrefs`

---

## ▶️ How to Run

- Open in Unity 2022.3 or newer
- Load the `Title Screen` from the `Scenes` folder
- Enter Play Mode and click the `Play` button
- Press space or right-click to start flying

---

## 💬 What I Learned

- Learned how to break tasks into smaller systems (game loop, input, UI)
- Built a simple but complete project from start to finish
- Got more comfortable using Unity’s interface and organizing files and logic
