# Entropy Checkers - Complete Rulebook

## Table of Contents

1. [Overview](#overview)
2. [Setup](#setup)
3. [Basic Movement](#basic-movement)
4. [Capturing](#capturing)
5. [The Aggressive Board](#the-aggressive-board)
6. [King Promotion](#king-promotion)
7. [Winning the Game](#winning-the-game)
8. [Quick Reference](#quick-reference)

---

## Overview

Entropy Checkers is played on a standard 8x8 checkerboard using only the 32 dark squares. Two players (Black and Red) compete to eliminate the opponent's pieces while surviving the board's inevitable collapse.

**Key Differences from Standard Checkers:**

| Standard Checkers | Entropy Checkers |
|-------------------|------------------|
| Static board | Board shrinks every 5 turns |
| Simple king promotion | Choice: Wraith King or Corrupted King |
| Forced jumps (any available) | Must take path with MOST captures |
| Games can end in draws | Board collapse forces conclusion |

---

## Setup

### The Board

```
    0   1   2   3   4   5   6   7
  +---+---+---+---+---+---+---+---+
7 |   | r |   | r |   | r |   | r |  ← Red's back row
  +---+---+---+---+---+---+---+---+
6 | r |   | r |   | r |   | r |   |
  +---+---+---+---+---+---+---+---+
5 |   | r |   | r |   | r |   | r |  ← Red's front row
  +---+---+---+---+---+---+---+---+
4 |   |   |   |   |   |   |   |   |  ← Empty center
  +---+---+---+---+---+---+---+---+
3 |   |   |   |   |   |   |   |   |  ← Empty center
  +---+---+---+---+---+---+---+---+
2 | b |   | b |   | b |   | b |   |  ← Black's front row
  +---+---+---+---+---+---+---+---+
1 |   | b |   | b |   | b |   | b |
  +---+---+---+---+---+---+---+---+
0 | b |   | b |   | b |   | b |   |  ← Black's back row
  +---+---+---+---+---+---+---+---+

b = Black piece    r = Red piece
```

- Each player begins with **12 pieces**
- Pieces occupy only the dark squares
- The board should be oriented so each player has a dark square in their left corner

### Starting Player

**Black always moves first.**

---

## Basic Movement

### Regular Pieces

- Move **diagonally forward** only (toward opponent's side)
- Move **one square** at a time
- Cannot move backward
- Cannot move onto occupied squares
- Must stay on dark squares

**Black's forward direction:** Increasing Y (rows 0→7)  
**Red's forward direction:** Decreasing Y (rows 7→0)

### Kings

- Move **diagonally in any direction** (forward or backward)
- Move **one square** at a time (except Wraith Kings—see [King Promotion](#king-promotion))

---

## Capturing

### Basic Capture (Jump)

To capture an opponent's piece:

1. The opponent's piece must be in an adjacent diagonal square
2. The square directly beyond the opponent (in the same diagonal line) must be empty
3. Jump over the opponent's piece to the empty square
4. The jumped piece is **removed from the board**

```
Before:          After:
+---+---+---+    +---+---+---+
|   |   |   |    |   |   | b |  ← Black lands here
+---+---+---+    +---+---+---+
|   | r |   |    |   | X |   |  ← Red piece removed
+---+---+---+    +---+---+---+
| b |   |   |    |   |   |   |  ← Black was here
+---+---+---+    +---+---+---+
```

### ⚠️ Compulsory Carnage Rule

> **This is the first major twist of Entropy Checkers.**

**If you CAN capture, you MUST capture.**

But more importantly:

**You must take the path that captures the MAXIMUM number of pieces.**

If multiple capture paths exist:
- Calculate total captures for each possible path
- You MUST choose a path with the highest capture count
- If multiple paths tie for most captures, you may choose between them

#### Example: Forced Maximum Capture

```
Black to move. Two capture paths available:

Path A: Jump one piece (1 capture)
Path B: Jump three pieces in sequence (3 captures)

Black MUST choose Path B.
```

#### The Trap

This rule can be **weaponized**. Skilled players will arrange their pieces to force opponents into capture chains that:

- End on a **hazard tile** (destroying the capturing piece)
- Leave the capturing piece in a **vulnerable position**
- Set up a devastating counter-attack

**In Entropy Checkers, sometimes the best move is to make your opponent capture you.**

### Multi-Jump Sequences

When a capture lands you in position for another capture:

1. You **must** continue jumping with the same piece
2. Continue until no more captures are possible
3. All captured pieces are removed after the sequence completes

---

## The Aggressive Board

> **This is the second major twist of Entropy Checkers.**

The board is your enemy. It shrinks relentlessly, forcing confrontation.

### Shrink Mechanic

**Every 5 turns**, the board contracts:

| Turn | Effect |
|------|--------|
| 5 | Ring 0 (outermost edge) becomes **Hazardous** |
| 10 | Ring 0 is **Removed**, Ring 1 becomes **Hazardous** |
| 15 | Ring 1 is **Removed**, Ring 2 becomes **Hazardous** |
| 20 | Ring 2 is **Removed**, Ring 3 becomes **Hazardous** |

### Board Rings

```
Ring levels (distance from edge):

    0   1   2   3   4   5   6   7
  +---+---+---+---+---+---+---+---+
7 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
  +---+---+---+---+---+---+---+---+
6 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
5 | 0 | 1 | 2 | 2 | 2 | 2 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
4 | 0 | 1 | 2 | 3 | 3 | 2 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
3 | 0 | 1 | 2 | 3 | 3 | 2 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
2 | 0 | 1 | 2 | 2 | 2 | 2 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
1 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 0 |
  +---+---+---+---+---+---+---+---+
0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
  +---+---+---+---+---+---+---+---+
```

### Tile States

| State | Visual | Effect |
|-------|--------|--------|
| **Normal** | Standard | Safe to occupy |
| **Hazardous** | Warning color | Pieces here will be destroyed next shrink |
| **Removed** | Empty/void | Cannot be entered; pieces cannot exist here |

### Hazard Effects

- **Moving onto a Hazard tile:** Piece is immediately destroyed
- **Being on a Hazard tile when it becomes Removed:** Piece is destroyed
- **Forced to jump onto a Hazard tile:** Piece is destroyed after completing the capture

### Strategic Implications

1. **No more endless king chases** - The shrinking board guarantees game resolution
2. **Edge pieces are vulnerable** - Pieces on outer rings die first
3. **Center control is critical** - Inner positions survive longest
4. **Time pressure increases** - Each shrink event escalates urgency

---

## King Promotion

> **This is the third major twist of Entropy Checkers.**

When a piece reaches the opponent's back row (the "King's Row"), promotion is **not automatic**. The player must choose between two corrupted forms of power:

### Option 1: Wraith King 👻

**Power:** Unlimited diagonal movement (like a bishop in chess)

**Cost:** The Wraith King dies after **3 turns**

| Turn | Status |
|------|--------|
| Promotion | Wraith King created (3 turns remaining) |
| +1 turn | 2 turns remaining |
| +2 turns | 1 turn remaining |
| +3 turns | Wraith King is destroyed |

**Best for:**
- Immediate tactical strikes
- Breaking through enemy lines
- Desperate endgame situations
- When the board is about to shrink anyway

### Option 2: Corrupted King 👑

**Power:** Standard king movement (diagonal, any direction, one square)

**Cost:** One of your OTHER pieces immediately **defects to the opponent**

The defecting piece:
- Is chosen by the player (you pick which piece to sacrifice)
- Switches color and becomes an enemy piece
- Remains in its current position
- Can be used by the opponent on their next turn

**Best for:**
- Long-term board presence
- When you have expendable pieces
- Securing permanent king advantage
- When a piece is about to be destroyed by board shrink anyway

### Promotion Decision

When a piece reaches the promotion row:

1. The game **pauses**
2. Player is presented with the choice: **Wraith** or **Corrupted**
3. After selection, the piece transforms and the game continues

**There is no option for a standard king.** In Entropy Checkers, power always comes with a price.

---

## Winning the Game

A player wins when:

### 1. Elimination Victory
Capture all of the opponent's pieces.

### 2. Stalemate Victory
The opponent cannot make any legal moves on their turn (all pieces blocked or no pieces remaining).

### 3. Attrition Victory
After board shrink events, only one player has surviving pieces.

### Draws

Draws are **extremely rare** in Entropy Checkers due to the shrinking board mechanic. However, a draw may be declared if:

- Both players are reduced to a single king each
- Neither can capture the other before the board shrinks to nothing
- In this case, **both players lose to the board itself**

---

## Quick Reference

### Turn Order
1. Black moves
2. Red moves
3. Repeat
4. Every 5 turns: Board shrinks

### Movement Summary
| Piece | Direction | Distance |
|-------|-----------|----------|
| Regular | Forward diagonal | 1 square |
| King (Corrupted) | Any diagonal | 1 square |
| King (Wraith) | Any diagonal | Unlimited (dies in 3 turns) |

### Capture Rules
- **Must capture** if able
- **Must take maximum captures** if multiple paths exist
- **Must complete** multi-jump sequences

### King Promotion
| Type | Power | Cost |
|------|-------|------|
| Wraith | Unlimited range | Dies in 3 turns |
| Corrupted | Standard king | One piece defects |

### Board Shrink Schedule
| Turn | Ring Removed | Ring Hazardous |
|------|--------------|----------------|
| 5 | — | 0 (edge) |
| 10 | 0 | 1 |
| 15 | 1 | 2 |
| 20 | 2 | 3 (center) |

---

*Remember: In Entropy Checkers, the board always wins eventually. Your job is to make sure your opponent loses first.*
