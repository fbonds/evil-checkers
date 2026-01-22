using UnityEngine;

namespace EntropyCheckers.Core
{
    public enum TileState
    {
        Normal,
        Hazard,
        Removed
    }

    public class Tile
    {
        public Vector2Int Position { get; private set; }
        public TileState State { get; private set; }
        public Piece OccupyingPiece { get; private set; }
        
        public bool IsPlayable => (Position.x + Position.y) % 2 == 1;
        public bool IsOccupied => OccupyingPiece != null;
        public bool IsValidForMove => IsPlayable && State == TileState.Normal && !IsOccupied;
        public bool IsValidForPiece => IsPlayable && State == TileState.Normal;

        public Tile(Vector2Int position)
        {
            Position = position;
            State = TileState.Normal;
            OccupyingPiece = null;
        }

        public void SetState(TileState newState)
        {
            State = newState;
            
            if (newState == TileState.Removed && OccupyingPiece != null)
            {
                OccupyingPiece.Destroy();
                OccupyingPiece = null;
            }
        }

        public void PlacePiece(Piece piece)
        {
            OccupyingPiece = piece;
            if (piece != null)
            {
                piece.SetPosition(Position);
            }
        }

        public Piece RemovePiece()
        {
            var piece = OccupyingPiece;
            OccupyingPiece = null;
            return piece;
        }

        public int GetRingLevel()
        {
            int distFromLeft = Position.x;
            int distFromRight = 7 - Position.x;
            int distFromBottom = Position.y;
            int distFromTop = 7 - Position.y;
            
            return Mathf.Min(distFromLeft, distFromRight, distFromBottom, distFromTop);
        }
    }
}
