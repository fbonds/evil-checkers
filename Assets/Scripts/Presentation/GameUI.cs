using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;

namespace EntropyCheckers.Presentation
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private int fontSize = 18;
        [SerializeField] private Color blackPlayerColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color redPlayerColor = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] private Color warningColor = new Color(1f, 0.6f, 0.2f);
        [SerializeField] private Color dangerColor = new Color(1f, 0.2f, 0.2f);

        private GameManager gameManager;
        private GUIStyle labelStyle;
        private GUIStyle turnStyle;
        private GUIStyle warningStyle;
        private GUIStyle gameOverStyle;
        private GUIStyle buttonStyle;
        private GUIStyle forcedCaptureStyle;
        private bool stylesInitialized;
        
        // Forced capture notification
        private bool showForcedCapture;
        private float forcedCaptureTimer;
        private const float ForcedCaptureDisplayTime = 1.5f;

        public void Initialize(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                normal = { textColor = Color.white }
            };

            turnStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize + 4,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            warningStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = warningColor }
            };

            gameOverStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold
            };

            forcedCaptureStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.8f, 0.2f) }
            };

            stylesInitialized = true;
        }

        private void Update()
        {
            if (showForcedCapture)
            {
                forcedCaptureTimer -= Time.deltaTime;
                if (forcedCaptureTimer <= 0)
                {
                    showForcedCapture = false;
                }
            }
        }

        private void OnGUI()
        {
            if (gameManager == null) return;

            InitializeStyles();

            DrawTopPanel();
            DrawBottomPanel();

            if (showForcedCapture)
            {
                DrawForcedCaptureNotification();
            }

            if (gameManager.State == GameState.GameOver)
            {
                DrawGameOverScreen();
            }
        }
        
        private void DrawForcedCaptureNotification()
        {
            float alpha = Mathf.PingPong(Time.time * 3f, 1f) * 0.5f + 0.5f;
            
            GUI.color = new Color(0, 0, 0, 0.6f * alpha);
            float boxWidth = 350;
            float boxHeight = 60;
            GUI.DrawTexture(new Rect((Screen.width - boxWidth) / 2, Screen.height / 2 - 80, boxWidth, boxHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;
            
            forcedCaptureStyle.normal.textColor = new Color(1f, 0.8f, 0.2f, alpha);
            GUI.Label(new Rect(0, Screen.height / 2 - 80, Screen.width, 60), "FORCED CAPTURE!", forcedCaptureStyle);
        }
        
        public void ShowForcedCaptureNotification()
        {
            showForcedCapture = true;
            forcedCaptureTimer = ForcedCaptureDisplayTime;
        }

        private void DrawTopPanel()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.BeginVertical();

            // Current turn
            turnStyle.normal.textColor = gameManager.CurrentPlayer == Player.Black 
                ? blackPlayerColor 
                : redPlayerColor;
            
            string turnText = gameManager.State == GameState.WaitingForPromotion 
                ? $"{gameManager.CurrentPlayer}'s Turn (PROMOTING)" 
                : $"{gameManager.CurrentPlayer}'s Turn";
            GUILayout.Label(turnText, turnStyle);

            // Turn counter
            GUILayout.Label($"Turn: {gameManager.TurnCount}", labelStyle);

            // Piece counts
            int blackCount = gameManager.Board.CountPieces(Player.Black);
            int redCount = gameManager.Board.CountPieces(Player.Red);
            GUILayout.Label($"Black: {blackCount}  |  Red: {redCount}", labelStyle);

            // Board shrink warning
            int turnsUntilShrink = gameManager.TurnsUntilShrink();
            if (turnsUntilShrink <= 2)
            {
                warningStyle.normal.textColor = turnsUntilShrink == 1 ? dangerColor : warningColor;
                GUILayout.Label($"BOARD SHRINKS IN {turnsUntilShrink} TURN(S)!", warningStyle);
            }
            else
            {
                GUILayout.Label($"Shrink in: {turnsUntilShrink} turns", labelStyle);
            }

            // Current shrink ring
            if (gameManager.Board.CurrentShrinkRing >= 0)
            {
                GUILayout.Label($"Ring {gameManager.Board.CurrentShrinkRing} is hazardous", warningStyle);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawBottomPanel()
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 80, 400, 70));
            GUILayout.BeginVertical();

            // Instructions based on game state
            string instructions = "";
            switch (gameManager.State)
            {
                case GameState.WaitingForMove:
                    if (gameManager.IsHumanTurn())
                    {
                        instructions = "Click a piece to select, then click a destination.\nRight-click to deselect.";
                    }
                    else
                    {
                        instructions = "AI is thinking...";
                    }
                    break;
                case GameState.WaitingForPromotion:
                    instructions = "Choose your promotion type.";
                    break;
                case GameState.GameOver:
                    instructions = "";
                    break;
            }

            GUILayout.Label(instructions, labelStyle);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawGameOverScreen()
        {
            // Semi-transparent overlay
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float panelWidth = 400;
            float panelHeight = 200;
            float panelX = (Screen.width - panelWidth) / 2;
            float panelY = (Screen.height - panelHeight) / 2;

            GUILayout.BeginArea(new Rect(panelX, panelY, panelWidth, panelHeight));
            GUILayout.BeginVertical();

            GUILayout.Space(20);

            // Winner announcement
            gameOverStyle.normal.textColor = gameManager.Winner == Player.Black 
                ? blackPlayerColor 
                : redPlayerColor;
            GUILayout.Label($"{gameManager.Winner} WINS!", gameOverStyle);

            GUILayout.Space(20);

            // Final stats
            labelStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label($"Game ended on turn {gameManager.TurnCount}", labelStyle);
            
            int blackCount = gameManager.Board.CountPieces(Player.Black);
            int redCount = gameManager.Board.CountPieces(Player.Red);
            GUILayout.Label($"Final pieces - Black: {blackCount}, Red: {redCount}", labelStyle);
            labelStyle.alignment = TextAnchor.MiddleLeft;

            GUILayout.Space(20);

            // Play Again button
            if (GUILayout.Button("Play Again", buttonStyle, GUILayout.Height(40)))
            {
                var bootstrap = FindFirstObjectByType<GameBootstrap>();
                if (bootstrap != null)
                {
                    bootstrap.ResetGame();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
