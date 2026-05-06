using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using CuttingStockProblem.Algorithms;
using CuttingStockProblem.Models;
using CuttingStockProblem.ViewModels;

namespace CuttingStockProblem.Tests
{
    public class GeneticAlgorithmTests
    {
        [Fact]
        public void GeneticAlgorithm_Should_Create_Instance()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm();

            // Assert
            Assert.NotNull(algorithm);
        }

        [Fact]
        public void GeneticAlgorithm_Should_Solve_Simple_Problem()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm();
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial(100, 10)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem(20, 10),
                new CuttingItem(15, 5)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            Assert.NotNull(solution);
            Assert.NotNull(solution.Patterns);
        }

        [Fact]
        public void CuttingItem_Should_Create_Instance()
        {
            // Arrange & Act
            var item = new CuttingItem("20*3", 1500, 10);

            // Assert
            Assert.Equal("20*3", item.Specification);
            Assert.Equal(1500, item.Length);
            Assert.Equal(10, item.Quantity);
        }

        [Fact]
        public void CuttingItem_Without_Spec_Should_Default_Empty()
        {
            // Arrange & Act
            var item = new CuttingItem(20, 5);

            // Assert
            Assert.Equal("", item.Specification);
            Assert.Equal(20, item.Length);
            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public void StockMaterial_Should_Create_Instance()
        {
            // Arrange & Act
            var stock = new StockMaterial("20*3", 6000, 9999);

            // Assert
            Assert.Equal("20*3", stock.Specification);
            Assert.Equal(6000, stock.Length);
            Assert.Equal(9999, stock.Quantity);
        }

        [Fact]
        public void StockMaterial_Without_Spec_Should_Default_Empty()
        {
            // Arrange & Act
            var stock = new StockMaterial(100, 5, 10);

            // Assert
            Assert.Equal("", stock.Specification);
            Assert.Equal(100, stock.Length);
            Assert.Equal(5, stock.Quantity);
            Assert.Equal(10, stock.Cost);
        }

        [Fact]
        public void GeneticAlgorithm_Should_Handle_Complex_Problem()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm();
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial(100, 10, 10),
                new StockMaterial(200, 5, 15)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem(20, 10),
                new CuttingItem(15, 5),
                new CuttingItem(30, 8),
                new CuttingItem(25, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            Assert.NotNull(solution);
            Assert.NotNull(solution.Patterns);
            Assert.True(solution.Patterns.Count > 0);
        }

        [Fact]
        public void GeneticAlgorithm_Should_Handle_Empty_Problem()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm();
            var stockMaterials = new List<StockMaterial>();
            var cuttingItems = new List<CuttingItem>();

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            Assert.NotNull(solution);
        }

        [Fact]
        public void CuttingPattern_Should_Create_Instance()
        {
            // Arrange & Act
            var stock = new StockMaterial("20*3", 6000, 9999);
            var pattern = new CuttingPattern(stock);

            // Assert
            Assert.NotNull(pattern);
            Assert.Equal(stock, pattern.Stock);
            Assert.Equal("20*3", pattern.Specification);
            Assert.Equal(6000, pattern.StockLength);
            Assert.Empty(pattern.CutItems);
        }

        [Fact]
        public void GeneticAlgorithm_Should_Solve_With_Specifications()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm(populationSize: 20, maxGenerations: 10);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            Assert.NotNull(solution);
            Assert.NotNull(solution.Patterns);
            Assert.True(solution.Patterns.Count > 0);
            // Each pattern should only contain items matching its stock spec
            foreach (var pattern in solution.Patterns)
            {
                foreach (var item in pattern.CutItems)
                {
                    Assert.Equal(pattern.Stock.Specification, item.Specification);
                }
            }
        }

        [Fact]
        public void MainViewModel_Should_Create_Instance()
        {
            // Act
            var viewModel = new ViewModels.MainViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void GeneticAlgorithm_Should_Populate_SpecificationGroups()
        {
            // Arrange — use larger population/generations for reliable convergence
            var algorithm = new GeneticAlgorithm(populationSize: 50, maxGenerations: 50);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            Assert.NotNull(solution.SpecificationGroups);
            Assert.True(solution.SpecificationGroups.Count >= 1);
            // Each group's patterns must match the group's specification
            foreach (var group in solution.SpecificationGroups)
            {
                Assert.False(string.IsNullOrEmpty(group.Specification));
                foreach (var pattern in group.Patterns)
                {
                    Assert.Equal(group.Specification, pattern.Specification);
                }
            }
        }

        [Fact]
        public void SpecificationGroup_StockBarsUsed_Equals_PatternCount()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm(populationSize: 20, maxGenerations: 10);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*3", 2000, 8),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            foreach (var group in solution.SpecificationGroups)
            {
                Assert.Equal(group.Patterns.Count, group.StockBarsUsed);
            }
        }

        [Fact]
        public void SpecificationGroup_TotalWaste_Equals_SumOfPatternWastes()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm(populationSize: 20, maxGenerations: 10);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            foreach (var group in solution.SpecificationGroups)
            {
                var expectedWaste = group.Patterns.Sum(p => p.Waste);
                Assert.Equal(expectedWaste, group.TotalWaste, 2);
            }
        }

        [Fact]
        public void SpecificationGroup_Contains_Only_Matching_Spec_Patterns()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm(populationSize: 20, maxGenerations: 10);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert
            foreach (var group in solution.SpecificationGroups)
            {
                foreach (var pattern in group.Patterns)
                {
                    Assert.Equal(group.Specification, pattern.Specification);
                }
            }
        }

        [Fact]
        public void Patterns_Should_Have_GroupStockBarsUsed_And_GroupTotalWaste()
        {
            // Arrange
            var algorithm = new GeneticAlgorithm(populationSize: 50, maxGenerations: 50);
            var stockMaterials = new List<StockMaterial>
            {
                new StockMaterial("20*3", 6000, 9999),
                new StockMaterial("20*4", 6000, 9999)
            };

            var cuttingItems = new List<CuttingItem>
            {
                new CuttingItem("20*3", 1500, 10),
                new CuttingItem("20*4", 1200, 12)
            };

            // Act
            var solution = algorithm.Solve(stockMaterials, cuttingItems);

            // Assert — every pattern should have group-level properties set
            foreach (var pattern in solution.Patterns)
            {
                Assert.True(pattern.GroupStockBarsUsed > 0,
                    $"Pattern {pattern.PatternNumber} spec={pattern.Specification} should have GroupStockBarsUsed > 0");
                Assert.True(pattern.GroupTotalWaste >= 0,
                    $"Pattern {pattern.PatternNumber} spec={pattern.Specification} should have GroupTotalWaste >= 0");
            }

            // Verify consistency: patterns in the same spec group share the same values
            foreach (var group in solution.SpecificationGroups)
            {
                foreach (var pattern in group.Patterns)
                {
                    Assert.Equal(group.StockBarsUsed, pattern.GroupStockBarsUsed);
                    Assert.Equal(group.TotalWaste, pattern.GroupTotalWaste, 2);
                }
            }
        }
    }
}