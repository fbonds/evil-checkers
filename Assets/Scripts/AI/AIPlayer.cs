using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;
using EntropyCheckers.Presentation;

namespace EntropyCheckers.AI
{
    public class AIPlayer : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private Difficulty difficulty = Difficulty.Medium;
        [SerializeField] private float thinkingDelay = 0.5f;
        [SerializeField] private Player aiPlayerColor = Player.Red;

        private GameManager gameManager;
        private PieceRenderer pieceRenderer;
        private MinimaxEngine minimaxEngine;
        private DifficultyController difficultyController;
        
        private bool isThinking;

        public Difficulty CurrentDifficulty
        {
            get => difficulty;
            set
            {
                difficulty = value;
                if (difficultyController != null)
                {
                    difficultyController.CurrentDifficulty = value;
                }
            }
        }

        public void Initialize(GameManager gameManager, PieceRenderer pieceRenderer)
        {
            this.gameManager = gameManager;
            this.pieceRenderer = pieceRenderer;

            minimaxEngine = new MinimaxEngine();
            difficultyController = new DifficultyController(difficulty);

            gameManager.OnTurnChanged += HandleTurnChanged;
            gameManager.OnPromotionRequired += HandlePromotionRequired;
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnTurnChanged -= HandleTurnChanged;
                gameManager.OnPromotionRequired -= HandlePromotionRequired;
            }
        }

        private void HandleTurnChanged(Player currentPlayer)
        {
            if (currentPlayer == aiPlayerColor && gameManager.State == GameState.WaitingForMove)
            {
                StartCoroutine(ThinkAndMove());
            }
        }

        private void HandlePromotionRequired(Piece piece)
        {
            if (piece.Owner == aiPlayerColor)
            {
                StartCoroutine(HandleAIPromotion(piece));
            }
        }

        private IEnumerator ThinkAndMove()
        {
            if (isThinking) yield break;
            isThinking = true;

            // Add a small delay to make AI feel more natural
            yield return new WaitForSeconds(thinkingDelay);

            if (gameManager.State != GameState.WaitingForMove || 
                gameManager.CurrentPlayer != aiPlayerColor)
            {
                isThinking = false;
                yield break;
            }

            // Find best moves
            int depth = difficultyController.GetSearchDepth();
            var rankedMoves = minimaxEngine.FindBestMoves(gameManager.Board, aiPlayerColor, depth);

            if (rankedMoves.Count == 0)
            {
                Debug.Log("AI has no legal moves!");
                isThinking = false;
                yield break;
            }

            // Select move based on difficulty
            var selectedMove = difficultyController.SelectMove(rankedMoves);

            if (selectedMove != null)
            {
                Debug.Log($"AI ({difficulty}) selected: {selectedMove}");
                
                // Animate the piece
                pieceRenderer.MovePiece(selectedMove.Move.Piece, selectedMove.Move.To, true);
                
                // Small delay for animation
                yield return new WaitForSeconds(0.3f);

                // Submit the move
                gameManager.SubmitMove(selectedMove.Move);
            }

            isThinking = false;
        }

        private IEnumerator HandleAIPromotion(Piece piece)
        {
            yield return new WaitForSeconds(0.3f);

            // AI promotion strategy:
            // - If we have pieces on hazard tiles, pick Corrupted and sacrifice the hazard piece
            // - If board is about to shrink and we have pieces in danger, sacrifice those
            // - Otherwise, favor Wraith King for aggressive play on Hard+, Corrupted on Easy/Medium

            var defectorCandidates = gameManager.GetValidDefectorCandidates();
            
            // Look for pieces on hazard tiles (free sacrifice)
            Piece hazardPiece = null;
            foreach (var candidate in defectorCandidates)
            {
                var tile = gameManager.Board.GetTile(candidate.Position);
                if (tile != null && tile.State == TileState.Hazard)
                {
                    hazardPiece = candidate;
                    break;
                }
            }

            // Look for pieces on soon-to-be-hazard tiles
            if (hazardPiece == null)
            {
                int nextHazardRing = gameManager.Board.CurrentShrinkRing + 1;
                foreach (var candidate in defectorCandidates)
                {
                    var tile = gameManager.Board.GetTile(candidate.Position);
                    if (tile != null && tile.GetRingLevel() == nextHazardRing)
                    {
                        hazardPiece = candidate;
                        break;
                    }
                }
            }

            if (hazardPiece != null && defectorCandidates.Count > 0)
            {
                // Free sacrifice available - choose Corrupted King
                Debug.Log($"AI chooses Corrupted King, sacrificing piece at {hazardPiece.Position}");
                gameManager.SubmitPromotion(PromotionChoice.CorruptedKing, hazardPiece);
            }
            else if (difficulty >= Difficulty.Hard && defectorCandidates.Count >= 3)
            {
                // Aggressive play - Wraith King for immediate impact
                Debug.Log("AI chooses Wraith King for aggressive play");
                gameManager.SubmitPromotion(PromotionChoice.WraithKing);
            }
            else if (defectorCandidates.Count > 0)
            {
                // Pick Corrupted King, sacrifice the least valuable piece (furthest from promotion)
                Piece worstPiece = FindWorstPiece(defectorCandidates);
                Debug.Log($"AI chooses Corrupted King, sacrificing piece at {worstPiece.Position}");
                gameManager.SubmitPromotion(PromotionChoice.CorruptedKing, worstPiece);
            }
            else
            {
                // No pieces to sacrifice, must choose Wraith
                Debug.Log("AI chooses Wraith King (no pieces to sacrifice)");
                gameManager.SubmitPromotion(PromotionChoice.WraithKing);
            }
        }

        private Piece FindWorstPiece(List<Piece> pieces)
        {
            Piece worst = pieces[0];
            float worstScore = float.MaxValue;

            foreach (var piece in pieces)
            {
                float score = 0;

                // Regular pieces less valuable than kings
                if (piece.Type == PieceType.Regular)
                {
                    score -= 50;
                }
                else if (piece.Type == PieceType.WraithKing)
                {
                    // Wraith kings about to die are good sacrifices
                    score -= 100 + (3 - piece.WraithTurnsRemaining) * 50;
                }

                // Pieces far from center are less valuable
                float centerDist = Mathf.Abs(piece.Position.x - 3.5f) + Mathf.Abs(piece.Position.y - 3.5f);
                score -= centerDist * 5;

                // Pieces on edges are less valuable
                if (piece.Position.x == 0 || piece.Position.x == 7)
                {
                    score -= 20;
                }

                if (score < worstScore)
                {
                    worstScore = score;
                    worst = piece;
                }
            }

            return worst;
        }

        /// <summary>
        /// Forces the AI to make a move immediately (for testing).
        /// </summary>
        [ContextMenu("Force AI Move")]
        public void ForceMove()
        {
            if (gameManager != null && gameManager.CurrentPlayer == aiPlayerColor)
            {
                StartCoroutine(ThinkAndMove());
            }
        }
    }
}
