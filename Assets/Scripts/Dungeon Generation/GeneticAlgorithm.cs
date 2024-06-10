using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class GeneticAlgorithm
    {
        private List<Individual> Population { get; set; }
        // the number of the current generation
        public int Generation { get; private set; }
        private readonly double _mutationRate;
        private readonly Random _random;
        // the sum of the fitness values of the generation
        private double _fitnessSum;
        // the individual with the best fitness value from the current generation
        public Individual BestIndividual { get; private set; }

        public GeneticAlgorithm(Random random, int populationSize, double mutationRate)
        {
            _mutationRate = mutationRate;
            _random = random;
            Population = new List<Individual>(populationSize);
            Generation = 1;

            for (var i = 0; i < populationSize; i++)
            {
                Population.Add(new Individual(_random));
            }
            CalculateFitness();
        }

        // function that generates a new generation. It chooses the parents for the crossover and mutates the child
        public void NewGeneration()
        {
            if (Population.Count == 0)
            {
                return;
            }
            var newPopulation = new List<Individual>(Population.Count);
            for (var i = 0; i < Population.Count / 2; i++)
            {
                var parent1 = ChooseParent();
                var parent2 = ChooseParent();
                var children = parent1.Crossover(parent2);
                var child1 = children[0];
                var child2 = children[1];
                child1.Mutate(_mutationRate);
                child2.Mutate(_mutationRate);
                newPopulation.Add(child1);
                newPopulation.Add(child2);
            }

            Population = newPopulation;
            CalculateFitness();
            Generation++;
        }

        // function that calculates the fitness sum for the generation and finds the best individual in terms of fitness
        private void CalculateFitness()
        {
            _fitnessSum = 0;
            var best = Population[0];
            foreach (var individual in Population)
            {
                _fitnessSum += individual.CalculateFitness();
                
                if (individual.Fitness > best.Fitness)
                {
                    best = individual;
                }
            }

            BestIndividual = new Individual(best);
        }

        // function that chooses a parent for the crossover based on fitness
        private Individual ChooseParent()
        {
            var randomNumber = _random.NextDouble() * _fitnessSum;
            foreach (var individual in Population)
            {
                if (randomNumber < individual.Fitness)
                {
                    return individual;
                }

                randomNumber -= individual.Fitness;
            }

            return null;
        }
    }
}