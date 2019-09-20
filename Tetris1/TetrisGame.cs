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

        SolidBrush BlackBrush = new SolidBrush(Color.Black);

        // Make queue for inputs
        // Disable inputs when doing important long tasks e.g. painting


        public Thread GameThread;

        // constructor
        public TetrisGame()
        {
            Setup();

            cGameState = GameState.Stop;
   
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

            typeof(Panel).InvokeMember("DoubleBuffered", 
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, Canvas, new object[] { true });

            Canvas.Paint += Canvas_Paint;
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
            cPiece = new Tetramino(Tetramino.GetRandomType(0), StartPos, Tetramino.PieceState.active);
            nPiece = new Tetramino(Tetramino.GetRandomType(1), new Point(), Tetramino.PieceState.next);
            hPiece = new Tetramino(Tetramino.TetraType.Empty, new Point(), Tetramino.PieceState.hold);

            GameThread = new Thread(GameTick);
            cGameState = GameState.Running;
            GameThread.Start();
        }


        // Game tick update
        private void GameTick()
        {
            long time0 = DateTime.Now.Ticks;

            while (cGameState != GameState.Stop)
            {
                if (cGameState == GameState.Running)
                {
                    if(DateTime.Now.Ticks - time0 > TimeSpan.TicksPerMillisecond*(800 - (score/50)))
                    {
                        // Clear all full rows
                        ClearRows();

                        // Attach piece if it collides with pieces in well
                        if (MoveCollision(new Point(0,-1)))
                        {
                            AttachPiece();
                        }
                        // Check full rows
                        CheckRows();
                        //Drop down only if it doesnt overlap with piece
                        DropDown();
                        time0 = DateTime.Now.Ticks;
                    }
                }
                Canvas.Invalidate();
                Thread.Sleep(17);
            }
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

            RenderPiece(e.Graphics);

            // Draw Well grid
            e.Graphics.DrawRectangles(new Pen(Color.DarkBlue), WellRect);

            // Draw UI
            RenderHoldPiece(e.Graphics);
            RenderNextPiece(e.Graphics);
            RenderScore(e.Graphics);
            if (cGameState.Equals(GameState.Pause)) RenderPause(e.Graphics);
            if (cGameState.Equals(GameState.GameOver)) RenderGameover(e.Graphics);
        }


        public void MoveLeft()
        {
            if (cGameState == GameState.Running)
            {
                if (!MoveCollision(new Point(-1, 0))) cPiece.MoveLeft();
            }
        }
        public void MoveRight()
        {
            if (cGameState == GameState.Running)
            {
                if (!MoveCollision(new Point(1, 0))) cPiece.MoveRight();
            }
        }

        public void DropDown()
        {
            if (cGameState == GameState.Running)
            {
                if (!MoveCollision(new Point(0, -1))) cPiece.Drop();
            }
        }
        
        public void HardDrop()
        {
            if (cGameState == GameState.Running)
            {
                while (!MoveCollision(new Point(0, -1)))
                {
                    cPiece.Drop();
                }
            }
        }

        public void HoldPiece()
        {
            if (cGameState == GameState.Running)
            {
                if (hPiece.GetTetraType().Equals(Tetramino.TetraType.Empty))
                {
                    hPiece = cPiece.Hold();
                    cPiece = nPiece.Activate(StartPos);
                    nPiece = Tetramino.NextPiece();
                }
                else
                {
                    Tetramino tmpPiece = hPiece;
                    hPiece = cPiece.Hold();
                    cPiece = tmpPiece.Activate(cPiece.GetLocation());

                    // FIX
                    while (MoveCollision(new Point(0,0)))
                    {
                        RotatePiece();
                        if (MoveCollision(new Point(0,0))) RotatePiece();
                        else return;
                        if (MoveCollision(new Point(0,0))) RotatePiece();
                        else return;
                        if (MoveCollision(new Point(0,0))) RotatePiece();
                        else return;
                        cPiece.MoveUp();
                    }
                }
            }
        }

        public void RotatePiece()
        {
            // FIX
            if (cGameState == GameState.Running)
            {
                cPiece.Rotate();
                while (MoveCollision(new Point(0,0)))
                {
                    cPiece.Rotate();
                }
            }
        }

        private void NextPiece()
        {
            cPiece = nPiece.Activate(StartPos);
            nPiece = Tetramino.NextPiece();
        }

        private void WallKick()
        {
            // Test 5 positions
            // This is to move the tetramino to different positions based on its rotation state
            // https://tetris.fandom.com/wiki/SRS





        }

        /** ScoreVal
         * Returns the score to be awarded for clearing rows
         * input: n - number of rows cleared
         * ouput: the corresponing score
         */
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
