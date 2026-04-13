using System;
using System.Collections.Generic;
using System.Linq;
using CuttingStockProblem.Models;

namespace CuttingStockProblem.Algorithms
{
    /// <summary>
    /// Genetic Algorithm implementation for one-dimensional cutting stock problem
    /// </summary>
    public class GeneticAlgorithm
    {
        // Algorithm parameters
        private readonly int _populationSize;
        private readonly int _maxGenerations;
        private readonly double _crossoverRate;
        private readonly double _mutationRate;
        private readonly Random _random;

        // Problem data
        private List<StockMaterial> _stockMaterials;
        private List<CuttingItem> _cuttingItems;
        private double _trimLoss;

        public GeneticAlgorithm(
            int populationSize = 100,
            int maxGenerations = 500,
            double crossoverRate = 0.8,
            double mutationRate = 0.1)
        {
            _populationSize = populationSize;
            _maxGenerations = maxGenerations;
            _crossoverRate = crossoverRate;
            _mutationRate = mutationRate;
            _random = new Random();
        }

        /// <summary>
        /// Solve the cutting stock problem using genetic algorithm
        /// </summary>
        /// <param name="stockMaterials">Available stock materials</param>
        /// <param name="cuttingItems">Items to be cut</param>
        /// <param name="trimLoss">Trim loss value</param>
        /// <returns>Optimal cutting solution</returns>
        public SolutionResult Solve(
            List<StockMaterial> stockMaterials,
            List<CuttingItem> cuttingItems,
            double trimLoss = 0.1)
        {
            _stockMaterials = stockMaterials;
            _cuttingItems = cuttingItems;
            _trimLoss = trimLoss;

            // Initialize population
            var population = InitializePopulation();

            // Evolution loop
            for (int generation = 0; generation < _maxGenerations; generation++)
            {
                // Evaluate fitness for each individual
                var fitnessScores = population.Select(individual => CalculateFitness(individual)).ToList();

                // Selection
                var selectedParents = Selection(population, fitnessScores);

                // Crossover
                var offspring = Crossover(selectedParents);

                // Mutation
                Mutation(offspring);

                // Create new population
                population = CreateNewPopulation(selectedParents, offspring);
            }

            // Return best solution
            return GetBestSolution(population);
        }

        /// <summary>
        /// Initialize the population with random solutions
        /// </summary>
        private List<List<CuttingPattern>> InitializePopulation()
        {
            var population = new List<List<CuttingPattern>>();

            for (int i = 0; i < _populationSize; i++)
            {
                var individual = GenerateRandomSolution();
                population.Add(individual);
            }

            return population;
        }

        /// <summary>
        /// Generate a random solution for the cutting stock problem
        /// </summary>
        private List<CuttingPattern> GenerateRandomSolution()
        {
            var solution = new List<CuttingPattern>();
            var remainingItems = new List<CuttingItem>(_cuttingItems);

            // Simple first-fit decreasing algorithm for initial solution
            var sortedItems = _cuttingItems.OrderByDescending(item => item.Length).ToList();

            foreach (var item in sortedItems)
            {
                // Try to place item in existing patterns
                bool placed = false;
                foreach (var pattern in solution)
                {
                    // Check if item fits in current pattern
                    if (CanFitInPattern(pattern, item))
                    {
                        pattern.AddItem(new CuttingItem(item.Length, item.Quantity));
                        placed = true;
                        break;
                    }
                }

                // If item doesn't fit in any existing pattern, create new pattern
                if (!placed && _stockMaterials.Any())
                {
                    var stock = _stockMaterials.First();
                    var newPattern = new CuttingPattern(stock);
                    newPattern.AddItem(new CuttingItem(item.Length, item.Quantity));
                    solution.Add(newPattern);
                }
            }

            return solution;
        }

        /// <summary>
        /// Check if an item can fit in a pattern
        /// </summary>
        private bool CanFitInPattern(CuttingPattern pattern, CuttingItem item)
        {
            double usedLength = pattern.CutItems.Sum(i => i.Length * i.Quantity) + item.Length * item.Quantity;
            return usedLength <= pattern.Stock.Length - _trimLoss;
        }

        /// <summary>
        /// Calculate fitness of an individual solution
        /// </summary>
        private double CalculateFitness(List<CuttingPattern> individual)
        {
            // Fitness function: minimize waste and number of stocks used
            double totalWaste = individual.Sum(pattern => pattern.Waste);
            int stocksUsed = individual.Count;

            // Multi-objective fitness function
            // Lower fitness value is better (minimize waste and stocks used)
            double fitness = totalWaste + (stocksUsed * 1000); // Weight stocks used more heavily

            return 1.0 / (1.0 + fitness); // Convert to maximization problem
        }

