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
                worksheet.Cell(8, 2).Value = $"{solution.Efficiency}%";

                // Add patterns table header
                worksheet.Cell(10, 1).Value = "Pattern #";
                worksheet.Cell(10, 2).Value = "Stock Length";
                worksheet.Cell(10, 3).Value = "Items Count";
                worksheet.Cell(10, 4).Value = "Waste";
                worksheet.Cell(10, 5).Value = "Pattern Count";

                // Style the header
                var headerRange = worksheet.Range(10, 1, 10, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add pattern data
                int row = 11;
                for (int i = 0; i < solution.Patterns.Count; i++)
                {
                    var pattern = solution.Patterns[i];
                    worksheet.Cell(row + i, 1).Value = i + 1;
                    worksheet.Cell(row + i, 2).Value = pattern.Stock?.Length ?? 0;
                    worksheet.Cell(row + i, 3).Value = pattern.CutItems.Count;
                    worksheet.Cell(row + i, 4).Value = pattern.Waste;
                    worksheet.Cell(row + i, 5).Value = pattern.PatternCount;
                }

                // Add cutting items details
                int detailRow = row + solution.Patterns.Count + 2;
                worksheet.Cell(detailRow, 1).Value = "Cutting Items Details";
                worksheet.Cell(detailRow, 1).Style.Font.Bold = true;

                detailRow += 2;
                worksheet.Cell(detailRow, 1).Value = "Pattern #";
                worksheet.Cell(detailRow, 2).Value = "Item Length";
                worksheet.Cell(detailRow, 3).Value = "Item Quantity";
                worksheet.Cell(detailRow, 4).Value = "Waste";

                // Style the detail header
                var detailHeaderRange = worksheet.Range(detailRow, 1, detailRow, 4);
                detailHeaderRange.Style.Font.Bold = true;
                detailHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                detailRow++;
                // Add cutting items for each pattern
                for (int i = 0; i < solution.Patterns.Count; i++)
                {
                    var pattern = solution.Patterns[i];
                    worksheet.Cell(detailRow, 1).Value = $"Pattern {i + 1}";

                    for (int j = 0; j < pattern.CutItems.Count; j++)
                    {
                        var item = pattern.CutItems[j];
                        if (j == 0)
                        {
                            worksheet.Cell(detailRow + j, 2).Value = item.Length;
                            worksheet.Cell(detailRow + j, 3).Value = item.Quantity;
                        }
                        else
                        {
                            worksheet.Cell(detailRow + j, 1).Value = "";
                            worksheet.Cell(detailRow + j, 2).Value = item.Length;
                            worksheet.Cell(detailRow + j, 3).Value = item.Quantity;
                        }
                    }

                    detailRow += pattern.CutItems.Count;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Export cutting items to Excel file
        /// </summary>
        /// <param name="items">Items to export</param>
        /// <param name="filePath">Output file path</param>
        public void ExportItemsToExcel(List<CuttingItem> items, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cutting Items");

                // Add header
                worksheet.Cell(1, 1).Value = "Cutting Items List";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;

                // Add table header
                worksheet.Cell(3, 1).Value = "Length";
                worksheet.Cell(3, 2).Value = "Quantity";
                worksheet.Cell(3, 3).Value = "Demand";

                // Style the header
                var headerRange = worksheet.Range(3, 1, 3, 3);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add data
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    worksheet.Cell(4 + i, 1).Value = item.Length;
                    worksheet.Cell(4 + i, 2).Value = item.Quantity;
                    worksheet.Cell(4 + i, 3).Value = item.Demand;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Export stock materials to Excel file
        /// </summary>
        /// <param name="stocks">Stock materials to export</param>
        /// <param name="filePath">Output file path</param>
        public void ExportStocksToExcel(List<StockMaterial> stocks, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stock Materials");

                // Add header
                worksheet.Cell(1, 1).Value = "Stock Materials List";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;

                // Add table header
                worksheet.Cell(3, 1).Value = "Length";
                worksheet.Cell(3, 2).Value = "Quantity";
                worksheet.Cell(3, 3).Value = "Cost";

                // Style the header
                var headerRange = worksheet.Range(3, 1, 3, 3);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add data
                for (int i = 0; i < stocks.Count; i++)
                {
                    var stock = stocks[i];
                    worksheet.Cell(4 + i, 1).Value = stock.Length;
                    worksheet.Cell(4 + i, 2).Value = stock.Quantity;
                    worksheet.Cell(4 + i, 3).Value = stock.Cost;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }
    }
}