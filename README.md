# Entropy Checkers

> *The board is not your friend.*

A Unity 6 implementation of a strategic board game that takes classic checkers and transforms it into a battle of **Environmental Attrition**. Play against an AI opponent on a battlefield that actively works against both players—forcing engagement, punishing hesitation, and guaranteeing decisive conclusions.

## Core Philosophy

In standard checkers, the board is a neutral stage. In Entropy Checkers, **the board is an active antagonist**. Every decision must account for not just your opponent's pieces, but the inevitable collapse of the playing field itself.

## Game Rules

📖 **[Complete Rulebook](docs/RULES.md)**

### The Three Pillars

1. **The Aggressive Board** - The battlefield shrinks every 5 turns, consuming pieces caught in the collapse
2. **Corrupted Promotion** - Reaching the king's row forces an impossible choice: fleeting power or permanent sacrifice
3. **Compulsory Carnage** - When you can capture, you must take the path of maximum destruction—even if it destroys you

## Quick Start

### Setup
- 8x8 board, dark squares only (32 playable squares)
- Each player starts with 12 pieces on their nearest three rows
- Black moves first

### Basic Movement
- Pieces move diagonally forward, one square at a time
- Kings move diagonally in any direction

### The Twist
Every 5 turns, the outermost ring of the board becomes **hazardous**. Pieces on hazard tiles are destroyed. The board continues shrinking until only a 4-square center remains.

## Documentation

| Document | Description |
|----------|-------------|
| [Rules](docs/RULES.md) | Complete game rules and mechanics |
| [Strategy Guide](docs/STRATEGY.md) | Tactics for the aggressive board |
| [Architecture](docs/ARCHITECTURE.md) | Technical design documentation |
| [Contributing](CONTRIBUTING.md) | How to contribute to the project |

## Project Status

🚧 **In Development**

- [x] Core data structures (Board, Tile, Piece, Move)
- [ ] Board rendering
- [ ] Move validation and capture pathfinding
- [ ] King promotion system (Wraith/Corrupted choice)
- [ ] Board shrinking mechanic
- [ ] Win conditions
- [ ] AI opponent with difficulty levels
- [ ] UI polish

## Tech Stack

- **Language**: C#
- **Engine**: [Unity 6](https://unity.com/) (6000.3.5f1)
- **Rendering**: Universal Render Pipeline (URP)
- **Graphics**: 2D sprites and tilemaps
- **Input**: Unity Input System

### Requirements

- Unity 6 (6000.3.5f1 or later)
- Git for cloning the repository

## License

[MIT License](LICENSE)

## Credits

Entropy Checkers - A game where survival is temporary, but entropy is forever.
