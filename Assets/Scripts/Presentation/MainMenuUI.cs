using UnityEngine;
using UnityEngine.InputSystem;
using EntropyCheckers.AI;

namespace EntropyCheckers.Presentation
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Menu Settings")]
        [SerializeField] private bool showMenuOnStart = true;
        
        private bool isMenuVisible;
        private bool initialized;
        private GameBootstrap gameBootstrap;
        private Difficulty selectedDifficulty = Difficulty.Medium;

        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle selectedButtonStyle;
        private GUIStyle descriptionStyle;
        private bool stylesInitialized;

        private void Start()
        {
            gameBootstrap = FindObjectOfType<GameBootstrap>();
            initialized = true;
            
            if (showMenuOnStart)
            {
                ShowMenu();
            }
        }

        private void Update()
        {
            // Press Escape to toggle menu
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (isMenuVisible)
                {
                    HideMenu();
                }
                else
                {
                    ShowMenu();
                }
            }
        }

        public void ShowMenu()
        {
            isMenuVisible = true;
            Time.timeScale = 0; // Pause game
        }

        public void HideMenu()
        {
            isMenuVisible = false;
            Time.timeScale = 1; // Resume game
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 48,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.9f, 0.3f, 0.3f) }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            selectedButtonStyle = new GUIStyle(buttonStyle);
            selectedButtonStyle.normal.textColor = Color.yellow;
            selectedButtonStyle.hover.textColor = Color.yellow;

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!isMenuVisible || !initialized) return;

            InitializeStyles();

            // Full screen overlay
            GUI.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float panelWidth = 400;
            float panelHeight = Mathf.Min(480, Screen.height - 20);
            float panelX = (Screen.width - panelWidth) / 2;
            float panelY = (Screen.height - panelHeight) / 2;

            GUILayout.BeginArea(new Rect(panelX, panelY, panelWidth, panelHeight));
            GUILayout.BeginVertical();

            // Title
            GUILayout.Space(10);
            GUILayout.Label("ENTROPY CHECKERS", titleStyle);
            GUILayout.Label("The board is not your friend.", subtitleStyle);
            GUILayout.Space(20);

            // Difficulty selection
            GUILayout.Label("SELECT DIFFICULTY", new GUIStyle(subtitleStyle) { fontStyle = FontStyle.Bold });
            GUILayout.Space(10);

            DrawDifficultyButton(Difficulty.Easy, "EASY");
            DrawDifficultyButton(Difficulty.Medium, "MEDIUM");
            DrawDifficultyButton(Difficulty.Hard, "HARD");
            DrawDifficultyButton(Difficulty.Master, "MASTER");

            GUILayout.Space(15);

            // Start/Resume button
            GUI.backgroundColor = new Color(0.3f, 0.7f, 0.3f);
            string startButtonText = gameBootstrap?.GameManager?.State == Game.GameState.NotStarted 
                ? "START GAME" 
                : "RESUME GAME";
            
            if (GUILayout.Button(startButtonText, buttonStyle, GUILayout.Height(45)))
            {
                if (gameBootstrap != null)
                {
                    gameBootstrap.SetDifficulty(selectedDifficulty);
                    
                    if (gameBootstrap.GameManager?.State == Game.GameState.NotStarted ||
                        gameBootstrap.GameManager?.State == Game.GameState.GameOver)
                    {
                        gameBootstrap.ResetGame();
                    }
                }
                HideMenu();
            }

            GUILayout.Space(10);

            // New Game button (if game in progress)
            if (gameBootstrap?.GameManager?.State != Game.GameState.NotStarted)
            {
                GUI.backgroundColor = new Color(0.7f, 0.5f, 0.2f);
                if (GUILayout.Button("NEW GAME", buttonStyle, GUILayout.Height(40)))
                {
                    if (gameBootstrap != null)
                    {
                        gameBootstrap.SetDifficulty(selectedDifficulty);
                        gameBootstrap.ResetGame();
                    }
                    HideMenu();
                }
            }

            GUI.backgroundColor = Color.white;

            GUILayout.FlexibleSpace();

            // Controls info
            GUILayout.Label("Left Click: Select/Move | Right Click: Deselect | Escape: Menu", descriptionStyle);

            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawDifficultyButton(Difficulty difficulty, string label)
        {
            bool isSelected = selectedDifficulty == difficulty;
            var style = isSelected ? selectedButtonStyle : buttonStyle;
            
            GUI.backgroundColor = isSelected 
                ? new Color(0.4f, 0.4f, 0.6f) 
                : new Color(0.3f, 0.3f, 0.4f);

            if (GUILayout.Button(label, style, GUILayout.Height(35)))
            {
                selectedDifficulty = difficulty;
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
