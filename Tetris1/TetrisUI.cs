using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Tetris1
{
    public partial class TetrisGame
    {
        // score
        int score;
        PointF scoreLoc;

        Font GameFont = new System.Drawing.Font("Arial", 16);

        // Game Over and Paused screen
        Font BigFont = new System.Drawing.Font("Arial", 26);
        PointF BigTextLoc;

        Rectangle BackgroundRect;

        // Next piece preview
        Rectangle[] NextRect;
        // Hold piece preview
        Rectangle[] HoldRect;
        const int PreviewCellSize = 30;

        public void UpdateScore(int add)
        {
            score += add;
        }


        public void RenderScore(Graphics g)
        {
            // HUD
            g.DrawString("Score: " + score, GameFont, BlackBrush, scoreLoc);
        }

        public void RenderPause(Graphics g)
        {
            if (cGameState == GameState.Pause)
            {
                g.FillRectangle(new SolidBrush(Color.Gray), BackgroundRect);
                g.DrawString("Paused", BigFont, BlackBrush, PointF.Add(BigTextLoc, new Size(30, 20)));
            }
        }

        public void RenderGameover(Graphics g)
        {
            if (cGameState == GameState.GameOver)
            {
                g.FillRectangle(new SolidBrush(Color.Gray), BackgroundRect);
                g.DrawString("Game Over", BigFont, BlackBrush, BigTextLoc);

                g.DrawString("Your score is: " + score, GameFont, BlackBrush, PointF.Add(BigTextLoc, new Size(-20, 60)));
            }
        }

        public void RenderHoldPiece(Graphics g)
        {
            for (int i = 0; i < 16; i++)
            {
                g.FillRectangle(BlackBrush, HoldRect[i]);
            }
            if (!hPiece.GetTetraType().Equals(Tetramino.TetraType.Empty))
            {
                int[] pieceD = hPiece.GetIndecies();
                SolidBrush pieceB = hPiece.GetColor();
                for (int i = 0; i < 4; i++)
                {
                    if (pieceD[i] < 16)
                        g.FillRectangle(pieceB, HoldRect[pieceD[i]]);
                }
            }
            g.DrawRectangles(new Pen(Color.DarkBlue), HoldRect);
        }

        public void RenderNextPiece(Graphics g)
        {
            for (int i = 0; i < 16; i++)
            {
                g.FillRectangle(BlackBrush, NextRect[i]);
            }
            int[] pieceD = nPiece.GetIndecies();
            SolidBrush pieceB = nPiece.GetColor();
            for (int i = 0; i < 4; i++)
            {
                if (pieceD[i] < 16)
                    g.FillRectangle(pieceB, NextRect[pieceD[i]]);
            }
            g.DrawRectangles(new Pen(Color.DarkBlue), NextRect);
        }

    }
}
