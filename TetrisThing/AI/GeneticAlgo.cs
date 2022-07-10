using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing.AI
{
    public class GeneticAlgo
    {
        public List<Guy> Population { get; set; }
        public List<Guy> newPop { get; set; }

        public int Generation { get; set; }
        public double BestFitness { get; set; }
        public double BestHeightSumWeight { get; set; }
        public double BestBumpinessWeight { get; set; }
        public double BestHolesWeight { get; set; }
        public double BestScoreWeight { get; set; }
        public double BestRelativeHeightWeight { get; set; }
        public Guy BestGuy { get; set; }
        public float MutationRate { get; set; }

        private Random Rand = new Random();

        private double FitnessSum { get; set; }
        public int Elitism { get; set; }

        public MainWindow MainWindow { get; set; }

        public GeneticAlgo(int popSize, int moveCount, Random rand, Func<int, double> fitnessFunction, int elitism, float mutationRate, MainWindow mw)
        {
            Generation = 1;
            this.MutationRate = mutationRate;
            Population = new List<Guy>(popSize);
            this.Rand = rand;
            this.Elitism = elitism;
            this.MainWindow = mw;
            for (int i = 0; i < popSize; i++)
            {
                Population.Add(new Guy(i, moveCount, rand, fitnessFunction, MainWindow));
            }
        }

        public void NewGeneration(int moveCount)
        {
            if (Population.Count <= 0)
                return;

            CalculateFitness();

            newPop = new List<Guy>();

            Population.Sort(CompareGuy);

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < Elitism)
                {
                    var newGuy = Population[i];
                    newGuy.Id = i;
                    newGuy.shoulDraw = false;
                    newPop.Add(newGuy);
                }
                else
                {
                    Guy parent1 = ChooseParent(newPop);
                    Guy parent2 = ChooseParent(newPop);

                    Guy child = parent1.CrossOver(parent2, moveCount, newPop.Count, MainWindow);
                    child.Mutate(MutationRate);
                    newPop.Add(child);
                }
            }

            Population = newPop;
            Generation++;
        }

        public int CompareGuy(Guy A, Guy B)
        {
            if (A.Fitness > B.Fitness)
                return -1;
            else if (A.Fitness < B.Fitness)
                return 1;
            else
                return 0;
        }

        public void CalculateFitness()
        {
            FitnessSum = 0;
            Guy best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                FitnessSum += Population[i].CalculateFitness(i);
                if(Population[i].Fitness > best.Fitness)
                    best = Population[i];
            }

            BestFitness = best.Fitness;
            BestBumpinessWeight = best.weight_bumpiness;
            BestHeightSumWeight = best.weight_heightSum;
            BestHolesWeight = best.weight_holes;
            BestScoreWeight = best.weight_Score;
            BestRelativeHeightWeight = best.weight_relativeHeight;
            BestGuy = best;

        }

        private Guy ChooseParent(List<Guy> newPop)
        {
            return newPop[Rand.Next(0, newPop.Count)];
        }
    }
}
