using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TetrisThing
{
    public class IPiece : Piece
    {
        private readonly Point[][] cells = new Point[][]
        {
            new Point[] { new(1,0), new(1,1), new(1,2), new(1,3)},
            new Point[] { new(0,2), new(1,2), new(2,2), new(3,2)},
            new Point[] { new(2,0), new(2,1), new(2,2), new(2,3)},
            new Point[] { new(0,1), new(1,1), new(2,1), new(3,1)}
        };

        public override Shape Shape => Shape.I;
        protected override Point StartOffset => new Point(1, 3);
        protected override Point[][] Cells => cells;
    }
    public class JPiece : Piece
    {
        protected override Point[][] Cells => new Point[][] {
            new Point[] {new(0, 0), new(1, 0), new(1, 1), new(1, 2)},
            new Point[] {new(0, 1), new(0, 2), new(1, 1), new(2, 1)},
            new Point[] {new(1, 0), new(1, 1), new(1, 2), new(2, 2)},
            new Point[] {new(0, 1), new(1, 1), new(2, 1), new(2, 0)}
        };

        public override Shape Shape => Shape.J;
        protected override Point StartOffset => new Point(0, 3);
    }
    public class LPiece : Piece
    {
        protected override Point[][] Cells => new Point[][] {
            new Point[] {new(0,2), new(1,0), new(1,1), new(1,2)},
            new Point[] {new(0,1), new(1,1), new(2,1), new(2,2)},
            new Point[] {new(1,0), new(1,1), new(1,2), new(2,0)},
            new Point[] {new(0,0), new(0,1), new(1,1), new(2,1)}
        };

        public override Shape Shape => Shape.L;
        protected override Point StartOffset => new Point(0, 3);
    }
    public class OPiece : Piece
    {
        private readonly Point[][] cells = new Point[][]
        {
            new Point[] { new(0,0), new(0,1), new(1,0), new(1,1) }
        };

        public override Shape Shape => Shape.O;
        protected override Point StartOffset => new Point(0, 4);
        protected override Point[][] Cells => cells;
    }
    public class SPiece : Piece
    {
        public override Shape Shape => Shape.S;

        protected override Point StartOffset => new(0, 3);

        protected override Point[][] Cells => new Point[][] {
            new Point[] { new(0,1), new(0,2), new(1,0), new(1,1) },
            new Point[] { new(0,1), new(1,1), new(1,2), new(2,2) },
            new Point[] { new(1,1), new(1,2), new(2,0), new(2,1) },
            new Point[] { new(0,0), new(1,0), new(1,1), new(2,1) }
        };
    }

    public class ZPiece : Piece
    {
        public override Shape Shape => Shape.Z;

        protected override Point StartOffset => new(0, 3);

        protected override Point[][] Cells => new Point[][] {
            new Point[] {new(0,0), new(0,1), new(1,1), new(1,2)},
            new Point[] {new(0,2), new(1,1), new(1,2), new(2,1)},
            new Point[] {new(1,0), new(1,1), new(2,1), new(2,2)},
            new Point[] {new(0,1), new(1,0), new(1,1), new(2,0)}
        };
    }
    public class TPiece : Piece
    {
        public override Shape Shape => Shape.T;

        protected override Point StartOffset => new(0, 3);

        protected override Point[][] Cells => new Point[][] {
            new Point[] {new(0,1), new(1,0), new(1,1), new(1,2)},
            new Point[] {new(0,1), new(1,1), new(1,2), new(2,1)},
            new Point[] {new(1,0), new(1,1), new(1,2), new(2,1)},
            new Point[] {new(0,1), new(1,0), new(1,1), new(2,1)}
        };
    }
}
