# Entropy Checkers - Technical Architecture

## Overview

Entropy Checkers is built in Unity 6 using a clean separation between game logic (pure C# classes) and presentation (MonoBehaviours). This architecture enables:

- Unit testing of game rules without Unity dependencies
- Clear data flow and state management
- Easy AI integration
- Future multiplayer potential

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Pure C# game logic (no Unity dependencies)
│   │   ├── Board.cs
│   │   ├── Tile.cs
│   │   ├── Piece.cs
│   │   └── Move.cs
│   │
│   ├── Game/           # Game flow and rules
│   │   ├── GameManager.cs
│   │   ├── MoveValidator.cs
│   │   ├── MoveGenerator.cs
│   │   └── CapturePathfinder.cs
│   │
│   ├── AI/             # Computer opponent
│   │   ├── AIPlayer.cs
│   │   ├── BoardEvaluator.cs
│   │   ├── MinimaxEngine.cs
│   │   └── DifficultyController.cs
│   │
│   └── Presentation/   # Unity-specific rendering and input
│       ├── BoardRenderer.cs
│       ├── PieceRenderer.cs
│       ├── InputHandler.cs
│       ├── UIManager.cs
│       └── PromotionDialogUI.cs
│
├── Prefabs/
├── Scenes/
├── Sprites/
└── UI/
```

---

## Core Layer

The Core layer contains pure C# classes with no Unity dependencies (except `Vector2Int` for convenience). These classes can be instantiated and tested independently.

### Board.cs

The central game state container.

```csharp
public class Board
{
    // Constants
    public const int Size = 8;
    
    // State
    private Tile[,] tiles;
    private List<Piece> blackPieces;
    private List<Piece> redPieces;
    public int CurrentShrinkRing { get; private set; }
    
    // Events (for Presentation layer to observe)
    public event Action<Tile> OnTileStateChanged;
    public event Action<Piece> OnPieceDestroyed;
    public event Action<Piece> OnPieceDefected;
    public event Action<int> OnBoardShrink;
    
    // Key Methods
    public void SetupInitialPieces();
    public void ExecuteMove(Move move);
    public void ShrinkBoard();
    public void HandleDefection(Piece defector);
    public void TickWraithKings(Player player);
}
```

**Responsibilities:**
- Maintain the 8x8 grid of tiles
- Track all pieces for both players
- Execute moves and captures
- Handle board shrink events
- Manage piece defection for Corrupted Kings
- Track Wraith King lifespans

### Tile.cs

Represents a single square on the board.

```csharp
public enum TileState { Normal, Hazard, Removed }

public class Tile
{
    public Vector2Int Position { get; }
    public TileState State { get; private set; }
    public Piece OccupyingPiece { get; private set; }
    
    // Computed Properties
    public bool IsPlayable;      // Dark square check
    public bool IsOccupied;
    public bool IsValidForMove;  // Playable + Normal + Empty
    
    // Methods
    public void SetState(TileState newState);
    public void PlacePiece(Piece piece);
    public Piece RemovePiece();
    public int GetRingLevel();   // Distance from board edge (0-3)
}
```

**Key Feature - Ring Level:**

```
Ring 0: Edge tiles (x=0, x=7, y=0, y=7)
Ring 1: One step inward
Ring 2: Two steps inward  
Ring 3: Center 2x2 area
```

Ring level determines when a tile becomes hazardous during board shrink.

### Piece.cs

Represents a checker piece.

```csharp
public enum Player { Black, Red }
public enum PieceType { Regular, King, WraithKing }

public class Piece
{
    public Player Owner { get; private set; }
    public PieceType Type { get; protected set; }
    public Vector2Int Position { get; private set; }
    public bool IsAlive { get; private set; }
    public int WraithTurnsRemaining { get; private set; }
    
    // Computed
    public bool IsKing;
    public int ForwardDirection;  // +1 for Black, -1 for Red
    
    // Methods
    public void PromoteToKing();
    public void PromoteToWraithKing();
    public void TickWraithTimer();
    public void Defect();         // Switch owner
    public void Destroy();
}
```

**Wraith King Lifecycle:**
1. `PromoteToWraithKing()` → Sets `WraithTurnsRemaining = 3`
2. Each turn: `TickWraithTimer()` → Decrements counter
3. When counter hits 0 → `Destroy()` called automatically

### Move.cs

Represents a move, including multi-jump capture chains.

```csharp
public class Move
{
    public Piece Piece { get; }
    public Vector2Int From { get; }
    public Vector2Int To { get; }
    public List<Vector2Int> JumpPath { get; }      // Full path for multi-jumps
    public List<Piece> CapturedPieces { get; }
    
    // Computed
    public bool IsCapture;
    public int CaptureCount;
    public bool IsMultiJump;
    
    // Methods
    public void AddCapture(Vector2Int landing, Piece captured);
    public Move Clone();
}
```

**Multi-Jump Representation:**

For a triple jump from (1,0) → (3,2) → (5,4) → (7,6):
- `From = (1,0)`
- `To = (7,6)`
- `JumpPath = [(1,0), (3,2), (5,4), (7,6)]`
- `CapturedPieces = [piece1, piece2, piece3]`

---

## Game Layer

Handles rules enforcement, move generation, and game flow.

### GameManager.cs (Planned)

```csharp
public class GameManager
{
    public Board Board { get; }
    public Player CurrentPlayer { get; private set; }
    public int TurnCount { get; private set; }
    public GameState State { get; private set; }
    
    // Events
    public event Action<Player> OnTurnChanged;
    public event Action<Player> OnGameOver;
    public event Action<Piece, Vector2Int> OnPromotionRequired;
    
    // Methods
    public void StartGame();
    public void SubmitMove(Move move);
    public void SubmitPromotion(Piece piece, PieceType promotionType, Piece defector = null);
    public List<Move> GetLegalMoves();
}
```

**Turn Flow:**
1. Get legal moves for current player
2. Player selects move
3. Execute move
4. Check for promotion
5. Check for win condition
6. Tick Wraith Kings
7. Every 5 turns: Shrink board
8. Switch player

### MoveGenerator.cs (Planned)

```csharp
public class MoveGenerator
{
    public List<Move> GenerateAllMoves(Board board, Player player);
    public List<Move> GenerateMovesForPiece(Board board, Piece piece);
}
```

### MoveValidator.cs (Planned)

```csharp
public class MoveValidator
{
    public bool IsValidMove(Board board, Move move);
    public List<Move> FilterToMaxCaptures(List<Move> moves);  // Compulsory Carnage
}
```

### CapturePathfinder.cs (Planned)

Handles the complex logic of finding all possible capture chains.

```csharp
public class CapturePathfinder
{
    // Find all capture chains starting from a piece
    public List<Move> FindAllCaptureChains(Board board, Piece piece);
    
    // Recursive helper to build chains
    private void ExtendCaptureChain(Board board, Move currentChain, List<Move> completedChains);
}
```

---

## AI Layer

### Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     AIPlayer                             │
│  - Coordinates AI decision making                        │
│  - Applies difficulty-based move selection               │
└─────────────────────┬───────────────────────────────────┘
                      │
        ┌─────────────┴─────────────┐
        ▼                           ▼
┌───────────────────┐     ┌─────────────────────┐
│  MinimaxEngine    │     │ DifficultyController │
│  - Alpha-beta     │     │ - Mistake injection  │
│  - Move scoring   │     │ - Move randomization │
└────────┬──────────┘     └─────────────────────┘
         │
         ▼
┌───────────────────┐
│  BoardEvaluator   │
│  - Position score │
│  - Material count │
│  - Hazard danger  │
└───────────────────┘
```

### BoardEvaluator.cs (Planned)

Evaluates board positions for the AI.

```csharp
public class BoardEvaluator
{
    public float Evaluate(Board board, Player perspective);
}
```

**Evaluation Factors:**
- Material count (weighted by piece type)
- King count
- Position value (center = good, edge = bad)
- Hazard exposure (pieces on soon-to-shrink rings)
- Mobility (number of legal moves)
- Promotion proximity

### MinimaxEngine.cs (Planned)

Standard minimax with alpha-beta pruning.

```csharp
public class MinimaxEngine
{
    public Move FindBestMove(Board board, Player player, int depth);
    
    private float Minimax(Board board, int depth, float alpha, float beta, bool maximizing);
}
```

### DifficultyController.cs (Planned)

The key innovation: AI always calculates optimal moves, but difficulty controls how often it "makes mistakes."

```csharp
public enum Difficulty { Easy, Medium, Hard, Master }

public class DifficultyController
{
    public Move SelectMove(List<Move> rankedMoves, Difficulty difficulty);
}
```

**Difficulty Behavior:**

| Difficulty | Optimal Move % | Mistake Type |
|------------|----------------|--------------|
| Easy | 20% | Often picks worst move |
| Medium | 50% | Sometimes picks suboptimal |
| Hard | 80% | Rarely makes mistakes |
| Master | 100% | Always optimal |

This approach feels more "human" than simply reducing search depth, because the AI still "sees" good moves—it just doesn't always take them.

---

## Presentation Layer

All Unity-specific code lives here. The Presentation layer observes Core/Game events and updates visuals accordingly.

### BoardRenderer.cs (Planned)

```csharp
public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hazardColor;
    [SerializeField] private Color removedColor;
    
    private void OnTileStateChanged(Tile tile);
    private void OnBoardShrink(int ring);
}
```

### PieceRenderer.cs (Planned)

```csharp
public class PieceRenderer : MonoBehaviour
{
    [SerializeField] private Sprite regularSprite;
    [SerializeField] private Sprite kingSprite;
    [SerializeField] private Sprite wraithSprite;
    
    public void UpdateVisual(Piece piece);
    public void AnimateMove(Move move);
    public void AnimateCapture(Piece captured);
}
```

### InputHandler.cs (Planned)

Handles player input for piece selection and move execution.

```csharp
public class InputHandler : MonoBehaviour
{
    public event Action<Piece> OnPieceSelected;
    public event Action<Move> OnMoveSelected;
    
    private void HighlightLegalMoves(List<Move> moves);
}
```

---

## Data Flow

### Move Execution Flow

```
1. InputHandler detects click
         │
         ▼
2. GameManager.GetLegalMoves() called
         │
         ▼
3. MoveGenerator produces all moves
         │
         ▼
4. MoveValidator filters to max-capture moves (Compulsory Carnage)
         │
         ▼
5. InputHandler highlights legal destinations
         │
         ▼
6. Player selects destination
         │
         ▼
7. GameManager.SubmitMove(move)
         │
         ▼
8. Board.ExecuteMove(move)
         │
         ├──► OnPieceDestroyed events (for captures)
         │
         ▼
9. Check for promotion → OnPromotionRequired
         │
         ▼
10. Check win condition → OnGameOver
         │
         ▼
11. If Turn % 5 == 0: Board.ShrinkBoard()
         │
         ├──► OnTileStateChanged events
         ├──► OnPieceDestroyed events (hazard casualties)
         │
         ▼
12. Switch player → OnTurnChanged
```

### Event-Driven Updates

The Presentation layer never directly queries game state. Instead, it subscribes to events:

```csharp
// In BoardRenderer.Start()
board.OnTileStateChanged += HandleTileChange;
board.OnBoardShrink += HandleBoardShrink;
board.OnPieceDestroyed += HandlePieceDestroyed;
board.OnPieceDefected += HandlePieceDefected;
```

This keeps the Core layer completely decoupled from Unity.

---

## Testing Strategy

### Unit Tests (Core Layer)

```csharp
[Test]
public void Tile_GetRingLevel_ReturnsCorrectLevel()
{
    var cornerTile = new Tile(new Vector2Int(0, 0));
    Assert.AreEqual(0, cornerTile.GetRingLevel());
    
    var centerTile = new Tile(new Vector2Int(3, 3));
    Assert.AreEqual(3, centerTile.GetRingLevel());
}

[Test]
public void Board_ShrinkBoard_MakesOuterRingHazardous()
{
    var board = new Board();
    board.ShrinkBoard();
    
    var edgeTile = board.GetTile(0, 0);
    Assert.AreEqual(TileState.Hazard, edgeTile.State);
}

[Test]
public void MoveValidator_EnforcesMaxCapture()
{
    // Setup board with multiple capture options
    // Assert that only max-capture moves are returned as legal
}
```

### Integration Tests (Game Layer)

```csharp
[Test]
public void FullGame_BoardShrinksEveryFiveTurns()
{
    var game = new GameManager();
    game.StartGame();
    
    // Simulate 5 turns
    // Assert board shrink occurred
}
```

---

## Future Considerations

### Multiplayer

The Core layer's pure C# design enables:
- Easy serialization of `Board` state
- Move validation on server
- State synchronization via events

### Replay System

`Move` objects contain complete information for replay:
- Record sequence of `Move` objects
- Reconstruct game by replaying moves

### Variant Rules

The event-driven architecture allows rule modifications:
- Different shrink schedules
- Alternative promotion options
- Custom win conditions

---

## Dependencies

- **Unity 6** (6000.3.5f1)
- **Universal Render Pipeline** (URP) for 2D rendering
- **Unity Input System** for input handling
- **TextMeshPro** for UI text
