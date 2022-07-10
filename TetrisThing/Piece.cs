using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing
{
    public abstract class Piece
    {
        protected abstract Point[][] Cells { get; }
        protected abstract Point StartOffset { get; }
        public abstract Shape Shape { get; }
        private int rotationState;
        private Point offset;

        public Piece()
        {
            offset = new Point(StartOffset.X, StartOffset.Y);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public IEnumerable<Point> CellPositions()
        {
            foreach (var item in Cells[rotationState])
            {
                yield return new Point(item.X + offset.X, item.Y + offset.Y);
            }
        }

        public void RotateClockWise()
        {
            rotationState = (rotationState + 1) % Cells.Length;
        }

        public void RotateCounterClockWise()
        {
            if (rotationState == 0)
                rotationState = Cells.Length - 1;
            else
                rotationState--;
        }

        public void Move(int x, int y)
        {
            offset.X += x;
            offset.Y += y;
        }
        public void Reset()
        {
            rotationState = 0;
            offset.X = StartOffset.X;
            offset.Y = StartOffset.Y;
        }
    }
}
