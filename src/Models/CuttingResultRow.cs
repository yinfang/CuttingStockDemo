namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Represents a result row grouped by cutting item length,
    /// showing how many stock bars are used and the waste for each.
    /// </summary>
    public class CuttingResultRow
    {
        /// <summary>
        /// The cutting item length
        /// </summary>
        public double ItemLength { get; set; }

        /// <summary>
        /// Number of pieces cut at this length
        /// </summary>
        public int CutCount { get; set; }

        /// <summary>
        /// Number of stock bars (原材料根数) used for this item length
        /// </summary>
        public int StockBarsUsed { get; set; }

        /// <summary>
        /// The stock material length used
        /// </summary>
        public double StockLength { get; set; }

        /// <summary>
        /// Waste (余料) length for this group
        /// </summary>
        public double WasteLength { get; set; }

        /// <summary>
        /// Formatted display of waste
        /// </summary>
        public string WasteDisplay => $"{WasteLength:F1}";

        /// <summary>
        /// Formatted display of stock info
        /// </summary>
        public string StockDisplay => $"{StockLength} × {StockBarsUsed} 根";
    }
}
