using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TetrisThing
{
    public class GameController
    {
        private Piece _currentPiece;

        public Piece CurrentPiece
        {
            get => _currentPiece;
            set
            {
                _currentPiece = value;
                _currentPiece.Reset();
            }
        }

        public Board Board { get; private set; }

        public Bag Bag { get; private set; }

        public bool IsDead { get; private set; }

        public int Score { get; private set; }
        public Piece HeldPiece { get; private set; }
        public bool CanHold { get; private set; }

        
        //Saved elements
        private Piece CurrentPieceSave { get; set; }
        private Board BoardSave { get; set; }
        private Bag BagSave { get; set; }
        private bool IsDeadSave { get; set; }
        private int ScoreSave { get; set; }
        private Piece HeldPieceSave { get; set; }
        private bool CanHoldSave { get; set; }


        public GameController()
        {
            Board = new Board(20, 10);
            Bag = new Bag();
            CurrentPiece = Bag.GetPiece();
            CanHold = true;
        }

        public void SaveGameState()
        {
            CurrentPieceSave = (Piece)CurrentPiece.Clone();
            Board.Save();
            Bag.Save();
            if(HeldPiece != null)
                HeldPieceSave = (Piece)HeldPiece.Clone();
            else
                HeldPieceSave = HeldPiece;
            CanHoldSave = CanHold;
            ScoreSave = Score;
            IsDeadSave = IsDead;
        }

        public void LoadGameState()
        {
            CurrentPiece = CurrentPieceSave;
            Board.Load();
            Bag.Load();
            HeldPiece = HeldPieceSave;
            CanHold = CanHoldSave;
            Score = ScoreSave;
            IsDead = IsDeadSave;
        }

        public void HoldPiece()
        {
            if (!CanHold)
                return;
            else
            {
                if(HeldPiece == null)
                {
                    HeldPiece = CurrentPiece;
                    CurrentPiece = Bag.GetPiece();
                }
                else
                {
                    var temp = CurrentPiece;
                    CurrentPiece = HeldPiece;
                    HeldPiece = temp;
                }
            }
            CurrentPiece.Reset();
            CanHold = false;
        }

        private bool IsPieceInBounds()
        {
            foreach (var p in CurrentPiece.CellPositions())
            {
                if (!Board.IsCellEmpty(p.X, p.Y))
                    return false;
            }
            return true;
        }

        public void RotateClockWise()
        {
            CurrentPiece.RotateClockWise();
            if (!IsPieceInBounds())
                CurrentPiece.RotateCounterClockWise();
        }

        public void RotateCounterClockWise()
        {
            CurrentPiece.RotateCounterClockWise();
            if (!IsPieceInBounds())
                CurrentPiece.RotateClockWise();
        }

        public void MoveLeft()
        {
            CurrentPiece.Move(0, -1);
            if (!IsPieceInBounds())
                CurrentPiece.Move(0, 1);
        }

        public void MoveRight()
        {
            CurrentPiece.Move(0, 1);
            if (!IsPieceInBounds())
                CurrentPiece.Move(0, -1);
        }

        private bool IsGameOver()
        {
            return !(Board.IsRowEmpty(0) && Board.IsRowEmpty(1));
        }

        private void PlacePiece()
        {
            foreach (var item in CurrentPiece.CellPositions())
            {
                Board[item.X, item.Y] = (int)CurrentPiece.Shape;
            }

            int rows = Board.ClearRows();

            switch (rows)
            {
                case 1:
                    this.Score += 40;
                    break;
                case 2:
                    this.Score += 100;
                    break;
                case 3:
                    this.Score += 300;
                    break;
                case 4:
                    this.Score += 1200;
                    break;
                default:
                    break;
            }
            this.Score += 1;


            if (IsGameOver())
                IsDead = true;
            else
            {
                CurrentPiece = Bag.GetPiece();
                CanHold = true;
            }
        }

        public void MoveDown()
        {
            CurrentPiece.Move(1, 0);

            if (!IsPieceInBounds())
            {
                CurrentPiece.Move(-1, 0);
                //PlacePiece();
            }
        }

        private int TileDropDistance(Point p)
        {
            int drop = 0;

            while (Board.IsCellEmpty(p.X + drop + 1, p.Y))
            {
                drop++;
            }
            return drop;
        }

        public int PieceDropDistance()
        {
            int drop = Board.Rows;

            foreach (var p in CurrentPiece.CellPositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
            
        }

        public void HardDrop()
        {
            CurrentPiece.Move(PieceDropDistance(), 0);
            PlacePiece();

            int heightsum;
            int bumpiness;
            int relativeHeight;
            GetOverHangs();
            GetHeightSumAndBumpiness(out heightsum, out bumpiness, out relativeHeight);
        }


        public int GetOverHangs()
        {
            int holes = 0;

            for (int c = 0; c < Board.Columns; c++)
            {
                for (int r = Board.Rows-1; r > 0; r--)
                {
                    if(Board[r,c] == 0)
                    {
                        for (int i = r; i > 0; i--)
                        {
                            if(Board[i,c] != 0)
                            {
                                holes++;
                                break;
                            }
                        }
                    }
                }
            }

            return holes;
        }

        //
        public void GetHeightSumAndBumpiness(out int heightSum, out int bumpiness, out int relativeHeight)
        {
            double sum = 0;
            List<int> columnHeights = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            for (int c = 0; c < Board.Columns; c++)
            {
                for (int r = 0; r < Board.Rows; r++)
                {
                    if (Board[r, c] != 0)
                    {
                        columnHeights[c] = (Board.Rows - r);
                        break;
                    }
                }
            }

            heightSum = HeightSum(columnHeights);
            bumpiness = Bumpiness(columnHeights);
            try
            {
                relativeHeight = columnHeights.Max() - columnHeights.Min();
            }
            catch
            {
                relativeHeight = 0;
                Debug.WriteLine($"Error while calculating relativeHeight: Columns heights count: {columnHeights.Count}");
            }
        }

        private int HeightSum(List<int> columnHeights)
        {
            return columnHeights.Sum();
        }
        private int Bumpiness(List<int> columnHeights)
        {
            int sum = 0;
            for (int i = 0; i < columnHeights.Count - 1; i++)
            {
                sum = Math.Abs(columnHeights[i + 1] - columnHeights[i]);
            }
            return sum;
        }
    }
}
