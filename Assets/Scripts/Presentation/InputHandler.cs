using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EntropyCheckers.Core;
using EntropyCheckers.Game;

namespace EntropyCheckers.Presentation
{
    public class InputHandler : MonoBehaviour
    {
        private GameManager gameManager;
        private BoardRenderer boardRenderer;
        private PieceRenderer pieceRenderer;
        private GameUI gameUI;
        private Camera mainCamera;

        private Piece selectedPiece;
        private List<Move> currentLegalMoves;
        private Dictionary<Vector2Int, Move> movesByDestination;
        
        // Forced capture auto-execution
        private bool pendingForcedCapture;
        private float forcedCaptureDelay;
        private Move forcedMove;
        private const float ForcedCaptureWaitTime = 1.2f;

        public void Initialize(GameManager gameManager, BoardRenderer boardRenderer, PieceRenderer pieceRenderer, GameUI gameUI = null)
        {
            this.gameManager = gameManager;
            this.boardRenderer = boardRenderer;
            this.pieceRenderer = pieceRenderer;
            this.gameUI = gameUI;
            this.mainCamera = Camera.main;

            currentLegalMoves = new List<Move>();
            movesByDestination = new Dictionary<Vector2Int, Move>();

            gameManager.OnTurnChanged += HandleTurnChanged;
            gameManager.OnMoveExecuted += HandleMoveExecuted;
            gameManager.OnGameOver += HandleGameOver;
        }
        
        public void SetGameUI(GameUI ui)
        {
            this.gameUI = ui;
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnTurnChanged -= HandleTurnChanged;
                gameManager.OnMoveExecuted -= HandleMoveExecuted;
                gameManager.OnGameOver -= HandleGameOver;
            }
        }

        private void Update()
        {
            // Handle pending forced capture
            if (pendingForcedCapture)
            {
                forcedCaptureDelay -= Time.deltaTime;
                if (forcedCaptureDelay <= 0)
                {
                    ExecuteForcedCapture();
                }
                return; // Don't allow input during forced capture
            }
            
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Always allow clicks for debugging - check state inside HandleClick
            if (mouse.leftButton.wasPressedThisFrame)
            {
                HandleClick();
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                ClearSelection();
            }
        }

        private void HandleClick()
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2Int boardPos = boardRenderer.WorldToBoard(mouseWorldPos);

            Debug.Log($"Click at board position: {boardPos}, GameState: {gameManager?.State}, CurrentPlayer: {gameManager?.CurrentPlayer}");

            if (gameManager == null || gameManager.State != GameState.WaitingForMove)
            {
                Debug.Log("Cannot interact - game not in WaitingForMove state");
                return;
            }

            if (!gameManager.IsHumanTurn())
            {
                Debug.Log("Cannot interact - not human's turn");
                return;
            }

            if (!IsValidBoardPosition(boardPos)) return;

            var tile = gameManager.Board.GetTile(boardPos);
            if (tile == null) return;

            // Check if clicking on a destination for the selected piece
            if (selectedPiece != null && movesByDestination.ContainsKey(boardPos))
            {
                ExecuteMove(movesByDestination[boardPos]);
                return;
            }