        /// <summary>
        /// Selection mechanism to choose parents for reproduction
        /// </summary>
        private List<List<CuttingPattern>> Selection(List<List<CuttingPattern>> population, List<double> fitnessScores)
        {
            var selected = new List<List<CuttingPattern>>();
            var totalFitness = fitnessScores.Sum();

            // Roulette wheel selection
            for (int i = 0; i < _populationSize / 2; i++)
            {
                double pick = _random.NextDouble() * totalFitness;
                double current = 0;

                for (int j = 0; j < population.Count; j++)
                {
                    current += fitnessScores[j];
                    if (current >= pick)
                    {
                        selected.Add(population[j]);
                        break;
                    }
                }
            }

            return selected;
        }

        /// <summary>
        /// Crossover operation to create offspring
        /// </summary>
        private List<List<CuttingPattern>> Crossover(List<List<CuttingPattern>> parents)
        {
            var offspring = new List<List<CuttingPattern>>();

            for (int i = 0; i < parents.Count - 1; i += 2)
            {
                if (_random.NextDouble() < _crossoverRate)
                {
                    var child1 = new List<CuttingPattern>();
                    var child2 = new List<CuttingPattern>();

                    // One-point crossover
                    if (parents[i].Count > 1 && parents[i + 1].Count > 1)
                    {
                        int crossoverPoint = _random.Next(1, Math.Min(parents[i].Count, parents[i + 1].Count));

                        // Copy first part from parent1
                        for (int j = 0; j < crossoverPoint; j++)
                        {
                            if (j < parents[i].Count)
                                child1.Add(parents[i][j]);
                        }

                        // Copy second part from parent2
                        for (int j = crossoverPoint; j < parents[i + 1].Count; j++)
                        {
                            child2.Add(parents[i + 1][j]);
                        }

                        offspring.Add(child1);
                        offspring.Add(child2);
                    }
                    else
                    {
                        // If crossover not possible, copy parents
                        offspring.Add(new List<CuttingPattern>(parents[i]));
                        if (i + 1 < parents.Count)
                            offspring.Add(new List<CuttingPattern>(parents[i + 1]));
                    }
                }
                else
                {
                    // No crossover, copy parents
                    offspring.Add(new List<CuttingPattern>(parents[i]));
                    if (i + 1 < parents.Count)
                        offspring.Add(new List<CuttingPattern>(parents[i + 1]));
                }
            }

            return offspring;
        }

        /// <summary>
        /// Mutation operation to introduce diversity
        /// </summary>
        private void Mutation(List<List<CuttingPattern>> population)
        {
            foreach (var individual in population)
            {
                if (_random.NextDouble() < _mutationRate)
                {
                    // Simple mutation: randomly swap two patterns
                    if (individual.Count > 1)
                    {
                        int index1 = _random.Next(individual.Count);
                        int index2 = _random.Next(individual.Count);

                        var temp = individual[index1];
                        individual[index1] = individual[index2];
                        individual[index2] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Create new population from parents and offspring
        /// </summary>
        private List<List<CuttingPattern>> CreateNewPopulation(
            List<List<CuttingPattern>> parents,
            List<List<CuttingPattern>> offspring)
        {
            var newPopulation = new List<List<CuttingPattern>>();

            // Keep best parents (elitism)
            var sortedParents = parents
                .Select((individual, index) => new { Individual = individual, Index = index })
                .OrderByDescending(x => CalculateFitness(x.Individual))
                .ToList();

            int eliteCount = _populationSize / 10; // Keep top 10%
            for (int i = 0; i < Math.Min(eliteCount, sortedParents.Count); i++)
            {
                newPopulation.Add(sortedParents[i].Individual);
            }

            // Fill remaining population with offspring
            int remaining = _populationSize - newPopulation.Count;
            for (int i = 0; i < Math.Min(remaining, offspring.Count); i++)
            {
                newPopulation.Add(offspring[i]);
            }

            // If we still need more individuals, create random ones
            while (newPopulation.Count < _populationSize)
            {
                newPopulation.Add(GenerateRandomSolution());
            }

            return newPopulation;
        }

        /// <summary>
        /// Get the best solution from the final population
        /// </summary>
        private SolutionResult GetBestSolution(List<List<CuttingPattern>> population)
        {
            var bestIndividual = population
                .OrderByDescending(individual => CalculateFitness(individual))
                .First();

            var result = new SolutionResult();
            result.Patterns = bestIndividual;

            // Calculate solution metrics
            result.TotalWaste = bestIndividual.Sum(p => p.Waste);
            result.TotalStocksUsed = bestIndividual.Count;
            result.TotalCost = bestIndividual.Sum(p => p.Stock.Cost * p.PatternCount);

            if (result.TotalStocksUsed > 0)
            {
                result.Efficiency = (1.0 - (result.TotalWaste / (result.TotalStocksUsed * _stockMaterials.First().Length))) * 100;
            }

            return result;
        }
    }
}