using System;
using System.Collections.Generic;
using System.Linq;

namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Represents stock material that will be cut
    /// </summary>
    public class StockMaterial
    {
        /// <summary>
        /// The length of the stock material
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// The quantity of this stock material available
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The cost of this stock material
        /// </summary>
        public double Cost { get; set; }

        public StockMaterial(double length, int quantity, double cost = 0)
        {
            Length = length;
            Quantity = quantity;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"Length: {Length}, Quantity: {Quantity}, Cost: {Cost}";
        }
    }
}