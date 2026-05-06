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
        /// The specification of the stock material (e.g., "20*3", "25*8")
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// The length of the stock material in mm
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

        public StockMaterial(string specification, double length, int quantity, double cost = 0)
        {
            Specification = specification;
            Length = length;
            Quantity = quantity;
            Cost = cost;
        }

        public StockMaterial(double length, int quantity, double cost = 0)
        {
            Specification = "";
            Length = length;
            Quantity = quantity;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"Spec: {Specification}, Length: {Length}, Quantity: {Quantity}, Cost: {Cost}";
        }
    }
}