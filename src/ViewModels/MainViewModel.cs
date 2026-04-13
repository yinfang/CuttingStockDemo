using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CuttingStockProblem.Commands;
using CuttingStockProblem.Models;
using CuttingStockProblem.Algorithms;
using CuttingStockProblem.Services;

namespace CuttingStockProblem.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<StockMaterial> _stockMaterials;
        private ObservableCollection<CuttingItem> _cuttingItems;
        private SolutionResult _solutionResult;
        private GeneticAlgorithm _algorithm;
        private ExcelExportService _exportService;

        public ObservableCollection<StockMaterial> StockMaterials
        {
            get { return _stockMaterials; }
            set
            {
                _stockMaterials = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CuttingItem> CuttingItems
        {
            get { return _cuttingItems; }
            set
            {
                _cuttingItems = value;
                OnPropertyChanged();
            }
        }

        public SolutionResult SolutionResult
        {
            get { return _solutionResult; }
            set
            {
                _solutionResult = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPatterns));
                OnPropertyChanged(nameof(TotalWaste));
                OnPropertyChanged(nameof(Efficiency));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        public int TotalPatterns => SolutionResult?.Patterns?.Count ?? 0;
        public double TotalWaste => SolutionResult?.TotalWaste ?? 0;
        public double Efficiency => SolutionResult?.Efficiency ?? 0;
        public double TotalCost => SolutionResult?.TotalCost ?? 0;

        public ICommand RunAlgorithmCommand { get; private set; }
        public ICommand ExportToExcelCommand { get; private set; }
        public ICommand AddStockCommand { get; private set; }
        public ICommand RemoveStockCommand { get; private set; }
        public ICommand AddItemCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            InitializeData();
            InitializeCommands();
        }

        private void InitializeData()
        {
            _stockMaterials = new ObservableCollection<StockMaterial>
            {
                new StockMaterial(100, 10, 10),
                new StockMaterial(200, 5, 15)
            };

            _cuttingItems = new ObservableCollection<CuttingItem>
            {
                new CuttingItem(20, 10),
                new CuttingItem(15, 5),
                new CuttingItem(30, 8)
            };

            _algorithm = new GeneticAlgorithm();
            _exportService = new ExcelExportService();

            _solutionResult = new SolutionResult();
        }

        private void InitializeCommands()
        {
            RunAlgorithmCommand = new RelayCommand(param => RunAlgorithm());
            ExportToExcelCommand = new RelayCommand(param => ExportToExcel(), param => SolutionResult != null);
            AddStockCommand = new RelayCommand(param => AddStock());
            RemoveStockCommand = new RelayCommand(param => RemoveStock());
            AddItemCommand = new RelayCommand(param => AddItem());
            RemoveItemCommand = new RelayCommand(param => RemoveItem());
        }

        private void RunAlgorithm()
        {
            try
            {
                var solution = _algorithm.Solve(
                    new List<StockMaterial>(StockMaterials),
                    new List<CuttingItem>(CuttingItems));

                SolutionResult = solution;
            }
            catch (Exception ex)
            {
                // Handle exception in UI
                System.Windows.MessageBox.Show($"Error running algorithm: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            try
            {
                if (SolutionResult != null)
                {
                    _exportService.ExportSolutionToExcel(SolutionResult, "solution.xlsx");
                    System.Windows.MessageBox.Show("Solution exported to Excel successfully!", "Export Complete",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Please run the algorithm first to generate a solution.", "No Solution",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Export Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void AddStock()
        {
            StockMaterials.Add(new StockMaterial(100, 1, 10));
        }

        private void RemoveStock()
        {
            if (StockMaterials.Count > 0)
            {
                StockMaterials.RemoveAt(StockMaterials.Count - 1);
            }
        }

        private void AddItem()
        {
            CuttingItems.Add(new CuttingItem(10, 1));
        }

        private void RemoveItem()
        {
            if (CuttingItems.Count > 0)
            {
                CuttingItems.RemoveAt(CuttingItems.Count - 1);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}