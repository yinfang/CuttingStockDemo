using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CuttingStockProblem.Models
{
    /// <summary>
    /// Represents the final cutting solution result
    /// </summary>
    public class SolutionResult
    {
        /// <summary>
        /// Collection of cutting patterns in the solution
        /// </summary>
        public ObservableCollection<CuttingPattern> Patterns { get; set; }

        /// <summary>
        /// Patterns grouped by specification, with per-spec summary statistics
        /// </summary>
        public ObservableCollection<SpecificationGroup> SpecificationGroups { get; set; }

        /// <summary>
        /// Total waste across all patterns
        /// </summary>
        public double TotalWaste { get; set; }

        /// <summary>
        /// Total number of stock materials used
        /// </summary>
        public int TotalStocksUsed { get; set; }

        /// <summary>
        /// Cost of the solution
        /// </summary>
        public double TotalCost { get; set; }

        /// <summary>
        /// Efficiency of the solution
        /// </summary>
        public double Efficiency { get; set; }

        public SolutionResult()
        {
            Patterns = new ObservableCollection<CuttingPattern>();
            SpecificationGroups = new ObservableCollection<SpecificationGroup>();
        }

        public void AddPattern(CuttingPattern pattern)
        {
            Patterns.Add(pattern);
        }

        public override string ToString()
        {
            return $"Patterns: {Patterns.Count}, Waste: {TotalWaste}, Cost: {TotalCost}, Efficiency: {Efficiency}%";
        }
    }
}