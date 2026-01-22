using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;

namespace EntropyCheckers.Presentation
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private PieceRenderer pieceRenderer;
        
        [Header("Input")]
        [SerializeField] private InputHandler inputHandler;
        
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float cameraZoom = 6f;

        private GameManager gameManager;

        public GameManager GameManager => gameManager;

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
            pieceRenderer.Initialize(gameManager.Board, boardRenderer);
            inputHandler.Initialize(gameManager, boardRenderer, pieceRenderer);
            
            SetupCamera();
            
            gameManager.StartGame();
            
            Debug.Log("Entropy Checkers initialized!");
            Debug.Log($"Black pieces: {gameManager.Board.CountPieces(Player.Black)}");
            Debug.Log($"Red pieces: {gameManager.Board.CountPieces(Player.Red)}");
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
    }
}
