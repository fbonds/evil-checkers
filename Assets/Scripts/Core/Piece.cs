using UnityEngine;

namespace EntropyCheckers.Core
{
    public enum Player
    {
        Black,
        Red
    }

    public enum PieceType
    {
        Regular,
        King,
        WraithKing
    }

    public class Piece
    {
        public Player Owner { get; private set; }
        public PieceType Type { get; protected set; }
        public Vector2Int Position { get; private set; }
        public bool IsAlive { get; private set; }
        
        public int WraithTurnsRemaining { get; private set; }
        
        public bool IsKing => Type == PieceType.King || Type == PieceType.WraithKing;
        
        public int ForwardDirection => Owner == Player.Black ? 1 : -1;

        public Piece(Player owner, Vector2Int position)
        {
            Owner = owner;
            Type = PieceType.Regular;
            Position = position;
            IsAlive = true;
            WraithTurnsRemaining = -1;
        }

        public void SetPosition(Vector2Int newPosition)
        {
            Position = newPosition;
        }

        public void PromoteToKing()
        {
            if (Type == PieceType.Regular)
            {
                Type = PieceType.King;
            }
        }

        public void PromoteToWraithKing()
        {
            if (Type == PieceType.Regular)
            {
                Type = PieceType.WraithKing;
                WraithTurnsRemaining = 3;
            }
        }

        public void TickWraithTimer()
        {
            if (Type == PieceType.WraithKing && WraithTurnsRemaining > 0)
            {
                WraithTurnsRemaining--;
                if (WraithTurnsRemaining <= 0)
                {
                    Destroy();
                }
            }
        }

        public void Defect()
        {
            Owner = Owner == Player.Black ? Player.Red : Player.Black;
        }

        public void Destroy()
        {
            IsAlive = false;
        }

        public bool CanMoveInDirection(Vector2Int direction)
        {
            if (IsKing)
            {
                return true;
            }
            return direction.y == ForwardDirection;
        }
    }
}
