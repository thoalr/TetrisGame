using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Tetris1
{
    // Handles drawing of tetraminos
    // Handles rotation
    class Tetramino
    {
        public enum TetraType
        {
            I, O, L, J, T, S, Z, Empty
        }
        TetraType cType; // Type of this instance of Tetramino

        public enum GameState
        {
            active, next, hold
        }
        public GameState cGameState;  // Type of what this piece is in the game
        // colors
        static Color[] TetraColor = { Color.Cyan, Color.Yellow, Color.Orange, Color.Blue, Color.Purple, Color.Green, Color.Red };

        public Point Loc;

        static Point[,] TetraShapes = {
            { new Point(1,0), new Point(1,1), new Point(1,2), new Point(1,3)}, // I
            { new Point(0,0), new Point(1,0), new Point(0,1), new Point(1,1)}, // O
            { new Point(1,0), new Point(2,0), new Point(1,1), new Point(1,2)}, // L
            { new Point(0,0), new Point(1,0), new Point(1,1), new Point(1,2)}, // J
            { new Point(1,0), new Point(0,1), new Point(1,1), new Point(2,1)}, // T
            { new Point(1,0), new Point(0,1), new Point(1,1), new Point(0,2)}, // S
            { new Point(0,0), new Point(0,1), new Point(1,1), new Point(1,2)}  // Z
        };

        private Point[] cShape = new Point[4];



        // contsructor
        public Tetramino(TetraType type, Point loc, GameState state)
        {
            cType = type;
            Loc = loc;
            cGameState = state;
            this.FillTetra();
        }

        private void FillTetra()
        {
            if (this.cType == TetraType.Empty) return;
            for(int i = 0; i < 4; i++)
            {
                this.cShape[i] = TetraShapes[(int)this.cType, i];
            }
        }

        static public TetraType GetRandomType()
        {
            Random r = new Random();
            return (TetraType) r.Next(7);
        }


        public int[] GetWellRectIndex()
        {
            int[] list = new int[4];
            for (int i = 0; i < 4; i++)
            {
                list[i] = Loc.X + cShape[i].X + (Loc.Y + cShape[i].Y )* 10;
                if (Loc.X + cShape[i].X >= 10 || Loc.X + cShape[i].X < 0) list[i] = 500;
                if (Loc.Y + cShape[i].Y > 21) list[i] = 501;
            }
            return list;
        }

        public int[] GetPreviewIndex()
        {
            int[] list = new int[4];
            for (int i = 0; i < 4; i++)
            {
                list[i] = cShape[i].X + cShape[i].Y * 4;
                if (cShape[i].X >= 10 || cShape[i].X < 0) list[i] = 500;
            }
            return list;
        }

        // Get the index for the current tetramino
        public int GetTypeIndex()
        {
            return (int)this.cType;
        }

        // Get Color of piece
        public Color GetColor()
        {
            return TetraColor[(int)this.cType];
        }

        // Rotate 90 degrees clockwise
        public void Rotate()
        {
            Point[] newShape = new Point[4];
            Point AddPoint = new Point(0, 0);
            // point M (h, k) will become M’ (k, -h).
            for (int i = 0; i < 4; i++)
            {
                newShape[i].X = cShape[i].Y;
                newShape[i].Y = -cShape[i].X;
                if (newShape[i].X < AddPoint.X) AddPoint.X = newShape[i].X;
                if (newShape[i].Y < AddPoint.Y) AddPoint.Y = newShape[i].Y;
            }
            for(int i = 0; i < 4; i++)
            {
                newShape[i].X -= AddPoint.X;
                newShape[i].Y -= AddPoint.Y;
            }
            if(cType == TetraType.L)
            {
                int count = 0;
                for(int i = 0; i < 4; i++)
                {
                    if (newShape[i].X == 1) count++; 
                }
                if(count == 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        newShape[i].X++;
                    }
                }
            }
            if(cType == TetraType.I)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (newShape[i].X == 3)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            newShape[j].Y++;
                        }
                        break;
                    }
                    else if(newShape[i].Y == 3)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            newShape[j].X++;
                        }
                        break;
                    }
                }
            }
            cShape = newShape;
        }

        public int maxX()
        {
            int max = 0;
            for(int i = 0; i < 4; i++)
            {
                if (cShape[i].X > max) max = cShape[i].X;
            }
            return max;
        }
        public int minX()
        {
            int min = 4;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].X < min) min = cShape[i].X;
            }
            return min;
        }
        public int maxY()
        {
            int max = 0;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].Y > max) max = cShape[i].Y;
            }
            return max;
        }

        public override string ToString()
        {
            return "Shape is: " + cType;
        }
    }
}
