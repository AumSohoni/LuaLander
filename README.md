

Lualander :- 

A precision-physics lunar landing simulator built with Unity and C#. This project focuses on handling 2D rigid body dynamics, custom gravity vectors, and fuel management systems.

🚀 Features
Physics-Driven Movement: Uses Unity's Rigidbody2D for authentic momentum, inertia, and gravitational pull.

Vector-Based Thrust: Implementation of directional force application based on the lander's rotation.

Fuel System: A logic-gate system that disables thrust and rotation once the fuel resource is depleted.

Smooth Camera Following: Uses Cinemachine (or custom Lerp logic) to keep the lander framed during high-altitude descents.

Landing Detection: Logic to calculate "Safe Landing" vs. "Crash" based on impact velocity and angular alignment.

🛠️ Technical Stack
Engine: Unity 2022.3+ (LTS)

Language: C#

Input System: Unity New Input System (Actions-based)

Physics: Rigidbody2D with Continuous Collision Detection.

🕹️ How to Play
W / Up Arrow: Activate Main Thrusters (Consumes Fuel).

A-D / Left-Right: Rotate Lander.

Space: Deploy Landing Gear (if implemented).

Objective: Land softly on the designated green platforms. If your vertical velocity is too high upon impact, the lander will explode!

📂 Key Script Components
LanderController.cs: Handles the physics forces and input processing.

FuelManager.cs: Tracks fuel consumption and triggers UI updates.

LandingZone.cs: Detects successful collisions and validates the landing speed.

ThrustVisuals.cs: Bridges the gap between logic and the Particle System for engine exhaust.

✍️ Credits
Developed as part of a learning journey inspired by Code Monkey's game design principles, adapted from the original Lualander concept into the Unity Engine.

Design Pattern Inspiration: Code Monkey
