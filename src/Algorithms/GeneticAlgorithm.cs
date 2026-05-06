using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CuttingStockProblem.Models;

namespace CuttingStockProblem.Algorithms
{
    /// <summary>
    /// Genetic Algorithm for one-dimensional cutting stock problem.
    /// Uses permutation-based encoding: each individual is an ordering of all
    /// expanded pieces. A deterministic decoder converts the ordering into a
    /// feasible cutting plan grouped by specification, guaranteeing that every
    /// piece from every specification is always included in the solution.
    /// </summary>
    public class GeneticAlgorithm
    {
        private readonly int _populationSize;
        private readonly int _maxGenerations;
        private readonly double _crossoverRate;
        private readonly double _mutationRate;
        private readonly Random _random;

        private List<StockMaterial> _stockMaterials = new();
        private List<CuttingItem> _cuttingItems = new();
        private double _trimLoss;

        // Expanded pieces (one entry per physical piece, qty=1)
        private List<CuttingItem> _allPieces = new();

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

        public SolutionResult Solve(
            List<StockMaterial> stockMaterials,
            List<CuttingItem> cuttingItems,
            double trimLoss = 0.1)
        {
            _stockMaterials = stockMaterials;
            _cuttingItems = cuttingItems;
            _trimLoss = trimLoss;

            // Expand every cutting item into individual pieces (qty=1 each)
            _allPieces = _cuttingItems
                .SelectMany(item => Enumerable.Range(0, item.Quantity)
                    .Select(_ => new CuttingItem(item.Specification, item.Length, 1)))
                .ToList();

            if (_allPieces.Count == 0)
                return new SolutionResult();

            // Each individual is a permutation of piece indices
            var population = new List<int[]>();
            for (int i = 0; i < _populationSize; i++)
                population.Add(RandomPermutation(_allPieces.Count));

            // Evolution loop
            for (int gen = 0; gen < _maxGenerations; gen++)
            {
                var fitness = population.Select(p => CalculateFitness(p)).ToList();
                var parents = TournamentSelection(population, fitness);
                var offspring = OrderCrossover(parents);
                SwapMutation(offspring);
                population = NextGeneration(population, fitness, offspring);
            }

            var bestPerm = population.OrderByDescending(p => CalculateFitness(p)).First();
            return BuildSolutionResult(Decode(bestPerm));
        }

        // ─── Permutation ───

        private int[] RandomPermutation(int n)
        {
            var perm = Enumerable.Range(0, n).ToArray();
            for (int i = n - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (perm[i], perm[j]) = (perm[j], perm[i]);
            }
            return perm;
        }

        // ─── Decoder ───

        private StockMaterial FindStockForSpec(string spec)
        {
            if (!string.IsNullOrEmpty(spec))
            {
                var match = _stockMaterials.FirstOrDefault(s => s.Specification == spec);
                if (match != null) return match;
            }
            return _stockMaterials.First();
        }

