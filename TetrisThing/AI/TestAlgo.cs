using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing.AI
{
    public class TestAlgo
    {
        private int PopulationSize = 100;
        private int MoveCount = 300;
        private float MutationRate = 0.175f;
        private int Elitism = 20;
        public int Generation = 0;
        private GeneticAlgo Algorithm { get; set; }
        private Random Rand { get; set; }

        private MainWindow MainWindow { get; set; }

        public TestAlgo(MainWindow mw)
        {
            Rand = new Random();
            MainWindow = mw;
            Algorithm = new GeneticAlgo(PopulationSize, MoveCount, Rand, FitnessFunction, Elitism, MutationRate, MainWindow);
        }

        public async Task NewGeneration()
        {
            Guy showcase;
            for (int i = 0; i < 50; i++)
            {
                Generation = i;
                Algorithm.NewGeneration(MoveCount);


                showcase = Algorithm.BestGuy;
                showcase.shoulDraw = true;

                await GameLoop(showcase, i);
            }
        }

        public async Task GameLoop(Guy guy, int genWhenStarted)
        {
            guy.controller = new GameController();
            for (int i = 0; i < 100 + 15 * Generation; i++)
            {
                guy.MakeMove();
                await Task.Delay(50);
                if (guy.controller.IsDead)
                {
                    break;
                    //if (genWhenStarted != this.Generation)
                    //    break;
                    //else
                    //    guy.controller = new GameController();
                }
            }
        }

        public double BestHolesW()
        {
            return Algorithm.BestHolesWeight;
        }
        public double BestBumpinessW()
        {
            return Algorithm.BestBumpinessWeight;
        }
        public double BestRelativeHeight()
        {
            return Algorithm.BestRelativeHeightWeight;
        }
        public double BestHeightSumW()
        {
            return Algorithm.BestHeightSumWeight;
        }
        public double BestScoreW()
        {
            return Algorithm.BestScoreWeight;
        }

        public GameController GetController()
        {
            return Algorithm.Population[0].controller;
        }
        public async Task Run()
        {
            await NewGeneration();
        }

        private double FitnessFunction(int index)
        {
            double rating = 0;
            Guy guy = Algorithm.Population[index];

            int bumpiness;
            int heightSum;
            int relativeHeight;

            guy.controller.GetHeightSumAndBumpiness(out bumpiness, out heightSum, out relativeHeight);

            rating += guy.weight_Score * (double)guy.controller.Score;
            rating += guy.weight_holes * (double)guy.controller.GetOverHangs();
            rating += guy.weight_bumpiness * (double)bumpiness;
            rating += guy.weight_heightSum * (double)heightSum;
            rating += guy.weight_relativeHeight * (double)relativeHeight;
            if (guy.controller.IsDead)
                rating -= 10000;

            return rating;
        }
    }
}
