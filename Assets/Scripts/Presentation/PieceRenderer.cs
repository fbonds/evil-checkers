using System.Collections.Generic;
using UnityEngine;
using EntropyCheckers.Core;

namespace EntropyCheckers.Presentation
{
    public class PieceRenderer : MonoBehaviour
    {
        [Header("Piece Colors")]
        [SerializeField] private Color blackPieceColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color redPieceColor = new Color(0.7f, 0.15f, 0.15f);
        [SerializeField] private Color blackKingColor = new Color(0.25f, 0.25f, 0.35f);
        [SerializeField] private Color redKingColor = new Color(0.85f, 0.25f, 0.25f);
        [SerializeField] private Color wraithTint = new Color(0.5f, 0.8f, 1f, 0.7f);
        
        [Header("Piece Settings")]
        [SerializeField] private float pieceScale = 0.8f;
        [SerializeField] private float kingCrownScale = 0.4f;

        private Dictionary<Piece, GameObject> pieceObjects = new Dictionary<Piece, GameObject>();
        private Board board;
        private BoardRenderer boardRenderer;

        public void Initialize(Board board, BoardRenderer boardRenderer)
        {
            this.board = board;
            this.boardRenderer = boardRenderer;
            
            board.OnPieceDestroyed += HandlePieceDestroyed;
            board.OnPieceDefected += HandlePieceDefected;
            
            CreatePieceObjects();
        }

        private void OnDestroy()
        {
            if (board != null)
            {
                board.OnPieceDestroyed -= HandlePieceDestroyed;
                board.OnPieceDefected -= HandlePieceDefected;
            }
        }

        private void CreatePieceObjects()
        {
            var allPieces = board.GetAllAlivePieces();
            
            foreach (var piece in allPieces)
            {
                CreatePieceObject(piece);
            }
        }

        private void CreatePieceObject(Piece piece)
        {
            Vector3 worldPos = boardRenderer.BoardToWorld(piece.Position);
            worldPos.z = -0.5f;
            
            GameObject pieceObj = new GameObject($"Piece_{piece.Owner}_{piece.Position.x}_{piece.Position.y}");
            pieceObj.transform.SetParent(transform);
            pieceObj.transform.position = worldPos;
            pieceObj.transform.localScale = Vector3.one * boardRenderer.TileSize * pieceScale;
            
            var renderer = pieceObj.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite();
            renderer.color = GetPieceColor(piece);
            renderer.sortingOrder = 10;
            
            pieceObjects[piece] = pieceObj;
        }

        private Sprite CreateCircleSprite()
        {
            int resolution = 64;
            Texture2D texture = new Texture2D(resolution, resolution);
            texture.filterMode = FilterMode.Bilinear;
            
            float center = resolution / 2f;
            float radius = resolution / 2f - 1;
            
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    
                    if (distance < radius - 1)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else if (distance < radius)
                    {
                        float alpha = 1 - (distance - (radius - 1));
                        texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            texture.Apply();
            
            return Sprite.Create(
                texture,
                new Rect(0, 0, resolution, resolution),
                new Vector2(0.5f, 0.5f),
                resolution
            );
        }

        private Color GetPieceColor(Piece piece)
        {
            Color baseColor;
            
            if (piece.IsKing)
            {
                baseColor = piece.Owner == Player.Black ? blackKingColor : redKingColor;
            }
            else
            {
                baseColor = piece.Owner == Player.Black ? blackPieceColor : redPieceColor;
            }
            
            if (piece.Type == PieceType.WraithKing)
            {
                baseColor = Color.Lerp(baseColor, wraithTint, 0.5f);
                baseColor.a = 0.8f;
            }
            
            return baseColor;
        }

        private void HandlePieceDestroyed(Piece piece)
        {
            if (pieceObjects.TryGetValue(piece, out GameObject pieceObj))
            {
                pieceObjects.Remove(piece);
                Destroy(pieceObj);
            }
        }

        private void HandlePieceDefected(Piece piece)
        {
            UpdatePieceVisual(piece);
        }

        public void UpdatePieceVisual(Piece piece)
        {
            if (!pieceObjects.TryGetValue(piece, out GameObject pieceObj)) return;
            
            var renderer = pieceObj.GetComponent<SpriteRenderer>();
            renderer.color = GetPieceColor(piece);
            
            Transform crownTransform = pieceObj.transform.Find("Crown");
            
            if (piece.IsKing && crownTransform == null)
            {
                CreateCrown(pieceObj, piece);
            }
            else if (!piece.IsKing && crownTransform != null)
            {
                Destroy(crownTransform.gameObject);
            }
        }

        private void CreateCrown(GameObject pieceObj, Piece piece)
        {
            GameObject crown = new GameObject("Crown");
            crown.transform.SetParent(pieceObj.transform);
            crown.transform.localPosition = new Vector3(0, 0, -0.1f);
            crown.transform.localScale = Vector3.one * kingCrownScale;
            
            var renderer = crown.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCrownSprite();
            renderer.color = piece.Type == PieceType.WraithKing ? 
                new Color(0.5f, 0.8f, 1f) : 
                new Color(1f, 0.85f, 0.2f);
            renderer.sortingOrder = 11;
        }

        private Sprite CreateCrownSprite()
        {
            int resolution = 32;
            Texture2D texture = new Texture2D(resolution, resolution);
            texture.filterMode = FilterMode.Bilinear;
            
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
            
            int baseY = 8;
            int topY = 24;
            int peakCount = 3;
            
            for (int x = 4; x < resolution - 4; x++)
            {
                for (int y = baseY; y < baseY + 4; y++)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
            
            for (int i = 0; i < peakCount; i++)
            {
                int peakX = 8 + i * 8;
                int peakHeight = topY;
                
                for (int y = baseY + 4; y < peakHeight; y++)
                {
                    int width = Mathf.Max(1, 4 - (y - baseY - 4) / 3);
                    for (int dx = -width; dx <= width; dx++)
                    {
                        int px = peakX + dx;
                        if (px >= 0 && px < resolution)
                        {
                            texture.SetPixel(px, y, Color.white);
                        }
                    }
                }
            }
            
            texture.Apply();
            
            return Sprite.Create(
                texture,
                new Rect(0, 0, resolution, resolution),
                new Vector2(0.5f, 0.5f),
                resolution
            );
        }

        public void MovePiece(Piece piece, Vector2Int newPosition, bool animate = true)
        {
            if (!pieceObjects.TryGetValue(piece, out GameObject pieceObj)) return;
            
            Vector3 targetPos = boardRenderer.BoardToWorld(newPosition);
            targetPos.z = -0.5f;
            
            if (animate)
            {
                StartCoroutine(AnimateMove(pieceObj, targetPos));
            }
            else
            {
                pieceObj.transform.position = targetPos;
            }
            
            pieceObj.name = $"Piece_{piece.Owner}_{newPosition.x}_{newPosition.y}";
        }

        private System.Collections.IEnumerator AnimateMove(GameObject pieceObj, Vector3 targetPos)
        {
            Vector3 startPos = pieceObj.transform.position;
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                pieceObj.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }
            
            pieceObj.transform.position = targetPos;
        }

        public void RefreshAllPieces()
        {
            foreach (var pieceObj in pieceObjects.Values)
            {
                Destroy(pieceObj);
            }
            pieceObjects.Clear();
            
            CreatePieceObjects();
        }

        public GameObject GetPieceObject(Piece piece)
        {
            pieceObjects.TryGetValue(piece, out GameObject obj);
            return obj;
        }
    }
}