        private List<CuttingPattern> Decode(int[] permutation)
        {
            var patterns = new List<CuttingPattern>();

            foreach (int idx in permutation)
            {
                var piece = _allPieces[idx];
                var stock = FindStockForSpec(piece.Specification);
                bool placed = false;

                foreach (var pattern in patterns)
                {
                    if (pattern.Stock.Specification != stock.Specification)
                        continue;
                    double used = pattern.CutItems.Sum(i => i.Length);
                    if (used + piece.Length + _trimLoss <= pattern.Stock.Length)
                    {
                        pattern.AddItem(piece);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    var newPattern = new CuttingPattern(stock);
                    newPattern.AddItem(piece);
                    patterns.Add(newPattern);
                }
            }

            // Calculate waste
            foreach (var p in patterns)
            {
                double used = p.CutItems.Sum(i => i.Length);
                p.Waste = Math.Max(0, p.Stock.Length - used - _trimLoss);
            }

            return patterns;
        }

        // ─── Fitness ───

        private double CalculateFitness(int[] permutation)
        {
            var patterns = Decode(permutation);
            double totalWaste = patterns.Sum(p => p.Waste);
            int stocksUsed = patterns.Count;
            double totalCost = patterns.Sum(p => p.Stock.Cost);

            // Minimize waste and number of stocks
            double score = totalWaste + stocksUsed * 1000.0 + totalCost * 0.1;
            return 1.0 / (1.0 + score);
        }

        // ─── Selection (Tournament) ───

        private List<int[]> TournamentSelection(List<int[]> population, List<double> fitness)
        {
            var selected = new List<int[]>();
            int tournamentSize = 3;

            for (int i = 0; i < _populationSize / 2; i++)
            {
                int bestIdx = _random.Next(population.Count);
                for (int t = 1; t < tournamentSize; t++)
                {
                    int candidate = _random.Next(population.Count);
                    if (fitness[candidate] > fitness[bestIdx])
                        bestIdx = candidate;
                }
                selected.Add(population[bestIdx]);
            }

            return selected;
        }

        // ─── Crossover (Order Crossover - OX) ───

        private List<int[]> OrderCrossover(List<int[]> parents)
        {
            var offspring = new List<int[]>();
            int n = _allPieces.Count;

            for (int i = 0; i < parents.Count - 1; i += 2)
            {
                if (_random.NextDouble() < _crossoverRate)
                {
                    var p1 = parents[i];
                    var p2 = parents[i + 1];

                    int start = _random.Next(n);
                    int end = _random.Next(start, n);

                    offspring.Add(OxChild(p1, p2, start, end, n));
                    offspring.Add(OxChild(p2, p1, start, end, n));
                }
                else
                {
                    offspring.Add((int[])parents[i].Clone());
                    offspring.Add((int[])parents[i + 1].Clone());
                }
            }

            return offspring;
        }

        private int[] OxChild(int[] p1, int[] p2, int start, int end, int n)
        {
            var child = new int[n];
            Array.Fill(child, -1);
            var inSegment = new HashSet<int>();

            for (int i = start; i <= end; i++)
            {
                child[i] = p1[i];
                inSegment.Add(p1[i]);
            }

            int pos = (end + 1) % n;
            for (int i = 0; i < n; i++)
            {
                int gene = p2[(end + 1 + i) % n];
                if (!inSegment.Contains(gene))
                {
                    child[pos] = gene;
                    pos = (pos + 1) % n;
                }
            }

            return child;
        }

        // ─── Mutation (swap) ───

        private void SwapMutation(List<int[]> population)
        {
            int n = _allPieces.Count;
            foreach (var individual in population)
            {
                if (_random.NextDouble() < _mutationRate)
                {
                    int a = _random.Next(n);
                    int b = _random.Next(n);
                    (individual[a], individual[b]) = (individual[b], individual[a]);
                }
            }
        }

        // ─── Next generation (elitism) ───

        private List<int[]> NextGeneration(
            List<int[]> current, List<double> fitness, List<int[]> offspring)
        {
            var next = new List<int[]>();

            // Elitism: keep top 10%
            var elite = current
                .Select((ind, i) => (ind, fit: fitness[i]))
                .OrderByDescending(x => x.fit)
                .Take(Math.Max(1, _populationSize / 10))
                .Select(x => x.ind)
                .ToList();

            next.AddRange(elite);
            next.AddRange(offspring);

            // Fill remaining slots with random individuals
            while (next.Count < _populationSize)
                next.Add(RandomPermutation(_allPieces.Count));

            // Trim to population size
            if (next.Count > _populationSize)
                next = next.Take(_populationSize).ToList();

            return next;
        }

        // ─── Build result ───

        private SolutionResult BuildSolutionResult(List<CuttingPattern> patterns)
        {
            var result = new SolutionResult();

            // Number patterns sequentially
            for (int i = 0; i < patterns.Count; i++)
            {
                patterns[i].PatternNumber = i + 1;
                patterns[i].PatternCount = 1;
            }

            result.Patterns = new ObservableCollection<CuttingPattern>(patterns);
            result.TotalWaste = patterns.Sum(p => p.Waste);
            result.TotalStocksUsed = patterns.Count;
            result.TotalCost = patterns.Sum(p => p.Stock.Cost * p.PatternCount);

            if (result.TotalStocksUsed > 0)
            {
                double totalStockLength = patterns.Sum(p => p.Stock.Length);
                result.Efficiency = (1.0 - (result.TotalWaste / totalStockLength)) * 100;
            }

            // Group by cutting item specification only
            // First, collect all cutting items from all patterns
            var allCutItems = new List<string>();
            foreach (var pattern in patterns)
            {
                foreach (var item in pattern.CutItems)
                {
                    allCutItems.Add(item.Specification);
                }
            }

            // Group by specification
            var groupedItems = allCutItems
                .GroupBy(x => x)
                .Select(g => new
                {
                    Specification = g.Key,
                    TotalCutCount = g.Count()
                })
                .ToList();

            // For each unique specification, find patterns that contain items with that spec
            result.SpecificationGroups = new ObservableCollection<SpecificationGroup>();
            foreach (var group in groupedItems)
            {
                // Find patterns that contain items with this specification
                var matchingPatterns = patterns
                    .Where(p => p.CutItems.Any(i => i.Specification == group.Specification))
                    .ToList();

                if (matchingPatterns.Any())
                {
                    var specGroup = new SpecificationGroup
                    {
                        Specification = group.Specification,
                        TotalCutCount = group.TotalCutCount,
                        StockLength = matchingPatterns.First().StockLength,
                        StockBarsUsed = matchingPatterns.Count,
                        TotalWaste = matchingPatterns.Sum(p => p.Waste),
                        Patterns = new ObservableCollection<CuttingPattern>(matchingPatterns)
                    };
                    result.SpecificationGroups.Add(specGroup);
                }
            }

            // Set group-level stats on each pattern for display binding
            foreach (var group in result.SpecificationGroups)
            {
                foreach (var pattern in group.Patterns)
                {
                    pattern.GroupStockBarsUsed = group.StockBarsUsed;
                    pattern.GroupTotalWaste = group.TotalWaste;
                }
            }

            return result;
        }
    }
}
