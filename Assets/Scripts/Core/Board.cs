using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntropyCheckers.Core
{
    public class Board
    {
        public const int Size = 8;
        
        private Tile[,] tiles;
        private List<Piece> blackPieces;
        private List<Piece> redPieces;
        
        public int CurrentShrinkRing { get; private set; }
        
        public event Action<Tile> OnTileStateChanged;
        public event Action<Piece> OnPieceDestroyed;
        public event Action<Piece> OnPieceDefected;
        public event Action<int> OnBoardShrink;

        public Board()
        {
            tiles = new Tile[Size, Size];
            blackPieces = new List<Piece>();
            redPieces = new List<Piece>();
            CurrentShrinkRing = -1;
            
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    tiles[x, y] = new Tile(new Vector2Int(x, y));
                }
            }
        }

        public void SetupInitialPieces()
        {
            blackPieces.Clear();
            redPieces.Clear();
            
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    var tile = tiles[x, y];
                    if (tile.IsPlayable)
                    {
                        var piece = new Piece(Player.Black, new Vector2Int(x, y));
                        tile.PlacePiece(piece);
                        blackPieces.Add(piece);
                    }
                }
            }
            
            for (int y = Size - 3; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    var tile = tiles[x, y];
                    if (tile.IsPlayable)
                    {
                        var piece = new Piece(Player.Red, new Vector2Int(x, y));
                        tile.PlacePiece(piece);
                        redPieces.Add(piece);
                    }
                }
            }
        }

        public Tile GetTile(Vector2Int position)
        {
            if (!IsInBounds(position)) return null;
            return tiles[position.x, position.y];
        }

        public Tile GetTile(int x, int y)
        {
            return GetTile(new Vector2Int(x, y));
        }

        public bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x < Size && 
                   position.y >= 0 && position.y < Size;
        }

        public List<Piece> GetPieces(Player player)
        {
            var pieces = player == Player.Black ? blackPieces : redPieces;
            return pieces.FindAll(p => p.IsAlive);
        }

        public List<Piece> GetAllAlivePieces()
        {
            var all = new List<Piece>();
            all.AddRange(GetPieces(Player.Black));
            all.AddRange(GetPieces(Player.Red));
            return all;
        }

        public void ExecuteMove(Move move)
        {
            var fromTile = GetTile(move.From);
            var toTile = GetTile(move.To);
            
            fromTile.RemovePiece();
            
            foreach (var capturedPiece in move.CapturedPieces)
            {
                var captureTile = GetTile(capturedPiece.Position);
                captureTile.RemovePiece();
                capturedPiece.Destroy();
                OnPieceDestroyed?.Invoke(capturedPiece);
            }
            
            toTile.PlacePiece(move.Piece);
            
            if (toTile.State == TileState.Hazard)
            {
                toTile.RemovePiece();
                move.Piece.Destroy();
                OnPieceDestroyed?.Invoke(move.Piece);
            }
        }

        public void ShrinkBoard()
        {
            CurrentShrinkRing++;
            
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    var tile = tiles[x, y];
                    if (tile.GetRingLevel() < CurrentShrinkRing && tile.State == TileState.Hazard)
                    {
                        tile.SetState(TileState.Removed);
                        if (tile.OccupyingPiece != null)
                        {
                            OnPieceDestroyed?.Invoke(tile.OccupyingPiece);
                        }
                        OnTileStateChanged?.Invoke(tile);
                    }
                    else if (tile.GetRingLevel() == CurrentShrinkRing && tile.State == TileState.Normal)
                    {
                        tile.SetState(TileState.Hazard);
                        OnTileStateChanged?.Invoke(tile);
                    }
                }
            }
            
            OnBoardShrink?.Invoke(CurrentShrinkRing);
        }

        public void HandleDefection(Piece defector)
        {
            if (defector.Owner == Player.Black)
            {
                blackPieces.Remove(defector);
                defector.Defect();
                redPieces.Add(defector);
            }
            else
            {
                redPieces.Remove(defector);
                defector.Defect();
                blackPieces.Add(defector);
            }
            
            OnPieceDefected?.Invoke(defector);
        }

        public void TickWraithKings(Player player)
        {
            var pieces = GetPieces(player);
            foreach (var piece in pieces)
            {
                if (piece.Type == PieceType.WraithKing)
                {
                    piece.TickWraithTimer();
                    if (!piece.IsAlive)
                    {
                        var tile = GetTile(piece.Position);
                        tile.RemovePiece();
                        OnPieceDestroyed?.Invoke(piece);
                    }
                }
            }
        }

        public bool IsPromotionRow(Vector2Int position, Player player)
        {
            if (player == Player.Black)
            {
                return position.y == Size - 1;
            }
            return position.y == 0;
        }

        public int CountPieces(Player player)
        {
            return GetPieces(player).Count;
        }

        public void Reset()
        {
            CurrentShrinkRing = -1;
            
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    tiles[x, y] = new Tile(new Vector2Int(x, y));
                }
            }
            
            blackPieces.Clear();
            redPieces.Clear();
        }
    }
}
