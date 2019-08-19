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
    class TetrisGame
    {
        public enum GameState
        {
            Running, Pause, Stop, GameOver
        }

        private GameState cGameState;
        // Start tetrominoes in row 22
        Point StartPos = new Point(4, 22);
        //Panel that will be drawn on
        public Panel Canvas;
        // current piece
        Tetramino cPiece;
        // next piece
        Tetramino nPiece;
        // hold piece
        Tetramino hPiece;
        // well   10 width, 21 height
        SolidBrush[] WellColor = new SolidBrush[210]; // Color for each cell
        Rectangle[] WellRect;                         // Rectangle for each cell
        // Next piece preview
        Rectangle[] NextRect;
        // Hold piece preview
        Rectangle[] HoldRect;
        const int PreviewCellSize = 30;
        // Stopwatch for drop
        Stopwatch DropTimer;

        // score
        int score;
        PointF scoreLoc;

        SolidBrush BlackBrush = new SolidBrush(Color.Black);
        Font GameFont = new System.Drawing.Font("Arial", 16);

        // Game Over and Paused screen
        Font BigFont = new System.Drawing.Font("Arial", 26);
        PointF BigTextLoc;
        Rectangle BackgroundRect;

        public Thread GameThread;

        // constructor
        public TetrisGame()
        {
            score = 0;
            cGameState = GameState.Stop;
        }

        // Game tick update
        private void GameTick()
        {
            DropTimer.Start();
            while (cGameState != GameState.Stop)
            {
                if (cGameState == GameState.Running)
                {
                    // Check full lines
                    CheckFullRow();
                    // Drop down piece
                    DropTimer.Stop();
                    if (DropTimer.ElapsedMilliseconds > (500 - score / 50))
                    {
                        cPiece.Loc.Y--;
                        DropTimer.Reset();
                    }
                    else DropTimer.Start();

                    // Attach piece to well
                    if (CheckWellCollision())
                    {
                        cPiece.Loc.Y++;
                        AttachPiece();
                    }
                    if (cPiece.Loc.Y == 0) AttachPiece();

                    // Check for fully completed rows and clear them  update score
                }
                Canvas.Invalidate();
                Thread.Sleep(3);
            }
            DropTimer.Stop();
        }

        private void NextPiece()
        {
            nPiece.Loc = StartPos;
            nPiece.cGameState = Tetramino.GameState.active;
            cPiece = nPiece;
            nPiece = new Tetramino(Tetramino.GetRandomType(), new Point(), Tetramino.GameState.next);
        }

        // Start function
        public void Start()
        {
            // Set up well dimensions
            int WellW = Canvas.Width / 4;
            int WellH = Canvas.Height - (Canvas.Height / 5);
            int WellX = Canvas.Width / 2 - WellW / 2;
            int WellY = Canvas.Height / 10;

            WellRect = new Rectangle[210];
            // Start at the bottom of the well
            for (int i = 0; i < 210; i++)
            {
                WellColor[i] = BlackBrush;
                WellRect[i] = new Rectangle(WellX + (WellW / 10 * (i % 10)), WellH + (WellH / 21) - ((WellH / 21) * (i / 10)), WellW / 10, WellH / 21);
            }

            int nextX = WellX + WellW + 20;
            int nextholdY = WellY + 20 + 3*PreviewCellSize;
            int holdX = WellX - 20 - 4 * PreviewCellSize;
            NextRect = new Rectangle[16];
            HoldRect = new Rectangle[16];
            // Rectangles for preview
            for(int i = 0; i < 16; i++)
            {
                NextRect[i] = new Rectangle(nextX + (i%4)*PreviewCellSize, nextholdY - (PreviewCellSize * (i / 4)), PreviewCellSize,PreviewCellSize);
                HoldRect[i] = new Rectangle(holdX + (i%4)*PreviewCellSize, nextholdY - (PreviewCellSize * (i / 4)), PreviewCellSize, PreviewCellSize);
            }

            // Setup for Game over and Paused screen
            BigTextLoc = new PointF(WellX + WellW / 4, WellY + 4 * WellH / 12);
            BackgroundRect = new Rectangle(WellX - 20, WellY + WellH / 4, WellW + 40, WellH / 3);

            scoreLoc = new PointF(WellX + WellW + 20, WellY + 40 + 4 * PreviewCellSize);
            score = 0;

            // Add tetromino piece
            cPiece = new Tetramino(Tetramino.GetRandomType(), StartPos, Tetramino.GameState.active);
            nPiece = new Tetramino(Tetramino.GetRandomType(), new Point(), Tetramino.GameState.next);
            hPiece = new Tetramino(Tetramino.TetraType.Empty, new Point(), Tetramino.GameState.hold);

            GameThread = new Thread(GameTick);
            cGameState = GameState.Running;
            DropTimer = new Stopwatch();
            GameThread.Start();
        }

        // Setup function
        public void Setup()
        {
            Canvas = new Panel();
            Canvas.Location = new System.Drawing.Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new System.Drawing.Size(400, 300);
            Canvas.TabIndex = 0;
            Canvas.Dock = DockStyle.Fill;

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, Canvas, new object[] { true });

            Canvas.Paint += Canvas_Paint;
        }


        // Draw function
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            // Draw Well backround
            for (int i = 0; i < 210; i++)
            {
                e.Graphics.FillRectangle(WellColor[i], WellRect[i]);
            }
            // Draw current piece
            this.Piece_Paint(e);
            // Draw Well grid
            e.Graphics.DrawRectangles(new Pen(Color.DarkBlue), WellRect);

            // HUD
            e.Graphics.DrawString("Score: " + score, GameFont, BlackBrush, scoreLoc);
            DrawNextPiece(e);
            DrawHoldPiece(e);

            DrawPause(e);
            DrawGameOver(e);
        }

        private void DrawGameOver(PaintEventArgs e)
        {
            if (cGameState == GameState.GameOver)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Gray), BackgroundRect);
                e.Graphics.DrawString("Game Over", BigFont, BlackBrush, BigTextLoc);

                e.Graphics.DrawString("Your score is: " + score, GameFont, BlackBrush, PointF.Add(BigTextLoc, new Size(-20,60)));
            }
        }

        private void DrawPause(PaintEventArgs e)
        {
            if (cGameState == GameState.Pause)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Gray),BackgroundRect);
                e.Graphics.DrawString("Paused", BigFont, BlackBrush, PointF.Add(BigTextLoc, new Size(30, 20)));
            }
        }

        private void DrawHoldPiece(PaintEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                e.Graphics.FillRectangle(BlackBrush, HoldRect[i]);
            }
            if (hPiece.GetTypeIndex() != (int)Tetramino.TetraType.Empty)
            {
                int[] pieceD = hPiece.GetPreviewIndex();
                SolidBrush pieceB = new SolidBrush(hPiece.GetColor());
                for (int i = 0; i < 4; i++)
                {
                    if (pieceD[i] < 16)
                        e.Graphics.FillRectangle(pieceB, HoldRect[pieceD[i]]);
                }
            }
            e.Graphics.DrawRectangles(new Pen(Color.DarkBlue), HoldRect);
        }

        private void DrawNextPiece(PaintEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                e.Graphics.FillRectangle(BlackBrush, NextRect[i]);
            }
            int[] pieceD = nPiece.GetPreviewIndex();
            SolidBrush pieceB = new SolidBrush(nPiece.GetColor());
            for (int i = 0; i < 4; i++)
            {
                if (pieceD[i] < 16)
                    e.Graphics.FillRectangle(pieceB, NextRect[pieceD[i]]);
            }
            e.Graphics.DrawRectangles(new Pen(Color.DarkBlue), NextRect);
        }

        private void Piece_Paint(PaintEventArgs e)
        {
            int[] pieceD = cPiece.GetWellRectIndex();
            SolidBrush pieceB = new SolidBrush(cPiece.GetColor());
            for (int i = 0; i < 4; i++)
            {
                if (pieceD[i] < 210)
                    e.Graphics.FillRectangle(pieceB, WellRect[pieceD[i]]);
            }
        }

        public void MoveLeft()
        {
            if (cGameState == GameState.Running)
            {
                cPiece.Loc.X -= 1;
                if (cPiece.Loc.X + cPiece.minX() < 0 || CheckWellCollision()) cPiece.Loc.X++;
            }
        }
        public void MoveRight()
        {
            if (cGameState == GameState.Running)
            {
                cPiece.Loc.X++;
                if (cPiece.Loc.X + cPiece.maxX() >= 10) cPiece.Loc.X = 9 - cPiece.maxX();
                if (CheckWellCollision())
                {
                    cPiece.Loc.X--;
                }
            }
        }

        public void DropDown()
        {
            if (cGameState == GameState.Running)
            {
                cPiece.Loc.Y--;
                if (cPiece.Loc.Y < 0) cPiece.Loc.Y = 0;
                if (CheckWellCollision())
                {
                    cPiece.Loc.Y++;
                }
            }
        }
        
        public void HardDrop()
        {
            if (cGameState == GameState.Running)
            {
                while (!CheckWellCollision())
                {
                    cPiece.Loc.Y--;
                }
                cPiece.Loc.Y++;
            }
        }

        public void HoldPiece()
        {
            if (cGameState == GameState.Running)
            {
                if (hPiece.GetTypeIndex() == (int)Tetramino.TetraType.Empty)
                {
                    hPiece = cPiece;
                    hPiece.cGameState = Tetramino.GameState.hold;
                    this.NextPiece();
                    cPiece.Loc = hPiece.Loc;
                }
                else
                {
                    Tetramino tmpPiece = hPiece;
                    tmpPiece.cGameState = Tetramino.GameState.active;
                    tmpPiece.Loc = cPiece.Loc;
                    hPiece = cPiece;
                    hPiece.cGameState = Tetramino.GameState.hold;
                    cPiece = tmpPiece;
                }
                while (CheckWellCollision())
                {
                    RotatePiece();
                    if (CheckWellCollision()) RotatePiece();
                    else return;
                    if (CheckWellCollision()) RotatePiece();
                    else return;
                    if (CheckWellCollision()) RotatePiece();
                    else return;
                    cPiece.Loc.Y++;
                }
            }
        }

        public void RotatePiece()
        {
            if (cGameState == GameState.Running)
            {
                cPiece.Rotate();
                if (CheckWellCollision())
                {
                    cPiece.Rotate();
                    cPiece.Rotate();
                    cPiece.Rotate();
                }
            }
        }

        bool CheckWellCollision()
        {
            int[] indecies = cPiece.GetWellRectIndex();
            for (int i = 0; i < 4; i++)
            {
                if (indecies[i] == 500 || indecies[i] < 0) return true;
                if (indecies[i] < 210)
                        if (!WellColor[indecies[i]].Equals(BlackBrush))
                            return true;
            }
            return false;
        }

        private void AttachPiece()
        {
            int[] indecies = cPiece.GetWellRectIndex();
            for (int i = 0; i < 4; i++)
            {
                if (indecies[i] < 210)
                    WellColor[indecies[i]] = new SolidBrush(cPiece.GetColor());
                else if (indecies[i] < 300)
                    GameOver();
            }
            NextPiece();
        }

        private void CheckFullRow()
        {
            List<int> rows = new List<int>();
            for (int y = 200; y >= 0; y -= 10)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (WellColor[x + y].Equals(BlackBrush)) break;
                    if (x == 9) // The entire row is full
                    {
                        rows.Add(y);
                    }
                }
            }
            score += ScoreVal(rows.Count());
            foreach (int row in rows)
            {
                for (int y = row; y < 210; y += 10)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        if (x + y + 10 >= 210)
                            WellColor[x + y] = BlackBrush;
                        else
                            WellColor[x + y] = WellColor[x + y + 10];
                    }
                }
            }
        }

        int ScoreVal(int n)
        {
            switch (n)
            {
                case 1:
                    return 10;
                case 2:
                    return 30;
                case 3:
                    return 70;
                case 4:
                    return 120;
                default:
                    return 0;
            }
        }

        public void PauseGame()
        {
            if (cGameState == GameState.Running)
            {
                cGameState = GameState.Pause;
            }
        }

        public bool IsPaused()
        {
            return cGameState == GameState.Pause;
        }

        public void ResumeGame()
        {
            if (cGameState == GameState.Pause)
            {
                cGameState = GameState.Running;
            }
        }

        public void GameOver()
        {
            cGameState = GameState.GameOver;
        }

        public bool IsGameOver()
        {
            return cGameState == GameState.GameOver;
        }

        public void RestartGame()
        {
            if( cGameState == GameState.GameOver)
            {
                cGameState = GameState.Running;
                this.Start();
            }
        }

        public void StopGame()
        {
            cGameState = GameState.Stop;
        }

        // Convert from 2d to 1d
        public static int map2to1(int x, int y, int width)
        {
            return x + y * width;
        }

    }
}
