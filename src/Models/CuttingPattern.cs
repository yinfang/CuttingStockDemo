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

        /// <summary>
        /// Gets the pattern number for display purposes
        /// </summary>
        public int PatternNumber { get; set; }

        /// <summary>
        /// Gets the specification for display purposes
        /// </summary>
        public string Specification => Stock?.Specification ?? "";

        /// <summary>
        /// Gets the stock length for display purposes
        /// </summary>
        public double StockLength => Stock?.Length ?? 0;

        /// <summary>
        /// Gets the items count for display purposes
        /// </summary>
        public int ItemsCount => CutItems?.Count ?? 0;

        /// <summary>
        /// Number of stock bars used in this pattern's specification group (原材料根数)
        /// Set by the algorithm after grouping.
        /// </summary>
        public int GroupStockBarsUsed { get; set; }

        /// <summary>
        /// Total waste across the specification group (余料总长)
        /// Set by the algorithm after grouping.
        /// </summary>
        public double GroupTotalWaste { get; set; }

        public CuttingPattern(StockMaterial stock)
        {
            PatternNumber = 1;
            Stock = stock;
            CutItems = new List<CuttingItem>();
        }

        public CuttingPattern(int patternNumber, StockMaterial stock)
        {
            PatternNumber = patternNumber;
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