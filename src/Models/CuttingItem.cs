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
        /// The specification this item requires (must match stock material spec)
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// The required length of the item in mm
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

        public CuttingItem(string specification, double length, int quantity, int demand = 1)
        {
            Specification = specification;
            Length = length;
            Quantity = quantity;
            Demand = demand;
        }

        public CuttingItem(double length, int quantity, int demand = 1)
        {
            Specification = "";
            Length = length;
            Quantity = quantity;
            Demand = demand;
        }

        public override string ToString()
        {
            return $"Spec: {Specification}, Length: {Length}, Quantity: {Quantity}";
        }
    }
}