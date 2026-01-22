using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.AI
{
    public class BoardEvaluator
    {
        // Piece values
        private const float RegularPieceValue = 100f;
        private const float KingValue = 175f;
        private const float WraithKingValue = 200f;

        // Position weights
        private const float CenterControlBonus = 10f;
        private const float AdvancementBonus = 5f;
        private const float BackRowBonus = 15f;
        private const float HazardPenalty = -50f;
        private const float EdgePenalty = -5f;

        // Strategic weights
        private const float MobilityWeight = 2f;
        private const float KingProximityBonus = 3f;

        /// <summary>
        /// Evaluates the board position from the perspective of the given player.
        /// Positive scores favor the player, negative scores favor the opponent.
        /// </summary>
        public float Evaluate(Board board, Player perspective)
        {
            float score = 0f;

            // Material evaluation
            score += EvaluateMaterial(board, perspective);

            // Positional evaluation
            score += EvaluatePositions(board, perspective);

            // Hazard awareness
            score += EvaluateHazardExposure(board, perspective);

            return score;
        }

        private float EvaluateMaterial(Board board, Player perspective)
        {
            float score = 0f;
            Player opponent = perspective == Player.Black ? Player.Red : Player.Black;

            foreach (var piece in board.GetPieces(perspective))
            {
                score += GetPieceValue(piece);
            }

            foreach (var piece in board.GetPieces(opponent))
            {
                score -= GetPieceValue(piece);
            }

            return score;
        }

        private float GetPieceValue(Piece piece)
        {
            switch (piece.Type)
            {
                case PieceType.WraithKing:
                    // Wraith king value decreases as it nears death
                    return WraithKingValue * (piece.WraithTurnsRemaining / 3f);
                case PieceType.King:
                    return KingValue;
                default:
                    return RegularPieceValue;
            }
        }

        private float EvaluatePositions(Board board, Player perspective)
        {
            float score = 0f;
            Player opponent = perspective == Player.Black ? Player.Red : Player.Black;

            foreach (var piece in board.GetPieces(perspective))
            {
                score += EvaluatePiecePosition(piece, board);
            }

            foreach (var piece in board.GetPieces(opponent))
            {
                score -= EvaluatePiecePosition(piece, board);
            }

            return score;
        }

        private float EvaluatePiecePosition(Piece piece, Board board)
        {
            float score = 0f;
            Vector2Int pos = piece.Position;

            // Center control (squares closer to center are more valuable)
            float centerX = Mathf.Abs(pos.x - 3.5f);
            float centerY = Mathf.Abs(pos.y - 3.5f);
            float centerDistance = centerX + centerY;
            score += (7 - centerDistance) * CenterControlBonus;

            // Advancement bonus for regular pieces
            if (piece.Type == PieceType.Regular)
            {
                int advancement = piece.Owner == Player.Black ? pos.y : (7 - pos.y);
                score += advancement * AdvancementBonus;

                // Proximity to king row
                int distanceToKingRow = piece.Owner == Player.Black ? (7 - pos.y) : pos.y;
                if (distanceToKingRow <= 2)
                {
                    score += (3 - distanceToKingRow) * KingProximityBonus;
                }
            }

            // Back row defense bonus (protects against opponent kings)
            if ((piece.Owner == Player.Black && pos.y == 0) ||
                (piece.Owner == Player.Red && pos.y == 7))
            {
                score += BackRowBonus;
            }

            // Edge penalty (edge pieces are less mobile)
            if (pos.x == 0 || pos.x == 7)
            {
                score += EdgePenalty;
            }

            return score;
        }

        private float EvaluateHazardExposure(Board board, Player perspective)
        {
            float score = 0f;
            Player opponent = perspective == Player.Black ? Player.Red : Player.Black;
            int currentRing = board.CurrentShrinkRing;

            foreach (var piece in board.GetPieces(perspective))
            {
                var tile = board.GetTile(piece.Position);
                if (tile != null)
                {
                    // Piece on hazard tile
                    if (tile.State == TileState.Hazard)
                    {
                        score += HazardPenalty;
                    }
                    // Piece on ring that will become hazard next
                    else if (tile.GetRingLevel() == currentRing + 1)
                    {
                        score += HazardPenalty * 0.5f;
                    }
                }
            }

            foreach (var piece in board.GetPieces(opponent))
            {
                var tile = board.GetTile(piece.Position);
                if (tile != null)
                {
                    if (tile.State == TileState.Hazard)
                    {
                        score -= HazardPenalty; // Good for us if opponent is on hazard
                    }
                    else if (tile.GetRingLevel() == currentRing + 1)
                    {
                        score -= HazardPenalty * 0.5f;
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Quick material-only evaluation for move ordering.
        /// </summary>
        public float QuickEvaluate(Board board, Player perspective)
        {
            return EvaluateMaterial(board, perspective);
        }
    }
}
