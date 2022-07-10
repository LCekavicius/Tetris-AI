using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing.AI
{
    
    public class Guy
    {
        public int Id { get; set; }
        public GameController controller { get; set; }
        public double weight_heightSum { get; set; }
        public double weight_Score { get; set; }
        public double weight_bumpiness { get; set; }
        public double weight_holes { get; set; }
        public double weight_relativeHeight { get; set; }
        public Random Rand { get; set; }
        public double Fitness { get; set; }
        public int MoveCount { get; set; }
        private Func<int, double> FitnessFunction;
        private MainWindow MainWindow { get; set; }
        public bool shoulDraw = false;

        public Guy(int id,int moveCount, Random rand, Func<int, double> fitnessFunc, MainWindow mw, GameController controller = null, bool shouldInitWeights = true)
        {
            this.Id = id;
            if(controller != null)
                this.controller = controller;
            else
                this.controller = new GameController();
            this.Rand = rand;
            this.FitnessFunction = fitnessFunc;
            this.MoveCount = moveCount;
            this.MainWindow = mw;
            if (shouldInitWeights)
            {
                weight_heightSum = Rand.NextDouble() - 0.5;
                weight_bumpiness = Rand.NextDouble() - 0.5;
                weight_holes = Rand.NextDouble() - 0.5;
                weight_Score = Rand.NextDouble() - 0.5;
                //weight_holes = -0.66834974383601999;
                //weight_heightSum = -0.1568519443204654;
                //weight_relativeHeight = 0.3016243109617487;
                //weight_bumpiness = 0.3016243109617487;
                //weight_Score = 0.8840099436321385;
            }
            

        }

        public double CalculateFitness(int index)
        {
            for (int i = 0; i < MoveCount; i++)
            {
                MakeMove();
                if (Id == 0)
                {
                    //MainWindow.Draw(this.controller);
                }
                if (controller.IsDead)
                    break;
            }
            Fitness = FitnessFunction(index);
            return Fitness;
        }

        public void Mutate(double mutationRate, double mutationStep = 0.33)
        {

            //if (Rand.NextDouble() < mutationRate)
            //    weight_heightSum = Rand.NextDouble() - 0.5;
            //if (Rand.NextDouble() < mutationRate)
            //    weight_bumpiness = Rand.NextDouble() - 0.5;
            //if (Rand.NextDouble() < mutationRate)
            //    weight_holes = Rand.NextDouble() - 0.5;
            //if (Rand.NextDouble() < mutationRate)
            //    weight_Score = Rand.NextDouble() - 0.5;

            if (Rand.NextDouble() < mutationRate)
                weight_heightSum = weight_heightSum + (Rand.NextDouble() - 0.5) * mutationStep;
            if (Rand.NextDouble() < mutationRate)
                weight_bumpiness = weight_bumpiness + (Rand.NextDouble() - 0.5) * mutationStep;
            if (Rand.NextDouble() < mutationRate)
                weight_holes = weight_holes + (Rand.NextDouble() - 0.5) * mutationStep;
            if (Rand.NextDouble() < mutationRate)
                weight_Score = weight_Score + (Rand.NextDouble() - 0.5) * mutationStep;
            if (Rand.NextDouble() < mutationRate)
                weight_relativeHeight = weight_relativeHeight + (Rand.NextDouble() - 0.5) * mutationStep;


        }

        public Guy CrossOver(Guy other, int moveCount, int id, MainWindow mw)
        {
            Guy child = new Guy(id, moveCount, Rand, FitnessFunction,mw, shouldInitWeights: false);

            child.weight_bumpiness = Rand.NextDouble() < 0.5 ? this.weight_bumpiness : other.weight_bumpiness;
            child.weight_holes = Rand.NextDouble() < 0.5 ? this.weight_holes : other.weight_holes;
            child.weight_Score = Rand.NextDouble() < 0.5 ? this.weight_Score : other.weight_Score;
            child.weight_relativeHeight = Rand.NextDouble() < 0.5 ? this.weight_relativeHeight : other.weight_relativeHeight;
            child.weight_heightSum = Rand.NextDouble() < 0.5 ? this.weight_heightSum : other.weight_heightSum;

            return child;
        }

        public List<Move> GetAllMoves()
        {
            List<Move> moves = new List<Move>();
            controller.SaveGameState();
            moves.AddRange(GetAllMovesA());
            //moves.AddRange(GetAllMovesA(true));

            return moves;
        }
        public List<Move> GetAllMovesA(bool withHold = false)
        {
            List<Move> moves = new List<Move>();
            if (withHold)
            {
                this.controller.HoldPiece();
            }
            //controller.SaveGameState();
            int iter = 0;
            for (int rots = 0; rots < 4; rots++)
            {
                for (int t = -5; t < 6; t++)
                {
                    iter++;
                    controller.LoadGameState();
                    for (int j = 0; j < rots; j++)
                    {
                        controller.RotateClockWise();
                    }
                    if (t < 0)
                    {
                        for (int l = 0; l < Math.Abs(t); l++)
                        {
                            controller.MoveLeft();
                        }
                    }
                    else if (t > 0)
                    {
                        for (int r = 0; r < t; r++)
                        {
                            controller.MoveRight();
                        }
                    }

                    controller.HardDrop();

                    int bumpiness;
                    int heightSum;
                    int relativeHeight;

                    controller.GetHeightSumAndBumpiness(out bumpiness, out heightSum, out relativeHeight);

                    double rating = 0;
                    rating += weight_Score * (double)controller.Score;
                    rating += weight_holes * (double)controller.GetOverHangs();
                    rating += weight_bumpiness * (double)bumpiness;
                    rating += weight_heightSum * (double)heightSum;
                    rating += weight_relativeHeight * (double)relativeHeight;
                    if (controller.IsDead)
                        rating -= 10000;

                    moves.Add(new Move(rots, t, rating, withHold));
                }      
            }

            controller.LoadGameState();
            return moves;
        }

        public Move GetHighestRatedMove(List<Move> allPossibleMoves)
        {
            Move maxRatingMove = new Move(0, 0, int.MinValue, false);
            foreach (Move move in allPossibleMoves)
            {
                if(move.Rating > maxRatingMove.Rating)
                    maxRatingMove = move;
            }

            return maxRatingMove;
        }
        public void MakeMove()
        { 
            Move move = GetHighestRatedMove(GetAllMoves());
            if (move.HoldPiece)
                controller.HoldPiece();

            for (int i = 0; i < move.RotationState; i++)
            {
                controller.RotateClockWise();
            }

            for (int i = 0; i < Math.Abs(move.Transform); i++)
            {
                if(move.Transform < 0)
                    controller.MoveLeft();
                else
                    controller.MoveRight();
            }

            controller.HardDrop();

            if (shoulDraw)
            {                    
                this.MainWindow.Draw(this.controller);
            }
        }

    }
}
