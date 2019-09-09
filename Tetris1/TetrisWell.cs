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
        // well   10 width, 21 height
        SolidBrush[] WellColor = new SolidBrush[210]; // Color for each cell
        Rectangle[] WellRect;                         // Rectangle for each cell
        Pen GridColor = new Pen(Color.DarkBlue);

        public void ClearRows()
        {
            // Clear full rows


            // Give score for the cleared rows
        }

        public void CheckRows()
        {
            // Check for full rows
            List<int> rows = new List<int>();
            // start at top row = y * 10 * 20
            for (int y = 200; y >= 0; y -= 10)
            {
                // For each column
                for (int x = 0; x < 10; x++)
                {
                    // If that cell is empty break
                    if (WellColor[x + y].Equals(BlackBrush)) break;
                    if (x == 9) // The entire row is full
                    {
                        rows.Add(y);
                    }
                }
            }
            UpdateScore(ScoreVal(rows.Count())); // Add to score
            foreach (int row in rows)
            {
                // Start in highest full row and check each cell above
                for (int y = row; y < 210; y += 10)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        // If the cell is at the very top clear it
                        if (x + y + 10 >= 210)
                            WellColor[x + y] = BlackBrush;
                        // Else give it the coler of the cell above
                        else
                            WellColor[x + y] = WellColor[x + y + 10];
                    }
                }
            }
            // change color of full rows
        }

        private void AttachPiece()
        {
            int[] indecies = cPiece.GetIndecies();
            for (int i = 0; i < 4; i++)
            {
                if (indecies[i] < 210)
                    WellColor[indecies[i]] = cPiece.GetColor();
                else if (indecies[i] < 300)
                    GameOver();
            }
            // FIX
            NextPiece();
        }

        // Check for collision in rotation described by rot
        public bool RotCollision(int rot)
        {
            return false;
        }

        // Check for collision in direction described by dir
        public bool MoveCollision(Point dir)
        {
            int[] indecies = cPiece.PreviewMove(dir);
            for (int i = 0; i < 4; i++)
            {
                if (indecies[i] == 500 || indecies[i] < 0) return true;
                if (indecies[i] < 210)
                    if (!WellColor[indecies[i]].Equals(BlackBrush))
                        return true;
            }
            return false;
        }


        public void RenderColor(Graphics g)
        {
            // Draw Well backround
            for (int i = 0; i < 210; i++)
            {
                g.FillRectangle(WellColor[i], WellRect[i]);
            }
        }

        public void RenderGrid(Graphics g)
        {
            // Draw Well grid
            g.DrawRectangles(GridColor, WellRect);
        }

        public void RenderPiece(Graphics g)
        {
            int[] pieceD = cPiece.GetIndecies();
            SolidBrush pieceB = cPiece.GetColor();
            for (int i = 0; i < 4; i++)
            {
                if (pieceD[i] < 210)
                    g.FillRectangle(pieceB, WellRect[pieceD[i]]);
            }
        }


}
}
