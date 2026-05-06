using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CuttingStockProblem.Models;
using ClosedXML.Excel;

namespace CuttingStockProblem.Services
{
    /// <summary>
    /// Service for exporting cutting stock solutions to Excel
    /// </summary>
    public class ExcelExportService
    {
        /// <summary>
        /// Export solution result to Excel file
        /// </summary>
        /// <param name="solution">Solution to export</param>
        /// <param name="filePath">Output file path</param>
        public void ExportSolutionToExcel(SolutionResult solution, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cutting Solution");

                // Add header
                worksheet.Cell(1, 1).Value = "Cutting Stock Problem Solution";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;

                // Add summary information
                worksheet.Cell(3, 1).Value = "Generated on:";
                worksheet.Cell(3, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                worksheet.Cell(4, 1).Value = "Total Patterns:";
                worksheet.Cell(4, 2).Value = solution.Patterns.Count;

                worksheet.Cell(5, 1).Value = "Total Stocks Used:";
                worksheet.Cell(5, 2).Value = solution.TotalStocksUsed;

                worksheet.Cell(6, 1).Value = "Total Waste:";
                worksheet.Cell(6, 2).Value = solution.TotalWaste;

                worksheet.Cell(7, 1).Value = "Total Cost:";
                worksheet.Cell(7, 2).Value = solution.TotalCost;

                worksheet.Cell(8, 1).Value = "Efficiency:";
                worksheet.Cell(8, 2).Value = solution.Efficiency;

                // Add patterns grouped by specification
                int row = 10;
                foreach (var group in solution.SpecificationGroups)
                {
                    // Spec group header row
                    worksheet.Cell(row, 1).Value = $"规格: {group.Specification}";
                    worksheet.Cell(row, 2).Value = $"总数量: {group.TotalCutCount}";
                    worksheet.Cell(row, 3).Value = $"原材料长度: {group.StockLength}";
                    worksheet.Cell(row, 4).Value = $"使用根数: {group.StockBarsUsed}";
                    worksheet.Cell(row, 5).Value = $"余料总长: {group.TotalWaste:F1}";
                    var groupHeaderRange = worksheet.Range(row, 1, row, 5);
                    groupHeaderRange.Style.Font.Bold = true;
                    groupHeaderRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    row++;

                    // Column headers for this group
                    worksheet.Cell(row, 1).Value = "Pattern #";
                    worksheet.Cell(row, 2).Value = "Stock Length";
                    worksheet.Cell(row, 3).Value = "Items Count";
                    worksheet.Cell(row, 4).Value = "Waste";
                    worksheet.Cell(row, 5).Value = "Pattern Count";
                    var colHeaderRange = worksheet.Range(row, 1, row, 5);
                    colHeaderRange.Style.Font.Bold = true;
                    colHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    row++;

                    // Pattern rows
                    foreach (var pattern in group.Patterns)
                    {
                        worksheet.Cell(row, 1).Value = pattern.PatternNumber;
                        worksheet.Cell(row, 2).Value = pattern.StockLength;
                        worksheet.Cell(row, 3).Value = pattern.ItemsCount;
                        worksheet.Cell(row, 4).Value = pattern.Waste;
                        worksheet.Cell(row, 5).Value = pattern.PatternCount;
                        row++;
                    }

                    // Blank row between groups
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }
    }
}