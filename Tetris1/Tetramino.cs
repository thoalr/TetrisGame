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
    public class Tetramino
    {
        public enum TetraType
        {
            I, O, L, J, T, S, Z, Empty
        }
        private TetraType cType; // Type of this instance of Tetramino

        public enum PieceState
        {
            active, next, hold
        }
        private PieceState cState;  // Type of what this piece is in the game
        // colors
        static SolidBrush[] TetraColor = { new SolidBrush( Color.Cyan ), new SolidBrush(Color.Yellow), new SolidBrush(Color.Orange), new SolidBrush(Color.Blue), new SolidBrush(Color.Purple), new SolidBrush(Color.Green), new SolidBrush(Color.Red) };

        private Point Loc;

        static Point[,] TetraShapes = {
            { new Point(0,1), new Point(1,1), new Point(2,1), new Point(3,1)}, // I
            { new Point(1,1), new Point(2,1), new Point(1,2), new Point(2,2)}, // O
            { new Point(0,1), new Point(1,1), new Point(2,1), new Point(2,2)}, // L
            { new Point(0,2), new Point(0,1), new Point(1,1), new Point(2,1)}, // J
            { new Point(0,1), new Point(1,1), new Point(2,1), new Point(1,2)}, // T
            { new Point(0,1), new Point(1,1), new Point(1,2), new Point(2,2)}, // S
            { new Point(0,2), new Point(1,2), new Point(1,1), new Point(2,1)}  // Z
        };

        private Point[] cShape = new Point[4];

        private int rotationState = 0;


        // contsructor
        public Tetramino(TetraType type, Point loc, PieceState state)
        {
            cType = type;
            Loc = loc;
            cState = state;
            this.FillTetra();
        }

        public TetraType GetTetraType()
        {
            return cType;
        }

        public PieceState GetState()
        {
            return cState;
        }

        public Tetramino Activate(Point loc)
        {
            Loc = loc;
            cState = PieceState.active;
            return this;
        }

        public Tetramino Hold()
        {
            cState = PieceState.hold;
            return this;
        }

        public void Drop()
        {
            Loc.Y--;
        }
        public void MoveUp()
        {
            Loc.Y++;
        }

        public void MoveLeft()
        {
            Loc.X--;
        }
        public void MoveRight()
        {
            Loc.X++;
        }

        public Point GetLocation()
        {
            return Loc;
        }

        public void SetLocation(Point p)
        {
            Loc = p;
        }

        // Initialize shape for tetramino
        private void FillTetra()
        {
            if (this.cType == TetraType.Empty) return;
            for (int i = 0; i < 4; i++)
            {
                this.cShape[i] = TetraShapes[(int)this.cType, i];
            }
        }

        static public TetraType GetRandomType()
        {
            Random r = new Random();
            return (TetraType)r.Next(7);
        }

        static public Tetramino NextPiece()
        {
            return new Tetramino(GetRandomType(), new Point(), PieceState.next);
        }

        // Returns indecies based on current state of object
        public int[] GetIndecies()
        {
            if (cState == PieceState.active)
            {
                int[] list = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    list[i] = Loc.X + cShape[i].X + (Loc.Y + cShape[i].Y) * 10;
                    if (Loc.X + cShape[i].X >= 10 || Loc.X + cShape[i].X < 0) list[i] = 500; // If it is outside the well
                    if (Loc.Y + cShape[i].Y > 21) list[i] = 501; // If it is above the well
                }
                return list;
            }
            else
            {
                int[] list = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    list[i] = cShape[i].X + cShape[i].Y * 4;
                    if (cShape[i].X >= 10 || cShape[i].X < 0) list[i] = 500;
                }
                return list;
            }
        }

        // Get the index for the current tetramino
        private int GetTypeIndex()
        {
            return (int)this.cType;
        }

        // Get Color of piece
        public SolidBrush GetColor()
        {
            return TetraColor[(int)this.cType];
        }

        // Rotate 90 degrees clockwise
        public void Rotate()
        {
            rotationState = (rotationState + 1) % 4;
            // rotate in place not around (0,0)
            /* 1 2 3 0    0 7 4 1
             * 4 5 6 0    0 8 5 2
             * 7 8 9 0    0 9 6 3
             * 0 0 0 0    0 0 0 0
             * (1,1)->(1,2), (0,0)->(0,3), (0,1)->(1,3)
             * (2,1)->(1,1), (3,3)->(3,0), (0,3)->(3,3)
             */
            if (cType == TetraType.O || cType == TetraType.I)
                for (int i = 0; i < 4; i++)
                {
                    int x = cShape[i].X;
                    cShape[i].X = cShape[i].Y;
                    cShape[i].Y = (7 - x) % 4;
                }
            /* 1 2 3   7 4 1
             * 4 5 6   8 5 2
             * 7 8 9   9 6 3
             * (0,0)->(0,2), (1,1)->(1,1)
             */
            else
                for (int i = 0; i < 4; i++)
                {
                    int x = cShape[i].X;
                    cShape[i].X = cShape[i].Y;
                    cShape[i].Y = (5 - x) % 3;
                }
        }

        public Point[] PreviewRotation(int n)
        {
            Point[] tmp = new Point[4];
            tmp = this.cShape;
            for (int j = 1; j < n; j++)
            {
                // rotate in place not around (0,0)
                /* 1 2 3 0    0 7 4 1
                 * 4 5 6 0    0 8 5 2
                 * 7 8 9 0    0 9 6 3
                 * 0 0 0 0    0 0 0 0
                 * (1,1)->(1,2), (0,0)->(0,3), (0,1)->(1,3)
                 * (2,1)->(1,1), (3,3)->(3,0), (0,3)->(3,3)
                 */
                if (cType == TetraType.O || cType == TetraType.I)
                    for (int i = 0; i < 4; i++)
                    {
                        int x = tmp[i].X;
                        tmp[i].X = tmp[i].Y;
                        tmp[i].Y = (7 - x) % 4;
                    }
                /* 1 2 3   7 4 1
                 * 4 5 6   8 5 2
                 * 7 8 9   9 6 3
                 * (0,0)->(0,2), (1,1)->(1,1)
                 */
                else
                    for (int i = 0; i < 4; i++)
                    {
                        int x = tmp[i].X;
                        tmp[i].X = tmp[i].Y;
                        tmp[i].Y = (5 - x) % 3;
                    }
            }
            return tmp;
        }

        public int[] PreviewMove(Point p)
        {
            int[] list = new int[4];
            for (int i = 0; i < 4; i++)
            {
                list[i] = Loc.X + p.X + cShape[i].X + (Loc.Y + p.Y + cShape[i].Y) * 10;
                if (Loc.X + p.X + cShape[i].X >= 10 || Loc.X + p.X  + cShape[i].X < 0) list[i] = 500; // If it is outside the well
                if (Loc.Y + p.Y + cShape[i].Y > 21) list[i] = 501; // If it is above the well
            }
            return list;
        }

        private int maxX()
        {
            int max = 0;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].X > max) max = cShape[i].X;
            }
            return max;
        }
        private int minX()
        {
            int min = 4;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].X < min) min = cShape[i].X;
            }
            return min;
        }

        public int LeftestX()
        {
            return minX() + Loc.X;
        }
        public int RightestX()
        {
            return maxX() + Loc.X;
        }

        private int maxY()
        {
            int max = 0;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].Y > max) max = cShape[i].Y;
            }
            return max;
        }

        private int minY()
        {
            int min = 4;
            for (int i = 0; i < 4; i++)
            {
                if (cShape[i].Y < min) min = cShape[i].Y;
            }
            return min;
        }

        public int LowestY()
        {
            return minY() + Loc.Y;
        }

        public int HighestY()
        {
            return maxY() + Loc.Y;
        }


        public void Render()
        {
            if(cState == PieceState.active)
            {

            }
        }


        public override string ToString()
        {
            return "Shape is: " + cType;
        }
    }
}
