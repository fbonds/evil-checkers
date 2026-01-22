using System;
using System.Collections.Generic;
using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.Game
{
    public enum GameState
    {
        NotStarted,
        WaitingForMove,
        WaitingForPromotion,
        GameOver
    }

    public enum PromotionChoice
    {
        WraithKing,
        CorruptedKing
    }

    public class GameManager
    {
        public Board Board { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int TurnCount { get; private set; }
        public GameState State { get; private set; }
        public Player? Winner { get; private set; }

        private MoveValidator moveValidator;
        private MoveGenerator moveGenerator;
        private Piece pendingPromotionPiece;

        public const int TurnsPerShrink = 5;

        // Events
        public event Action<Player> OnTurnChanged;
        public event Action<Move> OnMoveExecuted;
        public event Action<Piece> OnPromotionRequired;
        public event Action<Piece, PromotionChoice> OnPromotionComplete;
        public event Action<Player> OnGameOver;
        public event Action OnBoardShrinkWarning; // Fires 1 turn before shrink

        public GameManager()
        {
            Board = new Board();
            moveGenerator = new MoveGenerator();
            moveValidator = new MoveValidator(moveGenerator);
            State = GameState.NotStarted;
        }

        public void StartGame()
        {
            Board.Reset();
            Board.SetupInitialPieces();
            CurrentPlayer = Player.Black; // Black always moves first
            TurnCount = 0;
            State = GameState.WaitingForMove;
            Winner = null;
            pendingPromotionPiece = null;

            Debug.Log("Game started. Black moves first.");
        }

        public List<Move> GetLegalMoves()
        {
            if (State != GameState.WaitingForMove)
            {
                return new List<Move>();
            }
            return moveValidator.GetLegalMoves(Board, CurrentPlayer);
        }

        public List<Move> GetLegalMovesForPiece(Piece piece)
        {
            if (State != GameState.WaitingForMove || piece.Owner != CurrentPlayer)
            {
                return new List<Move>();
            }
            return moveValidator.GetLegalMovesForPiece(Board, piece);
        }

        public bool CanSelectPiece(Piece piece)
        {
            return State == GameState.WaitingForMove && 
                   piece.Owner == CurrentPlayer &&
                   moveValidator.CanSelectPiece(Board, piece);
        }

        public bool SubmitMove(Move move)
        {
            if (State != GameState.WaitingForMove)
            {
                Debug.LogWarning("Cannot submit move: not waiting for move.");
                return false;
            }

            if (move.Piece.Owner != CurrentPlayer)
            {
                Debug.LogWarning("Cannot submit move: not your turn.");
                return false;
            }

            if (!moveValidator.IsLegalMove(Board, move))
            {
                Debug.LogWarning("Cannot submit move: illegal move.");
                return false;
            }

            // Execute the move
            Board.ExecuteMove(move);
            OnMoveExecuted?.Invoke(move);

            // Check for promotion
            if (ShouldPromote(move))
            {
                pendingPromotionPiece = move.Piece;
                State = GameState.WaitingForPromotion;
                OnPromotionRequired?.Invoke(move.Piece);
                return true;
            }

            EndTurn();
            return true;
        }

        private bool ShouldPromote(Move move)
        {
            // Only regular pieces can promote
            if (move.Piece.Type != PieceType.Regular) return false;
            
            // Check if piece reached promotion row
            return Board.IsPromotionRow(move.To, move.Piece.Owner);
        }

        public bool SubmitPromotion(PromotionChoice choice, Piece defector = null)
        {
            if (State != GameState.WaitingForPromotion || pendingPromotionPiece == null)
            {
                Debug.LogWarning("Cannot submit promotion: not waiting for promotion.");
                return false;
            }

            if (choice == PromotionChoice.CorruptedKing)
            {
                if (defector == null)
                {
                    Debug.LogWarning("Corrupted King requires selecting a piece to defect.");
                    return false;
                }

                if (defector.Owner != CurrentPlayer || defector == pendingPromotionPiece)
                {
                    Debug.LogWarning("Invalid defector selection.");
                    return false;
                }

                // Promote to Corrupted King (standard king)
                pendingPromotionPiece.PromoteToKing();
                
                // Defect the selected piece
                Board.HandleDefection(defector);
                
                Debug.Log($"{CurrentPlayer} promoted to Corrupted King. {defector.Position} defected!");
            }
            else // WraithKing
            {
                pendingPromotionPiece.PromoteToWraithKing();
                Debug.Log($"{CurrentPlayer} promoted to Wraith King (3 turns remaining).");
            }

            OnPromotionComplete?.Invoke(pendingPromotionPiece, choice);
            pendingPromotionPiece = null;

            EndTurn();
            return true;
        }

        public List<Piece> GetValidDefectorCandidates()
        {
            if (pendingPromotionPiece == null) return new List<Piece>();

            var candidates = Board.GetPieces(CurrentPlayer);
            candidates.RemoveAll(p => p == pendingPromotionPiece);
            return candidates;
        }

        private void EndTurn()
        {
            // Tick Wraith Kings for current player (they lose a turn of life)
            Board.TickWraithKings(CurrentPlayer);

            // Increment turn counter
            TurnCount++;

            // Check for board shrink
            if (TurnCount > 0 && TurnCount % TurnsPerShrink == 0)
            {
                Board.ShrinkBoard();
                Debug.Log($"Board shrunk! Ring {Board.CurrentShrinkRing} is now hazardous.");
            }
            else if (TurnCount > 0 && TurnCount % TurnsPerShrink == TurnsPerShrink - 1)
            {
                OnBoardShrinkWarning?.Invoke();
            }

            // Switch player
            CurrentPlayer = CurrentPlayer == Player.Black ? Player.Red : Player.Black;

            // Check for game over
            if (CheckGameOver())
            {
                return;
            }

            State = GameState.WaitingForMove;
            OnTurnChanged?.Invoke(CurrentPlayer);
        }

        private bool CheckGameOver()
        {
            int blackCount = Board.CountPieces(Player.Black);
            int redCount = Board.CountPieces(Player.Red);

            // Check elimination
            if (blackCount == 0)
            {
                EndGame(Player.Red);
                return true;
            }
            if (redCount == 0)
            {
                EndGame(Player.Black);
                return true;
            }

            // Check stalemate (current player has no moves)
            if (!moveValidator.HasLegalMoves(Board, CurrentPlayer))
            {
                // Player with no moves loses
                Player winner = CurrentPlayer == Player.Black ? Player.Red : Player.Black;
                EndGame(winner);
                return true;
            }

            return false;
        }

        private void EndGame(Player winner)
        {
            State = GameState.GameOver;
            Winner = winner;
            Debug.Log($"Game Over! {winner} wins!");
            OnGameOver?.Invoke(winner);
        }

        public bool IsHumanTurn()
        {
            // For now, assume Black is human
            return CurrentPlayer == Player.Black && State == GameState.WaitingForMove;
        }

        public int TurnsUntilShrink()
        {
            return TurnsPerShrink - (TurnCount % TurnsPerShrink);
        }
    }
}
