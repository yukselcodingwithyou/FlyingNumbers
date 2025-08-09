# Wing Sprites Required

This directory should contain wing sprites extracted from the character sprites or created separately:

## Required Wing Sprites:
- **number1_wing.png** - Wing sprite for the Number 1 character
- **cat_wing.png** - Wing sprite for the Cat character  
- **dog_wing.png** - Wing sprite for the Dog character
- **fox_wing.png** - Wing sprite for the Fox character

## Sprite Import Settings:
- Texture Type: Sprite (2D and UI)
- Filter Mode: Point (or Bilinear)
- Compression: Automatic
- Pixels Per Unit: 100
- Mesh Type: Full Rect
- Pivot: Center (or Custom if wing hinge point is different)

## Usage:
These wing sprites will be used in the character prefabs as separate GameObjects with:
- SpriteRenderer component
- WingAnimationController script
- Animator component with WingAnimator controller

The wings should be positioned relative to the character body sprite at the appropriate hinge/shoulder points for natural flapping animation.