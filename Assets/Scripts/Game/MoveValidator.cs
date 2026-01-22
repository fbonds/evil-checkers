using System.Collections.Generic;
using System.Linq;
using EntropyCheckers.Core;

namespace EntropyCheckers.Game
{
    public class MoveValidator
    {
        private MoveGenerator moveGenerator;

        public MoveValidator()
        {
            moveGenerator = new MoveGenerator();
        }

        public MoveValidator(MoveGenerator generator)
        {
            moveGenerator = generator;
        }

        /// <summary>
        /// Gets all legal moves for a player, enforcing Compulsory Carnage rule.
        /// If captures are available, only capture moves are returned.
        /// Among capture moves, only those with maximum captures are returned.
        /// </summary>
        public List<Move> GetLegalMoves(Board board, Player player)
        {
            var allMoves = moveGenerator.GenerateAllMoves(board, player);
            
            if (allMoves.Count == 0)
            {
                return allMoves;
            }

            // Separate captures and simple moves
            var captureMoves = allMoves.Where(m => m.IsCapture).ToList();
            var simpleMoves = allMoves.Where(m => !m.IsCapture).ToList();

            // If captures exist, must capture (standard forced jump rule)
            if (captureMoves.Count > 0)
            {
                // Compulsory Carnage: Must take maximum capture path
                return FilterToMaxCaptures(captureMoves);
            }

            return simpleMoves;
        }

        /// <summary>
        /// Gets legal moves for a specific piece, respecting forced captures.
        /// </summary>
        public List<Move> GetLegalMovesForPiece(Board board, Piece piece)
        {
            var allLegalMoves = GetLegalMoves(board, piece.Owner);
            return allLegalMoves.Where(m => m.Piece == piece).ToList();
        }

        /// <summary>
        /// Filters moves to only include those with maximum capture count.
        /// This enforces the Compulsory Carnage rule.
        /// </summary>
        public List<Move> FilterToMaxCaptures(List<Move> captureMoves)
        {
            if (captureMoves.Count == 0) return captureMoves;

            int maxCaptures = captureMoves.Max(m => m.CaptureCount);
            return captureMoves.Where(m => m.CaptureCount == maxCaptures).ToList();
        }

        /// <summary>
        /// Checks if a specific move is legal given the current board state.
        /// </summary>
        public bool IsLegalMove(Board board, Move move)
        {
            var legalMoves = GetLegalMoves(board, move.Piece.Owner);
            
            return legalMoves.Any(m => 
                m.Piece == move.Piece && 
                m.From == move.From && 
                m.To == move.To &&
                m.CaptureCount == move.CaptureCount);
        }

        /// <summary>
        /// Checks if the player has any legal moves available.
        /// </summary>
        public bool HasLegalMoves(Board board, Player player)
        {
            return GetLegalMoves(board, player).Count > 0;
        }

        /// <summary>
        /// Checks if the player must capture (has capture moves available).
        /// </summary>
        public bool MustCapture(Board board, Player player)
        {
            var allMoves = moveGenerator.GenerateAllMoves(board, player);
            return allMoves.Any(m => m.IsCapture);
        }

        /// <summary>
        /// Gets the maximum capture count available for a player.
        /// Returns 0 if no captures are available.
        /// </summary>
        public int GetMaxCaptureCount(Board board, Player player)
        {
            var allMoves = moveGenerator.GenerateAllMoves(board, player);
            var captureMoves = allMoves.Where(m => m.IsCapture).ToList();
            
            if (captureMoves.Count == 0) return 0;
            
            return captureMoves.Max(m => m.CaptureCount);
        }

        /// <summary>
        /// Checks if a piece can be selected (has legal moves).
        /// </summary>
        public bool CanSelectPiece(Board board, Piece piece)
        {
            return GetLegalMovesForPiece(board, piece).Count > 0;
        }
    }
}
