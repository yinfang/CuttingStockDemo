using System;
using System.Collections.Generic;
using System.Linq;

namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Represents an item that needs to be cut
    /// </summary>
    public class CuttingItem
    {
        /// <summary>
        /// The required length of the item
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// The required quantity of this item
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The demand for this item
        /// </summary>
        public int Demand { get; set; }

        public CuttingItem(double length, int quantity, int demand = 1)
        {
            Length = length;
            Quantity = quantity;
            Demand = demand;
        }

        public override string ToString()
        {
            return $"Length: {Length}, Quantity: {Quantity}, Demand: {Demand}";
        }
    }
}