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
        private Camera mainCamera;

        private Piece selectedPiece;
        private List<Move> currentLegalMoves;
        private Dictionary<Vector2Int, Move> movesByDestination;

        public void Initialize(GameManager gameManager, BoardRenderer boardRenderer, PieceRenderer pieceRenderer)
        {
            this.gameManager = gameManager;
            this.boardRenderer = boardRenderer;
            this.pieceRenderer = pieceRenderer;
            this.mainCamera = Camera.main;

            currentLegalMoves = new List<Move>();
            movesByDestination = new Dictionary<Vector2Int, Move>();

            gameManager.OnTurnChanged += HandleTurnChanged;
            gameManager.OnMoveExecuted += HandleMoveExecuted;
            gameManager.OnGameOver += HandleGameOver;
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
            if (gameManager == null || gameManager.State != GameState.WaitingForMove)
            {
                return;
            }

            if (!gameManager.IsHumanTurn())
            {
                return;
            }

            var mouse = Mouse.current;
            if (mouse == null) return;

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
                Debug.Log("Cannot select this piece - no legal moves available.");
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
