using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;

namespace EntropyCheckers.AI
{
    public class MinimaxEngine
    {
        private BoardEvaluator evaluator;
        private MoveGenerator moveGenerator;
        private MoveValidator moveValidator;
        
        private int nodesEvaluated;
        private int maxDepth;

        public MinimaxEngine()
        {
            evaluator = new BoardEvaluator();
            moveGenerator = new MoveGenerator();
            moveValidator = new MoveValidator(moveGenerator);
        }

        /// <summary>
        /// Finds the best move for the given player using minimax with alpha-beta pruning.
        /// Returns a list of moves sorted by score (best first).
        /// </summary>
        public List<ScoredMove> FindBestMoves(Board board, Player player, int depth)
        {
            nodesEvaluated = 0;
            maxDepth = depth;

            var legalMoves = moveValidator.GetLegalMoves(board, player);
            if (legalMoves.Count == 0)
            {
                return new List<ScoredMove>();
            }

            var scoredMoves = new List<ScoredMove>();

            foreach (var move in legalMoves)
            {
                // Create a copy of the board and apply the move
                var boardCopy = CloneBoard(board);
                ApplyMove(boardCopy, move);

                // Evaluate with minimax
                float score = Minimax(
                    boardCopy, 
                    depth - 1, 
                    float.NegativeInfinity, 
                    float.PositiveInfinity, 
                    false, 
                    player
                );

                scoredMoves.Add(new ScoredMove(move, score));
            }

            // Sort by score (highest first for maximizing player)
            scoredMoves = scoredMoves.OrderByDescending(m => m.Score).ToList();

            Debug.Log($"AI evaluated {nodesEvaluated} nodes at depth {depth}");

            return scoredMoves;
        }

        private float Minimax(Board board, int depth, float alpha, float beta, bool maximizing, Player aiPlayer)
        {
            nodesEvaluated++;

            Player currentPlayer = maximizing ? aiPlayer : (aiPlayer == Player.Black ? Player.Red : Player.Black);

            // Terminal conditions
            if (depth == 0)
            {
                return evaluator.Evaluate(board, aiPlayer);
            }

            var legalMoves = moveValidator.GetLegalMoves(board, currentPlayer);
            
            // No legal moves = loss for current player
            if (legalMoves.Count == 0)
            {
                return maximizing ? float.NegativeInfinity : float.PositiveInfinity;
            }

            // Check for elimination
            if (board.CountPieces(currentPlayer) == 0)
            {
                return maximizing ? float.NegativeInfinity : float.PositiveInfinity;
            }

            if (maximizing)
            {
                float maxEval = float.NegativeInfinity;

                foreach (var move in legalMoves)
                {
                    var boardCopy = CloneBoard(board);
                    ApplyMove(boardCopy, move);

                    float eval = Minimax(boardCopy, depth - 1, alpha, beta, false, aiPlayer);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                        break; // Beta cutoff
                }

                return maxEval;
            }
            else
            {
                float minEval = float.PositiveInfinity;

                foreach (var move in legalMoves)
                {
                    var boardCopy = CloneBoard(board);
                    ApplyMove(boardCopy, move);

                    float eval = Minimax(boardCopy, depth - 1, alpha, beta, true, aiPlayer);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                        break; // Alpha cutoff
                }

                return minEval;
            }
        }

        private Board CloneBoard(Board original)
        {
            var clone = new Board();
            
            // Copy tile states
            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    var origTile = original.GetTile(x, y);
                    var cloneTile = clone.GetTile(x, y);
                    cloneTile.SetState(origTile.State);
                }
            }

            // Copy pieces
            foreach (var piece in original.GetAllAlivePieces())
            {
                var newPiece = new Piece(piece.Owner, piece.Position);
                
                if (piece.Type == PieceType.King)
                {
                    newPiece.PromoteToKing();
                }
                else if (piece.Type == PieceType.WraithKing)
                {
                    newPiece.PromoteToWraithKing();
                    // Adjust wraith timer
                    for (int i = 3; i > piece.WraithTurnsRemaining; i--)
                    {
                        newPiece.TickWraithTimer();
                    }
                }

                var tile = clone.GetTile(piece.Position);
                tile.PlacePiece(newPiece);
            }

            return clone;
        }

        private void ApplyMove(Board board, Move move)
        {
            var fromTile = board.GetTile(move.From);
            var toTile = board.GetTile(move.To);
            var piece = fromTile.OccupyingPiece;

            if (piece == null) return;

            // Remove from original position
            fromTile.RemovePiece();

            // Remove captured pieces
            foreach (var captured in move.CapturedPieces)
            {
                var capTile = board.GetTile(captured.Position);
                if (capTile != null && capTile.OccupyingPiece != null)
                {
                    capTile.OccupyingPiece.Destroy();
                    capTile.RemovePiece();
                }
            }

            // Place at new position
            piece.SetPosition(move.To);
            toTile.PlacePiece(piece);

            // Check for hazard death
            if (toTile.State == TileState.Hazard)
            {
                piece.Destroy();
                toTile.RemovePiece();
            }

            // Simple promotion (AI always picks Corrupted King for now - more strategic)
            if (piece.Type == PieceType.Regular && board.IsPromotionRow(move.To, piece.Owner))
            {
                piece.PromoteToKing();
            }
        }
    }

    public class ScoredMove
    {
        public Move Move { get; }
        public float Score { get; }

        public ScoredMove(Move move, float score)
        {
            Move = move;
            Score = score;
        }

        public override string ToString()
        {
            return $"{Move} (Score: {Score:F1})";
        }
    }
}