            // Check if clicking on a piece
            if (tile.IsOccupied)
            {
                var clickedPiece = tile.OccupyingPiece;
                
                if (clickedPiece.Owner == gameManager.CurrentPlayer)
                {
                    SelectPiece(clickedPiece);
                }
                else if (selectedPiece != null)
                {
                    // Clicked on enemy piece - check if it's part of a capture move destination
                    // (This shouldn't happen since captures land on empty squares, but clear selection)
                    ClearSelection();
                }
            }
            else if (selectedPiece != null)
            {
                // Clicked on empty non-destination tile, clear selection
                ClearSelection();
            }
        }

        private void SelectPiece(Piece piece)
        {
            if (!gameManager.CanSelectPiece(piece))
            {
                // Debug: show why no moves
                var allMoves = gameManager.GetLegalMoves();
                Debug.Log($"Cannot select this piece at {piece.Position} - no legal moves available.");
                Debug.Log($"Total legal moves for {gameManager.CurrentPlayer}: {allMoves.Count}");
                foreach (var m in allMoves)
                {
                    Debug.Log($"  Available move: {m}");
                }
                return;
            }

            ClearSelection();

            selectedPiece = piece;
            currentLegalMoves = gameManager.GetLegalMovesForPiece(piece);
            
            // Build destination lookup
            movesByDestination.Clear();
            foreach (var move in currentLegalMoves)
            {
                movesByDestination[move.To] = move;
            }

            // Highlight selected piece
            boardRenderer.HighlightTile(piece.Position, HighlightType.Selected);

            // Highlight valid destinations
            foreach (var move in currentLegalMoves)
            {
                var highlightType = move.IsCapture ? HighlightType.CaptureMove : HighlightType.ValidMove;
                boardRenderer.HighlightTile(move.To, highlightType);
            }

            Debug.Log($"Selected {piece.Owner} {piece.Type} at {piece.Position}. {currentLegalMoves.Count} legal moves.");
        }

        private void ClearSelection()
        {
            selectedPiece = null;
            currentLegalMoves.Clear();
            movesByDestination.Clear();
            boardRenderer.ClearAllHighlights();
        }

        private void ExecuteMove(Move move)
        {
            Debug.Log($"Executing move: {move}");
            
            // Animate the piece movement
            foreach (var pos in move.JumpPath)
            {
                if (pos != move.From)
                {
                    pieceRenderer.MovePiece(move.Piece, pos, true);
                }
            }

            ClearSelection();
            gameManager.SubmitMove(move);
        }

        private void HandleTurnChanged(Player newPlayer)
        {
            ClearSelection();
            Debug.Log($"Turn changed to {newPlayer}. Turn {gameManager.TurnCount}");
            
            int turnsUntilShrink = gameManager.TurnsUntilShrink();
            if (turnsUntilShrink <= 2)
            {
                Debug.Log($"Warning: Board shrinks in {turnsUntilShrink} turn(s)!");
            }
            
            // Check for forced capture on human's turn
            if (gameManager.IsHumanTurn())
            {
                CheckForForcedCapture();
            }
        }
        
        private void CheckForForcedCapture()
        {
            var legalMoves = gameManager.GetLegalMoves();
            
            // If there's exactly one legal move and it's a capture, it's forced
            if (legalMoves.Count == 1 && legalMoves[0].IsCapture)
            {
                TriggerForcedCapture(legalMoves[0]);
            }
            // Also check if ALL moves are captures (meaning player must capture, even if choice exists)
            else if (legalMoves.Count > 0 && legalMoves.TrueForAll(m => m.IsCapture))
            {
                // Multiple capture options - highlight and notify but don't auto-execute
                if (gameUI != null)
                {
                    gameUI.ShowForcedCaptureNotification();
                }
                // Auto-select a piece that can capture to help the player
                SelectPiece(legalMoves[0].Piece);
            }
        }
        
        private void TriggerForcedCapture(Move move)
        {
            forcedMove = move;
            pendingForcedCapture = true;
            forcedCaptureDelay = ForcedCaptureWaitTime;
            
            // Show notification
            if (gameUI != null)
            {
                gameUI.ShowForcedCaptureNotification();
            }
            
            // Highlight the forced move
            boardRenderer.HighlightTile(move.Piece.Position, HighlightType.Selected);
            boardRenderer.HighlightTile(move.To, HighlightType.CaptureMove);
            
            Debug.Log($"Forced capture: {move}");
        }
        
        private void ExecuteForcedCapture()
        {
            pendingForcedCapture = false;
            
            if (forcedMove != null)
            {
                // Animate and execute
                pieceRenderer.MovePiece(forcedMove.Piece, forcedMove.To, true);
                ClearSelection();
                gameManager.SubmitMove(forcedMove);
                forcedMove = null;
            }
        }

        private void HandleMoveExecuted(Move move)
        {
            // Update piece visuals after move
            pieceRenderer.UpdatePieceVisual(move.Piece);
        }

        private void HandleGameOver(Player winner)
        {
            ClearSelection();
            Debug.Log($"Game Over! {winner} wins!");
        }

        private bool IsValidBoardPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < Board.Size && 
                   pos.y >= 0 && pos.y < Board.Size;
        }
    }
}
