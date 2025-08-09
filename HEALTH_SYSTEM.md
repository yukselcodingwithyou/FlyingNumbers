# Health Bar System Implementation

This document describes the health bar system implementation for the Flying Numbers game.

## Features Implemented

### 1. Health System in PlayerController

- **maxHealth**: Maximum health points (default: 3)
- **currentHealth**: Current health points (initialized to maxHealth)
- **TakeDamage(int amount)**: Method to reduce health and handle damage
- **GetCurrentHealth()**: Public getter for current health
- **GetMaxHealth()**: Public getter for maximum health
- **OnHealthChanged**: Event that fires when health changes

### 2. Damage Integration

The health system integrates with the existing shield system:
1. When damage is taken, shields absorb damage first
2. If no shields, health is reduced
3. Game over triggers when health reaches zero
4. Health changes trigger UI updates via events

### 3. Obstacle Damage System

All obstacles now have a `damageAmount` property with these default values:

- **Pipe**: 1 damage
- **Mine**: 1 damage  
- **MovingSpikes**: 2 damage
- **RotatingBlades**: 3 damage
- **LaserBeams**: 5 damage

### 4. New Obstacles

#### MovingSpikes
- Moves leftward with vertical sine wave motion
- Rotating and scaling animations for threatening appearance
- Deals 2 damage on collision

#### RotatingBlades  
- Moves leftward with rapid spinning animation
- Rotation speed increases over time
- Deals 3 damage on collision
- Has hit cooldown to prevent multiple rapid hits

#### LaserBeams
- Complex state machine: Cooldown → Charging → Firing → repeat
- Only deals damage during firing phase
- Highest damage at 5 points
- Visual charging and firing effects

### 5. HealthBar UI Component

- Supports multiple UI modes: Slider, Image Fill, or Heart Icons
- Real-time health updates via PlayerController events
- Smooth animations for health changes
- Color gradient from green (full) to red (low)
- Automatically finds and connects to player

## Usage Instructions

### Setting Up Health Bar UI

1. Create a UI Canvas with a Slider, Image, or Heart icons
2. Add the `HealthBar` script to a GameObject
3. Assign the UI components in the inspector:
   - `healthSlider`: For slider-based health bar
   - `healthFillImage`: For image fill health bar  
   - `healthHeartImages`: Array of heart icon images
4. Configure visual settings (colors, animation speed)

### Adding New Obstacles

1. Create obstacle prefab with appropriate sprite
2. Add one of the obstacle scripts (MovingSpikes, RotatingBlades, LaserBeams)
3. Configure damage amount and movement parameters
4. Ensure Player tag collision detection works
5. Add to spawn manager obstacle list

### Testing

Use the `HealthSystemTest` script to test the health system:
- Press 'T' to deal test damage
- Press 'Y' to add shields
- Monitor console output for health/shield status

## Technical Notes

- Health system maintains backward compatibility with existing shield system
- All obstacle scripts follow the same pattern: movement + collision damage
- UI system uses Unity's event system for loose coupling
- Placeholder sprites are auto-generated via Unity editor tools

## File Structure

```
Assets/Scripts/
├── Player/
│   └── PlayerController.cs (enhanced with health system)
├── Obstacles/
│   ├── Pipe.cs (updated with damage)
│   ├── Mine.cs (updated with damage)  
│   ├── MovingSpikes.cs (new)
│   ├── RotatingBlades.cs (new)
│   └── LaserBeams.cs (new)
├── UI/
│   └── HealthBar.cs (new)
└── Tests/
    └── HealthSystemTest.cs (new)
```

## Editor Tools Enhanced

- **PlaceholderSpriteGenerator**: Adds sprites for new obstacles
- **PrefabAutoBuilder**: Creates prefabs for new obstacles
- Use Unity menu: Tools → FlyingNumbers → Generate/Build commands