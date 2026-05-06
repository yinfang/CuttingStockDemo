using System.Collections.ObjectModel;

namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Groups cutting patterns by cutting item specification,
    /// providing per-spec summary statistics.
    /// </summary>
    public class SpecificationGroup
    {
        /// <summary>
        /// The cutting item specification name (e.g. "20*3")
        /// </summary>
        public string Specification { get; set; } = "";

        /// <summary>
        /// Total number of pieces cut at this specification
        /// </summary>
        public int TotalCutCount { get; set; }

        /// <summary>
        /// The stock material length used for this specification
        /// </summary>
        public double StockLength { get; set; }

        /// <summary>
        /// Number of stock bars used for this specification (原材料根数)
        /// </summary>
        public int StockBarsUsed { get; set; }

        /// <summary>
        /// Total waste/remainder length for this specification (余料长度)
        /// </summary>
        public double TotalWaste { get; set; }

        /// <summary>
        /// The cutting patterns belonging to this specification group
        /// </summary>
        public ObservableCollection<CuttingPattern> Patterns { get; set; } = new ObservableCollection<CuttingPattern>();

        /// <summary>
        /// Formatted header for display
        /// </summary>
        public string HeaderDisplay => $"规格: {Specification}  |  总数量: {TotalCutCount}  |  原材料长度: {StockLength}  |  使用根数: {StockBarsUsed}  |  余料总长: {TotalWaste:F1}";
    }
}
