using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;
using EntropyCheckers.AI;

namespace EntropyCheckers.Presentation
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private PieceRenderer pieceRenderer;
        
        [Header("Input & UI")]
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private GameUI gameUI;
        [SerializeField] private PromotionDialogUI promotionDialog;
        
        [Header("AI")]
        [SerializeField] private AIPlayer aiPlayer;
        [SerializeField] private Difficulty aiDifficulty = Difficulty.Medium;
        
        [Header("Menus")]
        [SerializeField] private MainMenuUI mainMenu;
        
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float cameraZoom = 6f;

        private GameManager gameManager;

        public GameManager GameManager => gameManager;
        public AIPlayer AIPlayer => aiPlayer;

        private void Awake()
        {
            if (boardRenderer == null)
            {
                var boardObj = new GameObject("BoardRenderer");
                boardRenderer = boardObj.AddComponent<BoardRenderer>();
            }
            
            if (pieceRenderer == null)
            {
                var pieceObj = new GameObject("PieceRenderer");
                pieceRenderer = pieceObj.AddComponent<PieceRenderer>();
            }
            
            if (inputHandler == null)
            {
                var inputObj = new GameObject("InputHandler");
                inputHandler = inputObj.AddComponent<InputHandler>();
            }
            
            if (gameUI == null)
            {
                var uiObj = new GameObject("GameUI");
                gameUI = uiObj.AddComponent<GameUI>();
            }
            
            if (promotionDialog == null)
            {
                var dialogObj = new GameObject("PromotionDialog");
                promotionDialog = dialogObj.AddComponent<PromotionDialogUI>();
            }
            
            if (aiPlayer == null)
            {
                var aiObj = new GameObject("AIPlayer");
                aiPlayer = aiObj.AddComponent<AIPlayer>();
            }
            
            if (mainMenu == null)
            {
                var menuObj = new GameObject("MainMenu");
                mainMenu = menuObj.AddComponent<MainMenuUI>();
            }
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameManager = new GameManager();
            
            boardRenderer.Initialize(gameManager.Board);
            
            SetupCamera();
            
            // Start game BEFORE initializing piece renderer so pieces exist
            gameManager.StartGame();
            
            pieceRenderer.Initialize(gameManager.Board, boardRenderer);
            inputHandler.Initialize(gameManager, boardRenderer, pieceRenderer);
            gameUI.Initialize(gameManager);
            promotionDialog.Initialize(gameManager, pieceRenderer);
            aiPlayer.Initialize(gameManager, pieceRenderer);
            aiPlayer.CurrentDifficulty = aiDifficulty;
            
            Debug.Log("Entropy Checkers initialized!");
            Debug.Log($"Black pieces: {gameManager.Board.CountPieces(Player.Black)}");
            Debug.Log($"Red pieces: {gameManager.Board.CountPieces(Player.Red)}");
            Debug.Log($"AI Difficulty: {aiDifficulty}");
            Debug.Log("Black (human) moves first. Click a piece to select, then click a destination.");
        }

        private void SetupCamera()
        {
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = cameraZoom;
                mainCamera.transform.position = new Vector3(0, 0, -10);
                mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
            }
        }

        public void SetDifficulty(Difficulty difficulty)
        {
            aiDifficulty = difficulty;
            if (aiPlayer != null)
            {
                aiPlayer.CurrentDifficulty = difficulty;
            }
        }

        [ContextMenu("Test Board Shrink")]
        public void TestBoardShrink()
        {
            if (gameManager != null)
            {
                gameManager.Board.ShrinkBoard();
                Debug.Log($"Board shrunk to ring level: {gameManager.Board.CurrentShrinkRing}");
            }
        }

        [ContextMenu("Reset Game")]
        public void ResetGame()
        {
            if (gameManager != null)
            {
                gameManager.StartGame();
                pieceRenderer.RefreshAllPieces();
                Debug.Log("Game reset!");
            }
        }

        [ContextMenu("Show Legal Moves")]
        public void ShowLegalMoves()
        {
            if (gameManager != null)
            {
                var moves = gameManager.GetLegalMoves();
                Debug.Log($"Legal moves for {gameManager.CurrentPlayer}: {moves.Count}");
                foreach (var move in moves)
                {
                    Debug.Log($"  {move}");
                }
            }
        }

        [ContextMenu("Set Easy Difficulty")]
        public void SetEasyDifficulty() => SetDifficulty(Difficulty.Easy);

        [ContextMenu("Set Medium Difficulty")]
        public void SetMediumDifficulty() => SetDifficulty(Difficulty.Medium);

        [ContextMenu("Set Hard Difficulty")]
        public void SetHardDifficulty() => SetDifficulty(Difficulty.Hard);

        [ContextMenu("Set Master Difficulty")]
        public void SetMasterDifficulty() => SetDifficulty(Difficulty.Master);
    }
}
