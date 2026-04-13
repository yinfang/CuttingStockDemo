using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using CuttingStockProblem.Algorithms;
using CuttingStockProblem.Models;

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
            var item = new CuttingItem(20, 5);

            // Assert
            Assert.Equal(20, item.Length);
            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public void StockMaterial_Should_Create_Instance()
        {
            // Arrange & Act
            var stock = new StockMaterial(100, 5, 10);

            // Assert
            Assert.Equal(100, stock.Length);
            Assert.Equal(5, stock.Quantity);
            Assert.Equal(10, stock.Cost);
        }
    }
}