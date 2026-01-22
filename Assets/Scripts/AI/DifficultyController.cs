using System.Collections.Generic;
using UnityEngine;

namespace EntropyCheckers.AI
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Master
    }

    public class DifficultyController
    {
        private Difficulty currentDifficulty;
        private System.Random random;

        // Search depth by difficulty
        private static readonly Dictionary<Difficulty, int> SearchDepths = new Dictionary<Difficulty, int>
        {
            { Difficulty.Easy, 2 },
            { Difficulty.Medium, 3 },
            { Difficulty.Hard, 4 },
            { Difficulty.Master, 5 }
        };

        // Probability of choosing optimal move
        private static readonly Dictionary<Difficulty, float> OptimalMoveProbability = new Dictionary<Difficulty, float>
        {
            { Difficulty.Easy, 0.3f },      // 30% chance of best move
            { Difficulty.Medium, 0.6f },    // 60% chance of best move
            { Difficulty.Hard, 0.85f },     // 85% chance of best move
            { Difficulty.Master, 1.0f }     // Always best move
        };

        public DifficultyController(Difficulty difficulty = Difficulty.Medium)
        {
            currentDifficulty = difficulty;
            random = new System.Random();
        }

        public Difficulty CurrentDifficulty
        {
            get => currentDifficulty;
            set => currentDifficulty = value;
        }

        public int GetSearchDepth()
        {
            return SearchDepths[currentDifficulty];
        }

        /// <summary>
        /// Selects a move from the ranked list based on difficulty.
        /// Lower difficulties have a chance to pick suboptimal moves.
        /// </summary>
        public ScoredMove SelectMove(List<ScoredMove> rankedMoves)
        {
            if (rankedMoves == null || rankedMoves.Count == 0)
            {
                return null;
            }

            if (rankedMoves.Count == 1)
            {
                return rankedMoves[0];
            }

            float optimalChance = OptimalMoveProbability[currentDifficulty];

            // Roll for optimal move
            if (random.NextDouble() < optimalChance)
            {
                return rankedMoves[0]; // Best move
            }

            // Otherwise, pick a suboptimal move based on difficulty
            return SelectSuboptimalMove(rankedMoves);
        }

        private ScoredMove SelectSuboptimalMove(List<ScoredMove> rankedMoves)
        {
            switch (currentDifficulty)
            {
                case Difficulty.Easy:
                    // Easy: Favor bad moves (weighted toward bottom of list)
                    return SelectWeightedMove(rankedMoves, favorWorst: true);

                case Difficulty.Medium:
                    // Medium: Random from top half
                    int mediumRange = Mathf.Max(1, rankedMoves.Count / 2);
                    return rankedMoves[random.Next(mediumRange)];

                case Difficulty.Hard:
                    // Hard: Random from top 3
                    int hardRange = Mathf.Min(3, rankedMoves.Count);
                    return rankedMoves[random.Next(hardRange)];

                default:
                    return rankedMoves[0];
            }
        }

        private ScoredMove SelectWeightedMove(List<ScoredMove> moves, bool favorWorst)
        {
            // Create weights that favor either end of the list
            int count = moves.Count;
            float[] weights = new float[count];
            float totalWeight = 0;

            for (int i = 0; i < count; i++)
            {
                if (favorWorst)
                {
                    // Higher weight for worse moves (later in sorted list)
                    weights[i] = i + 1;
                }
                else
                {
                    // Higher weight for better moves (earlier in sorted list)
                    weights[i] = count - i;
                }
                totalWeight += weights[i];
            }

            // Weighted random selection
            float roll = (float)(random.NextDouble() * totalWeight);
            float cumulative = 0;

            for (int i = 0; i < count; i++)
            {
                cumulative += weights[i];
                if (roll < cumulative)
                {
                    return moves[i];
                }
            }

            return moves[count - 1];
        }

        /// <summary>
        /// Gets a human-readable description of the current difficulty.
        /// </summary>
        public string GetDifficultyDescription()
        {
            switch (currentDifficulty)
            {
                case Difficulty.Easy:
                    return "Easy - AI often makes mistakes";
                case Difficulty.Medium:
                    return "Medium - AI plays reasonably well";
                case Difficulty.Hard:
                    return "Hard - AI plays strategically";
                case Difficulty.Master:
                    return "Master - AI plays optimally";
                default:
                    return "Unknown";
            }
        }
    }
}
