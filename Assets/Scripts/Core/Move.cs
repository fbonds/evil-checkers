using System.Collections.Generic;
using UnityEngine;

namespace EntropyCheckers.Core
{
    public class Move
    {
        public Piece Piece { get; private set; }
        public Vector2Int From { get; private set; }
        public Vector2Int To { get; private set; }
        public List<Vector2Int> JumpPath { get; private set; }
        public List<Piece> CapturedPieces { get; private set; }
        
        public bool IsCapture => CapturedPieces.Count > 0;
        public int CaptureCount => CapturedPieces.Count;
        public bool IsMultiJump => CapturedPieces.Count > 1;

        public Move(Piece piece, Vector2Int from, Vector2Int to)
        {
            Piece = piece;
            From = from;
            To = to;
            JumpPath = new List<Vector2Int> { from, to };
            CapturedPieces = new List<Piece>();
        }

        public Move(Piece piece, Vector2Int from, List<Vector2Int> jumpPath, List<Piece> capturedPieces)
        {
            Piece = piece;
            From = from;
            To = jumpPath[jumpPath.Count - 1];
            JumpPath = new List<Vector2Int>(jumpPath);
            CapturedPieces = new List<Piece>(capturedPieces);
        }

        public void AddCapture(Vector2Int landingPosition, Piece capturedPiece)
        {
            JumpPath.Add(landingPosition);
            To = landingPosition;
            CapturedPieces.Add(capturedPiece);
        }

        public Move Clone()
        {
            return new Move(Piece, From, new List<Vector2Int>(JumpPath), new List<Piece>(CapturedPieces));
        }

        public override string ToString()
        {
            if (IsCapture)
            {
                return $"{Piece.Owner} {Piece.Type}: {From} -> {To} (captures {CaptureCount})";
            }
            return $"{Piece.Owner} {Piece.Type}: {From} -> {To}";
        }
    }
}
