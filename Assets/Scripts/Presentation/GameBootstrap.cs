using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.Presentation
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private PieceRenderer pieceRenderer;
        
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float cameraZoom = 6f;

        private Board board;

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
            board = new Board();
            board.SetupInitialPieces();
            
            boardRenderer.Initialize(board);
            pieceRenderer.Initialize(board, boardRenderer);
            
            SetupCamera();
            
            Debug.Log("Entropy Checkers initialized!");
            Debug.Log($"Black pieces: {board.CountPieces(Player.Black)}");
            Debug.Log($"Red pieces: {board.CountPieces(Player.Red)}");
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
            if (board != null)
            {
                board.ShrinkBoard();
                Debug.Log($"Board shrunk to ring level: {board.CurrentShrinkRing}");
            }
        }

        [ContextMenu("Reset Game")]
        public void ResetGame()
        {
            if (board != null)
            {
                board.Reset();
                board.SetupInitialPieces();
                pieceRenderer.RefreshAllPieces();
                Debug.Log("Game reset!");
            }
        }
    }
}
