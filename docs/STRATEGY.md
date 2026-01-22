# Entropy Checkers - Strategy Guide

## Table of Contents

1. [Fundamental Principles](#fundamental-principles)
2. [Early Game (Turns 1-5)](#early-game-turns-1-5)
3. [Mid Game (Turns 6-15)](#mid-game-turns-6-15)
4. [Late Game (Turns 16+)](#late-game-turns-16)
5. [Mastering the Three Pillars](#mastering-the-three-pillars)
6. [Advanced Tactics](#advanced-tactics)
7. [Common Mistakes](#common-mistakes)

---

## Fundamental Principles

### 1. The Board is a Timer

Unlike standard checkers where you can play defensively forever, Entropy Checkers has a built-in clock. Every 5 turns, the playable area shrinks. This means:

- **Passive play is punished** - Pieces hiding in corners will die to board shrink
- **Aggression is rewarded** - Forcing trades in the outer rings gives you positional advantage
- **Time is a resource** - Sometimes it's worth sacrificing material to survive longer

### 2. Position > Material

Having more pieces doesn't matter if they're all on the outer rings. A single piece in the center can outlast three pieces on the edge.

**Priority zones:**
```
    0   1   2   3   4   5   6   7
  +---+---+---+---+---+---+---+---+
7 | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ |  Ring 0: Death zone (Turn 5+)
  +---+---+---+---+---+---+---+---+
6 | ☠ | ⚠ | ⚠ | ⚠ | ⚠ | ⚠ | ⚠ | ☠ |  Ring 1: Danger zone (Turn 10+)
  +---+---+---+---+---+---+---+---+
5 | ☠ | ⚠ | ○ | ○ | ○ | ○ | ⚠ | ☠ |  Ring 2: Contested (Turn 15+)
  +---+---+---+---+---+---+---+---+
4 | ☠ | ⚠ | ○ | ★ | ★ | ○ | ⚠ | ☠ |  Ring 3: Safe haven (Turn 20+)
  +---+---+---+---+---+---+---+---+
3 | ☠ | ⚠ | ○ | ★ | ★ | ○ | ⚠ | ☠ |
  +---+---+---+---+---+---+---+---+
2 | ☠ | ⚠ | ○ | ○ | ○ | ○ | ⚠ | ☠ |
  +---+---+---+---+---+---+---+---+
1 | ☠ | ⚠ | ⚠ | ⚠ | ⚠ | ⚠ | ⚠ | ☠ |
  +---+---+---+---+---+---+---+---+
0 | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ | ☠ |

☠ = Avoid   ⚠ = Temporary   ○ = Good   ★ = Best
```

### 3. Forced Captures Are Double-Edged

The Compulsory Carnage rule means captures can be traps:

- **Offensive use**: Bait opponent into capture chains ending on hazards
- **Defensive use**: Position pieces so opponent MUST take unfavorable captures
- **Calculation**: Always count the full capture chain before setting up positions

---

## Early Game (Turns 1-5)

### Objectives

1. **Advance toward the center** - Move pieces off the back row and toward rings 1-2
2. **Avoid the edges** - Don't commit pieces to columns 0 or 7
3. **Maintain flexibility** - Don't lock pieces into positions where they can't retreat

### Opening Principles

**DO:**
- Develop pieces from the back row
- Control the center diagonals
- Keep pieces mobile

**DON'T:**
- Rush for promotion (the cost isn't worth it early)
- Trade pieces on the outer ring (you lose the piece AND the position)
- Block your own pieces

### Sample Opening Moves (Black)

```
Turn 1: Move a center piece forward
        (e.g., (2,2) → (3,3) or (4,2) → (3,3))
        
Turn 2: Develop another center piece
        Support your advanced piece
        
Turn 3-4: Continue central development
          Prepare for Turn 5 shrink
```

### Turn 5 Preparation

**Critical**: Before Turn 5, evaluate which of your pieces are on Ring 0.

- If you have pieces on the edge: Get them moving inward
- If opponent has edge pieces: Consider forcing them to stay there through positioning

---

## Mid Game (Turns 6-15)

### The Shrinking Reality

By Turn 10, the board looks very different:

```
Turn 5-9: Ring 0 is Hazardous (edge is dangerous)
Turn 10-14: Ring 0 is gone, Ring 1 is Hazardous
```

### Objectives

1. **Consolidate in the center** - All pieces should be on Ring 2 or deeper
2. **Force opponent outward** - Use the Compulsory Carnage rule to trap them on hazards
3. **Evaluate promotion** - Is it time to make a king?

### The Promotion Decision

**Choose Wraith King when:**
- You need immediate impact (breaking a defensive formation)
- The board is about to shrink (3 turns of Wraith = surviving the shrink)
- You're losing and need a Hail Mary

**Choose Corrupted King when:**
- You have a piece on a hazard tile anyway (free defection)
- You have significant material advantage (can afford to lose a piece)
- You need long-term board control

### Mid-Game Tactics

#### The Hazard Trap

Set up a capture chain that ends on a hazard tile:

```
Before:
+---+---+---+---+
| H |   |   |   |   H = Hazard tile
+---+---+---+---+
|   | b |   |   |   b = Your piece (bait)
+---+---+---+---+
|   |   | r |   |   r = Opponent's piece
+---+---+---+---+
|   |   |   |   |
+---+---+---+---+

Opponent MUST capture b, landing on H and dying.
```

#### The Pincer

Use the shrinking board as one "attacker":

```
Ring 1 becomes hazard →  | r |  ← Your piece threatens from other side

Opponent's piece is squeezed between the hazard and your attack.
```

---

## Late Game (Turns 16+)

### The Endgame Board

By Turn 20, only the center 4x4 (Ring 3) remains:

```
Playable area (Turn 20+):

    3   4
  +---+---+
4 |   |   |
  +---+---+
3 |   |   |
  +---+---+

Only 4 playable dark squares remain!
```

### Objectives

1. **Survive** - Be on Ring 3 when the final shrink happens
2. **Kings dominate** - In tight quarters, king mobility is crucial
3. **Piece count matters again** - With nowhere to run, material advantage wins

### Endgame Scenarios

#### King vs King
- Wraith King wins short-term (more mobility)
- Corrupted King wins long-term (outlasts Wraith's 3-turn timer)

#### King vs Regular Pieces
- King usually wins through superior mobility
- Exception: Multiple regular pieces can corner a king

#### The Final Shrink
When the board is reduced to 4 squares:
- Any piece still on Ring 2 dies
- Whoever has pieces remaining in the center wins
- If both have pieces, the fight continues in the cramped arena

---

## Mastering the Three Pillars

### Pillar 1: The Aggressive Board

**Beginner Mistake**: Treating board shrink as random danger  
**Master Approach**: Using board shrink as a weapon

Techniques:
1. **Shrink Timing** - Know exactly when shrinks happen (turns 5, 10, 15, 20)
2. **Hazard Awareness** - Plan 5 turns ahead for piece positioning
3. **Sacrifice Play** - Sometimes losing a piece to hazard is better than a bad trade

### Pillar 2: Corrupted Promotion

**Beginner Mistake**: Always choosing the same promotion  
**Master Approach**: Contextual decision-making

Evaluation checklist:
- [ ] Do I have a piece on a hazard tile? (Free Corrupted King)
- [ ] Do I need immediate impact? (Wraith King)
- [ ] Am I ahead in material? (Can afford Corrupted cost)
- [ ] Is the board about to shrink? (Wraith might die to shrink anyway)

### Pillar 3: Compulsory Carnage

**Beginner Mistake**: Only thinking about your captures  
**Master Approach**: Calculating opponent's forced captures

The "Capture Map":
1. Before moving, identify ALL possible captures for both sides
2. Count the chain length for each capture path
3. Determine if any path ends on a hazard
4. Make the move that forces opponent into the worst capture

---

## Advanced Tactics

### The Poisoned Piece

Deliberately place a piece where capturing it leads to disaster:

```
Setup: Your piece on (3,3), hazard on (5,5)
       Opponent piece on (4,4)

If opponent captures: They land on hazard and die
If opponent doesn't capture: Illegal move (Compulsory Carnage)

Opponent loses either way.
```

### The Defection Bomb

Use Corrupted King promotion strategically:

```
Scenario: You have a piece deep in enemy territory about to die

1. Promote another piece to Corrupted King
2. Choose the doomed piece as the defector
3. That piece is now OPPONENT's problem (might block their moves)
```

### The Wraith Gambit

Use Wraith King's 3-turn timer as a feature:

```
Scenario: Turn 17, board shrink on Turn 20

1. Promote to Wraith King
2. Wraith dies on Turn 20 anyway (same turn as shrink)
3. You got 3 turns of unlimited movement for "free"
```

### Multi-Jump Manipulation

Force opponent into a specific multi-jump path:

```
Arrange your pieces so:
- Opponent has only ONE legal capture chain
- That chain ends exactly where you want them
- Set up your counter-attack before they even capture
```

---

## Common Mistakes

### 1. Edge Camping
**Mistake**: Keeping pieces on the outer edges  
**Why it's bad**: Those pieces die on Turn 5  
**Fix**: Prioritize central development from Turn 1

### 2. Promotion Obsession
**Mistake**: Racing to promote as fast as possible  
**Why it's bad**: Promotion has heavy costs in Entropy Checkers  
**Fix**: Only promote when the benefit outweighs the cost

### 3. Capture Blindness
**Mistake**: Taking captures without calculating the full chain  
**Why it's bad**: You might be forced into a losing position  
**Fix**: Always trace the complete capture path before committing

### 4. Ignoring the Clock
**Mistake**: Playing as if the board won't shrink  
**Why it's bad**: Sudden piece losses to hazards  
**Fix**: Every move should account for upcoming shrink events

### 5. Symmetric Thinking
**Mistake**: Assuming both sides have equal options  
**Why it's bad**: Board shrink affects players asymmetrically based on piece positions  
**Fix**: Evaluate each player's "shrink exposure" separately

---

## Final Wisdom

> *"In Entropy Checkers, you're not trying to win—you're trying to lose more slowly than your opponent while the universe collapses around you both."*

The best Entropy Checkers players understand:

1. **Every piece is temporary** - The board will take them eventually
2. **Aggression is survival** - Passive play means death by entropy
3. **Sacrifice is strategy** - Sometimes the best move is to lose intentionally
4. **The board always wins** - Your job is to make sure it takes your opponent first

Good luck. The board is watching.
