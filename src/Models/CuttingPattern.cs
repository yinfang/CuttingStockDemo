using System;
using System.Collections.Generic;
using System.Linq;

namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Represents a cutting pattern solution
    /// </summary>
    public class CuttingPattern
    {
        /// <summary>
        /// The stock material used in this pattern
        /// </summary>
        public StockMaterial Stock { get; set; }

        /// <summary>
        /// Collection of items cut from this stock
        /// </summary>
        public List<CuttingItem> CutItems { get; set; } = new List<CuttingItem>();

        /// <summary>
        /// The waste length for this pattern
        /// </summary>
        public double Waste { get; set; }

        /// <summary>
        /// The number of times this pattern is used
        /// </summary>
        public int PatternCount { get; set; }

        public CuttingPattern(StockMaterial stock)
        {
            Stock = stock;
            CutItems = new List<CuttingItem>();
        }

        public void AddItem(CuttingItem item)
        {
            CutItems.Add(item);
        }

        public override string ToString()
        {
            return $"Stock: {Stock?.Length ?? 0}, Items: {CutItems.Count}, Waste: {Waste}";
        }
    }
}