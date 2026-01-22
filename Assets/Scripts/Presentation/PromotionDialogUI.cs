using System;
using System.Collections.Generic;
using UnityEngine;
using EntropyCheckers.Core;
using EntropyCheckers.Game;

namespace EntropyCheckers.Presentation
{
    public class PromotionDialogUI : MonoBehaviour
    {
        [Header("Dialog Settings")]
        [SerializeField] private Vector2 dialogSize = new Vector2(400, 420);
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        [SerializeField] private Color buttonColor = new Color(0.3f, 0.3f, 0.4f);
        [SerializeField] private Color buttonHoverColor = new Color(0.4f, 0.4f, 0.5f);
        [SerializeField] private Color wraithColor = new Color(0.5f, 0.8f, 1f);
        [SerializeField] private Color corruptedColor = new Color(0.9f, 0.7f, 0.2f);

        private GameManager gameManager;
        private PieceRenderer pieceRenderer;
        private bool isShowing;
        private Piece promotingPiece;
        private Piece selectedDefector;
        private List<Piece> defectorCandidates;
        private bool selectingDefector;

        private GUIStyle titleStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle buttonStyle;
        private GUIStyle selectedButtonStyle;
        private bool stylesInitialized;

        public void Initialize(GameManager gameManager, PieceRenderer pieceRenderer)
        {
            this.gameManager = gameManager;
            this.pieceRenderer = pieceRenderer;

            gameManager.OnPromotionRequired += ShowPromotionDialog;
            gameManager.OnPromotionComplete += HidePromotionDialog;
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnPromotionRequired -= ShowPromotionDialog;
                gameManager.OnPromotionComplete -= HidePromotionDialog;
            }
        }

        private void ShowPromotionDialog(Piece piece)
        {
            promotingPiece = piece;
            isShowing = true;
            selectedDefector = null;
            selectingDefector = false;
            defectorCandidates = gameManager.GetValidDefectorCandidates();
        }

        private void HidePromotionDialog(Piece piece, PromotionChoice choice)
        {
            isShowing = false;
            promotingPiece = null;
            selectedDefector = null;
            selectingDefector = false;
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                hover = { textColor = Color.white },
                active = { textColor = Color.white }
            };

            selectedButtonStyle = new GUIStyle(buttonStyle);
            selectedButtonStyle.normal.textColor = Color.yellow;

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!isShowing) return;

            InitializeStyles();

            // Draw semi-transparent overlay
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Center the dialog
            float dialogX = (Screen.width - dialogSize.x) / 2;
            float dialogY = (Screen.height - dialogSize.y) / 2;
            Rect dialogRect = new Rect(dialogX, dialogY, dialogSize.x, dialogSize.y);

            // Draw dialog background
            GUI.color = backgroundColor;
            GUI.DrawTexture(dialogRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUILayout.BeginArea(dialogRect);
            GUILayout.BeginVertical();

            GUILayout.Space(20);

            if (selectingDefector)
            {
                DrawDefectorSelection();
            }
            else
            {
                DrawPromotionChoice();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawPromotionChoice()
        {
            GUILayout.Label("PROMOTION", titleStyle);
            GUILayout.Space(5);
            GUILayout.Label("Your piece has reached the King's Row.\nChoose your corruption:", descriptionStyle);
            GUILayout.Space(15);

            // Wraith King button
            GUI.backgroundColor = wraithColor;
            if (GUILayout.Button("WRAITH KING", buttonStyle, GUILayout.Height(45)))
            {
                gameManager.SubmitPromotion(PromotionChoice.WraithKing);
            }
            GUILayout.Label("Unlimited diagonal movement\nDies after 3 turns", descriptionStyle);

            GUILayout.Space(15);

            // Corrupted King button
            GUI.backgroundColor = corruptedColor;
            if (GUILayout.Button("CORRUPTED KING", buttonStyle, GUILayout.Height(45)))
            {
                if (defectorCandidates.Count > 0)
                {
                    selectingDefector = true;
                }
                else
                {
                    // No pieces to defect, can't choose corrupted
                    Debug.Log("No pieces available to defect!");
                }
            }
            
            string corruptedDesc = defectorCandidates.Count > 0 
                ? "Standard king movement\nOne of your pieces defects to enemy" 
                : "Standard king movement\n(Unavailable: no pieces to defect)";
            GUILayout.Label(corruptedDesc, descriptionStyle);

            GUI.backgroundColor = Color.white;
        }

        private void DrawDefectorSelection()
        {
            GUILayout.Label("SELECT DEFECTOR", titleStyle);
            GUILayout.Space(10);
            GUILayout.Label("Choose which piece will defect to the enemy:", descriptionStyle);
            GUILayout.Space(20);

            // Scroll view for defector candidates
            GUILayout.BeginVertical();
            
            foreach (var piece in defectorCandidates)
            {
                string pieceLabel = $"{piece.Type} at ({piece.Position.x}, {piece.Position.y})";
                
                // Highlight pieces on hazard tiles as "good" defection choices
                var tile = gameManager.Board.GetTile(piece.Position);
                if (tile != null && tile.State == TileState.Hazard)
                {
                    pieceLabel += " [ON HAZARD - RECOMMENDED]";
                    GUI.backgroundColor = new Color(0.8f, 0.4f, 0.2f);
                }
                else
                {
                    GUI.backgroundColor = buttonColor;
                }

                if (GUILayout.Button(pieceLabel, buttonStyle, GUILayout.Height(35)))
                {
                    selectedDefector = piece;
                    gameManager.SubmitPromotion(PromotionChoice.CorruptedKing, selectedDefector);
                }
            }

            GUILayout.Space(20);

            // Cancel button
            GUI.backgroundColor = new Color(0.5f, 0.3f, 0.3f);
            if (GUILayout.Button("Cancel", buttonStyle, GUILayout.Height(35)))
            {
                selectingDefector = false;
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();
        }
    }
}
