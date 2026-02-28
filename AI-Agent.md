# 🚀 Unity 3D ECS Project -- AI Assistant Context File

## 📌 Project Overview

-   Engine: Unity (3D)
-   Architecture: ECS (DOTS)
-   Target Platform: Mobile
-   Programming Language: C#

------------------------------------------------------------------------

## 🎯 Project Goals

-   Build scalable 3D game using ECS pattern
-   Optimize performance for large number of entities
-   Avoid GC allocations
-   Use Burst + Jobs for heavy systems

------------------------------------------------------------------------

## 📂 Folder Structure

    Assets/
     ├── Scripts/
     │    ├── Components/
     │    ├── Systems/
     │    ├── Aspects/
     │    ├── Authoring/
     ├── Prefabs/
     ├── SubScenes/
     ├── Materials/

------------------------------------------------------------------------

## 🧱 ECS Rules for AI

### 1️⃣ Components

-   Must be `struct`
-   Implement `IComponentData`
-   No reference types
-   No managed objects

### 2️⃣ Systems

-   Prefer `ISystem`
-   Use `SystemAPI.Query`
-   Use `RefRO` / `RefRW` properly
-   BurstCompile when possible

### 3️⃣ Performance Rules

-   No LINQ
-   No GC allocations
-   Avoid structural changes inside loops
-   Use EntityCommandBuffer when needed

------------------------------------------------------------------------

## 🧠 Coding Style Guide

-   Clean separation: Component / System
-   No MonoBehaviour logic (only Authoring)
-   Data-oriented mindset
-   Avoid OOP-heavy patterns

------------------------------------------------------------------------

## 🛠 AI Support Instructions

When generating code: - Follow ECS pattern strictly - Separate files
clearly - Avoid managed allocations - Add comments explaining system
logic - Prefer scalable architecture

When reviewing code: - Check for GC allocations - Check correct RefRO /
RefRW usage - Check Burst compatibility - Suggest performance
improvements

------------------------------------------------------------------------

## 🔥 Current Development Focus

-   [ ] Movement system
-   [ ] Enemy spawn system
-   [ ] Combat logic
-   [ ] Optimization pass
-   [ ] Mobile performance testing

------------------------------------------------------------------------

## 📝 Notes

(Add project-specific rules or decisions here)

------------------------------------------------------------------------

End of context file.
