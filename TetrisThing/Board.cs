using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing
{
    public class Board
    {
        private int[,] PlayField;
        private int[,] PlayFieldSave;

        public int Rows { get; }

        public int Columns { get; }

        public int this[int r, int c]
        {
            get => PlayField[r, c];
            set => PlayField[r, c] = value;
        }

        public void Save()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    PlayFieldSave[r, c] = PlayField[r, c];
                }
            }
        }
        public void Load()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    PlayField[r, c] = PlayFieldSave[r, c];
                }
            }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Board(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;

            PlayField = new int[rows, columns];
            PlayFieldSave = new int[rows, columns];
        }

        public bool IsWithinBoundaries(int x, int y)
        {
            return x >= 0 && x < Rows && y >= 0 && y < Columns;
        }

        public bool IsCellEmpty(int x, int y)
        {
            return IsWithinBoundaries(x, y) && this.PlayField[x, y] == 0;
        }

        public bool IsRowComplete(int x)
        {
            for (int y = 0; y < Columns; y++)
            {
                if (this.PlayField[x, y] == 0)
                    return false;
            }
            return true;
        }

        public bool IsRowEmpty(int x)
        {
            for (int y = 0; y < Columns; y++)
            {
                if (this.PlayField[x, y] != 0)
                    return false;
            }
            return true;
        }

        private void ClearRow(int x)
        {
            for (int y = 0; y < Columns; y++)
            {
                this.PlayField[x, y] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                this.PlayField[r + numRows, c] = this.PlayField[r, c];
                this.PlayField[r, c] = 0;
            }
        }


        public int ClearRows()
        {
            int cleared = 0;

            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowComplete(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }
    }
}
