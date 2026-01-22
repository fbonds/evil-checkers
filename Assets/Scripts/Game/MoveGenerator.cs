using System.Collections.Generic;
using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.Game
{
    public class MoveGenerator
    {
        private static readonly Vector2Int[] DiagonalDirections = new Vector2Int[]
        {
            new Vector2Int(-1, 1),   // Forward-left (for black)
            new Vector2Int(1, 1),    // Forward-right (for black)
            new Vector2Int(-1, -1),  // Backward-left
            new Vector2Int(1, -1)    // Backward-right
        };

        public List<Move> GenerateAllMoves(Board board, Player player)
        {
            var allMoves = new List<Move>();
            var pieces = board.GetPieces(player);

            foreach (var piece in pieces)
            {
                var pieceMoves = GenerateMovesForPiece(board, piece);
                allMoves.AddRange(pieceMoves);
            }

            return allMoves;
        }

        public List<Move> GenerateMovesForPiece(Board board, Piece piece)
        {
            var moves = new List<Move>();

            var captureMoves = GenerateCaptureMoves(board, piece);
            if (captureMoves.Count > 0)
            {
                moves.AddRange(captureMoves);
            }
            else
            {
                var simpleMoves = GenerateSimpleMoves(board, piece);
                moves.AddRange(simpleMoves);
            }

            return moves;
        }

        private List<Move> GenerateSimpleMoves(Board board, Piece piece)
        {
            var moves = new List<Move>();
            Vector2Int from = piece.Position;

            foreach (var direction in DiagonalDirections)
            {
                if (!piece.CanMoveInDirection(direction)) continue;

                if (piece.Type == PieceType.WraithKing)
                {
                    // Wraith kings can move any distance diagonally
                    for (int distance = 1; distance < Board.Size; distance++)
                    {
                        Vector2Int to = from + direction * distance;
                        var tile = board.GetTile(to);
                        
                        if (tile == null || tile.State == TileState.Removed) break;
                        if (tile.IsOccupied) break;
                        
                        moves.Add(new Move(piece, from, to));
                    }
                }
                else
                {
                    Vector2Int to = from + direction;
                    var tile = board.GetTile(to);

                    if (tile != null && tile.IsValidForMove)
                    {
                        moves.Add(new Move(piece, from, to));
                    }
                }
            }

            return moves;
        }

        private List<Move> GenerateCaptureMoves(Board board, Piece piece)
        {
            var captureChains = new List<Move>();
            var initialMove = new Move(piece, piece.Position, piece.Position);
            
            FindCaptureChains(board, piece, initialMove, captureChains, new HashSet<Vector2Int>());

            return captureChains;
        }

        private void FindCaptureChains(
            Board board, 
            Piece piece, 
            Move currentChain, 
            List<Move> completedChains,
            HashSet<Vector2Int> capturedPositions)
        {
            Vector2Int currentPos = currentChain.To;
            bool foundCapture = false;

            foreach (var direction in DiagonalDirections)
            {
                if (!piece.CanMoveInDirection(direction) && !piece.IsKing) continue;

                if (piece.Type == PieceType.WraithKing)
                {
                    // Wraith kings can capture at distance
                    for (int distance = 1; distance < Board.Size - 1; distance++)
                    {
                        Vector2Int enemyPos = currentPos + direction * distance;
                        Vector2Int landingPos = currentPos + direction * (distance + 1);

                        var enemyTile = board.GetTile(enemyPos);
                        var landingTile = board.GetTile(landingPos);

                        if (enemyTile == null || landingTile == null) break;
                        if (enemyTile.State == TileState.Removed || landingTile.State == TileState.Removed) break;

                        // Check if there's an obstacle before the enemy
                        bool pathClear = true;
                        for (int d = 1; d < distance; d++)
                        {
                            var midTile = board.GetTile(currentPos + direction * d);
                            if (midTile == null || midTile.IsOccupied || midTile.State == TileState.Removed)
                            {
                                pathClear = false;
                                break;
                            }
                        }
                        if (!pathClear) break;

                        if (enemyTile.IsOccupied && 
                            enemyTile.OccupyingPiece.Owner != piece.Owner &&
                            !capturedPositions.Contains(enemyPos))
                        {
                            // Can capture at further distances if landing is valid
                            for (int landDist = distance + 1; landDist < Board.Size; landDist++)
                            {
                                Vector2Int farLanding = currentPos + direction * landDist;
                                var farTile = board.GetTile(farLanding);
                                
                                if (farTile == null || farTile.State == TileState.Removed) break;
                                if (farTile.IsOccupied) break;

                                foundCapture = true;
                                var newChain = currentChain.Clone();
                                newChain.AddCapture(farLanding, enemyTile.OccupyingPiece);

                                var newCaptured = new HashSet<Vector2Int>(capturedPositions);
                                newCaptured.Add(enemyPos);

                                FindCaptureChains(board, piece, newChain, completedChains, newCaptured);
                            }
                            break; // Found enemy in this direction, stop searching further
                        }

                        if (enemyTile.IsOccupied) break; // Blocked by any piece
                    }
                }
                else
                {
                    // Regular pieces and normal kings capture adjacent
                    Vector2Int enemyPos = currentPos + direction;
                    Vector2Int landingPos = currentPos + direction * 2;

                    var enemyTile = board.GetTile(enemyPos);
                    var landingTile = board.GetTile(landingPos);

                    if (enemyTile == null || landingTile == null) continue;
                    if (enemyTile.State == TileState.Removed || landingTile.State == TileState.Removed) continue;
                    if (!enemyTile.IsOccupied) continue;
                    if (enemyTile.OccupyingPiece.Owner == piece.Owner) continue;
                    if (capturedPositions.Contains(enemyPos)) continue;
                    if (landingTile.IsOccupied) continue;

                    foundCapture = true;
                    var newChain = currentChain.Clone();
                    newChain.AddCapture(landingPos, enemyTile.OccupyingPiece);

                    var newCaptured = new HashSet<Vector2Int>(capturedPositions);
                    newCaptured.Add(enemyPos);

                    FindCaptureChains(board, piece, newChain, completedChains, newCaptured);
                }
            }

            // If no more captures found and we have at least one capture, this is a completed chain
            if (!foundCapture && currentChain.CaptureCount > 0)
            {
                completedChains.Add(currentChain);
            }
        }
    }
}
