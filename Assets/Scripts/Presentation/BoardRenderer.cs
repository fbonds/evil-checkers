using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.Presentation
{
    public class BoardRenderer : MonoBehaviour
    {
        [Header("Tile Settings")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.05f;
        
        [Header("Tile Colors")]
        [SerializeField] private Color lightTileColor = new Color(0.93f, 0.93f, 0.82f);
        [SerializeField] private Color darkTileColor = new Color(0.45f, 0.32f, 0.22f);
        [SerializeField] private Color hazardTileColor = new Color(0.8f, 0.2f, 0.1f);
        [SerializeField] private Color removedTileColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        
        [Header("Highlight Colors")]
        [SerializeField] private Color selectedTileColor = new Color(0.2f, 0.6f, 0.2f, 0.7f);
        [SerializeField] private Color validMoveTileColor = new Color(0.2f, 0.5f, 0.8f, 0.7f);
        [SerializeField] private Color captureMoveTileColor = new Color(0.9f, 0.6f, 0.1f, 0.7f);

        private GameObject[,] tileObjects;
        private SpriteRenderer[,] tileRenderers;
        private GameObject[,] highlightObjects;
        private SpriteRenderer[,] highlightRenderers;
        private Board board;

        public float TileSize => tileSize;
        public float TotalTileSize => tileSize + tileSpacing;

        public void Initialize(Board board)
        {
            this.board = board;
            
            board.OnTileStateChanged += HandleTileStateChanged;
            board.OnBoardShrink += HandleBoardShrink;
            
            CreateTileObjects();
        }

        private void OnDestroy()
        {
            if (board != null)
            {
                board.OnTileStateChanged -= HandleTileStateChanged;
                board.OnBoardShrink -= HandleBoardShrink;
            }
        }

        private void CreateTileObjects()
        {
            tileObjects = new GameObject[Board.Size, Board.Size];
            tileRenderers = new SpriteRenderer[Board.Size, Board.Size];
            highlightObjects = new GameObject[Board.Size, Board.Size];
            highlightRenderers = new SpriteRenderer[Board.Size, Board.Size];

            float boardOffset = (Board.Size - 1) * TotalTileSize / 2f;

            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    Vector3 position = new Vector3(
                        x * TotalTileSize - boardOffset,
                        y * TotalTileSize - boardOffset,
                        0
                    );

                    GameObject tileObj;
                    if (tilePrefab != null)
                    {
                        tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    }
                    else
                    {
                        tileObj = CreateDefaultTile(position);
                    }
                    
                    tileObj.name = $"Tile_{x}_{y}";
                    tileObjects[x, y] = tileObj;
                    
                    var renderer = tileObj.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                    {
                        renderer = tileObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = CreateSquareSprite();
                    }
                    tileRenderers[x, y] = renderer;
                    
                    bool isPlayable = (x + y) % 2 == 1; // Playable squares are dark
                    renderer.color = isPlayable ? darkTileColor : lightTileColor;
                    renderer.sortingOrder = 0;
                    
                    GameObject highlightObj = new GameObject($"Highlight_{x}_{y}");
                    highlightObj.transform.SetParent(tileObj.transform);
                    highlightObj.transform.localPosition = new Vector3(0, 0, -0.1f);
                    
                    var highlightRenderer = highlightObj.AddComponent<SpriteRenderer>();
                    highlightRenderer.sprite = CreateSquareSprite();
                    highlightRenderer.color = Color.clear;
                    highlightRenderer.sortingOrder = 1;
                    
                    highlightObjects[x, y] = highlightObj;
                    highlightRenderers[x, y] = highlightRenderer;
                }
            }
        }

        private GameObject CreateDefaultTile(Vector3 position)
        {
            var tileObj = new GameObject();
            tileObj.transform.position = position;
            tileObj.transform.SetParent(transform);
            tileObj.transform.localScale = new Vector3(tileSize, tileSize, 1);
            return tileObj;
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            
            return Sprite.Create(
                texture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1
            );
        }

        private void HandleTileStateChanged(Tile tile)
        {
            UpdateTileVisual(tile.Position.x, tile.Position.y);
        }

        private void HandleBoardShrink(int ringLevel)
        {
            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    UpdateTileVisual(x, y);
                }
            }
        }

        private void UpdateTileVisual(int x, int y)
        {
            var tile = board.GetTile(x, y);
            var renderer = tileRenderers[x, y];
            
            bool isDark = (x + y) % 2 == 1;
            
            switch (tile.State)
            {
                case TileState.Normal:
                    renderer.color = isDark ? darkTileColor : lightTileColor;
                    break;
                case TileState.Hazard:
                    renderer.color = isDark ? hazardTileColor : Color.Lerp(lightTileColor, hazardTileColor, 0.5f);
                    break;
                case TileState.Removed:
                    renderer.color = removedTileColor;
                    break;
            }
        }

        public void HighlightTile(Vector2Int position, HighlightType type)
        {
            if (!IsValidPosition(position)) return;
            
            var renderer = highlightRenderers[position.x, position.y];
            
            switch (type)
            {
                case HighlightType.None:
                    renderer.color = Color.clear;
                    break;
                case HighlightType.Selected:
                    renderer.color = selectedTileColor;
                    break;
                case HighlightType.ValidMove:
                    renderer.color = validMoveTileColor;
                    break;
                case HighlightType.CaptureMove:
                    renderer.color = captureMoveTileColor;
                    break;
            }
        }

        public void ClearAllHighlights()
        {
            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    highlightRenderers[x, y].color = Color.clear;
                }
            }
        }

        public Vector2Int WorldToBoard(Vector3 worldPosition)
        {
            float boardOffset = (Board.Size - 1) * TotalTileSize / 2f;
            
            int x = Mathf.RoundToInt((worldPosition.x + boardOffset) / TotalTileSize);
            int y = Mathf.RoundToInt((worldPosition.y + boardOffset) / TotalTileSize);
            
            return new Vector2Int(x, y);
        }

        public Vector3 BoardToWorld(Vector2Int boardPosition)
        {
            float boardOffset = (Board.Size - 1) * TotalTileSize / 2f;
            
            return new Vector3(
                boardPosition.x * TotalTileSize - boardOffset,
                boardPosition.y * TotalTileSize - boardOffset,
                0
            );
        }

        private bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < Board.Size &&
                   position.y >= 0 && position.y < Board.Size;
        }

        public void RefreshAllTiles()
        {
            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    UpdateTileVisual(x, y);
                }
            }
            ClearAllHighlights();
        }
    }

    public enum HighlightType
    {
        None,
        Selected,
        ValidMove,
        CaptureMove
    }
}
