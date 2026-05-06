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
        private int _populationSize = 100;
        private int _maxGenerations = 500;
        private double _crossoverRate = 0.8;
        private double _mutationRate = 0.1;
        private double _trimLoss = 0.1;
        private ObservableCollection<StockMaterial>? _stockMaterials;
        private ObservableCollection<CuttingItem>? _cuttingItems;
        private SolutionResult? _solutionResult;
        private GeneticAlgorithm? _algorithm;
        private ExcelExportService? _exportService;
        private ICommand? _runAlgorithmCommand;
        private ICommand? _exportToExcelCommand;
        private ICommand? _addStockCommand;
        private ICommand? _removeStockCommand;
        private ICommand? _addItemCommand;
        private ICommand? _removeItemCommand;
        private ICommand? _newCommand;
        private ICommand? _openCommand;
        private ICommand? _saveCommand;
        private ICommand? _exitCommand;
        private ICommand? _settingsCommand;
        private ICommand? _algorithmSettingsCommand;
        private ICommand? _aboutCommand;
        private ICommand? _printCommand;
        private List<string> _testDataSetNames = new List<string>();
        private string? _selectedTestDataSet;

        public List<string> TestDataSetNames
        {
            get => _testDataSetNames;
            set
            {
                _testDataSetNames = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedTestDataSet
        {
            get => _selectedTestDataSet;
            set
            {
                _selectedTestDataSet = value;
                OnPropertyChanged();
                LoadTestData(value);
            }
        }

        public ObservableCollection<StockMaterial> StockMaterials
        {
            get { return _stockMaterials ??= new ObservableCollection<StockMaterial>(); }
            set
            {
                _stockMaterials = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CuttingItem> CuttingItems
        {
            get { return _cuttingItems ??= new ObservableCollection<CuttingItem>(); }
            set
            {
                _cuttingItems = value;
                OnPropertyChanged();
            }
        }

        public SolutionResult? SolutionResult
        {
            get { return _solutionResult ??= new SolutionResult(); }
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

        public ICommand RunAlgorithmCommand => _runAlgorithmCommand ??= new RelayCommand(param => RunAlgorithm());
        public ICommand ExportToExcelCommand => _exportToExcelCommand ??= new RelayCommand(param => ExportToExcel(), param => SolutionResult != null);
        public ICommand AddStockCommand => _addStockCommand ??= new RelayCommand(param => AddStock());
        public ICommand RemoveStockCommand => _removeStockCommand ??= new RelayCommand(param => RemoveStock());
        public ICommand AddItemCommand => _addItemCommand ??= new RelayCommand(param => AddItem());
        public ICommand RemoveItemCommand => _removeItemCommand ??= new RelayCommand(param => RemoveItem());
        public ICommand NewCommand => _newCommand ??= new RelayCommand(param => New());
        public ICommand OpenCommand => _openCommand ??= new RelayCommand(param => Open());
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(param => Save());
        public ICommand ExitCommand => _exitCommand ??= new RelayCommand(param => Exit());
        public ICommand SettingsCommand => _settingsCommand ??= new RelayCommand(param => ShowSettings());
        public ICommand AlgorithmSettingsCommand => _algorithmSettingsCommand ??= new RelayCommand(param => ShowAlgorithmSettings());
        public ICommand AboutCommand => _aboutCommand ??= new RelayCommand(param => ShowAbout());
        public ICommand PrintCommand => _printCommand ??= new RelayCommand(param => PrintSolution(), param => SolutionResult != null);

        public event PropertyChangedEventHandler? PropertyChanged;

        public int PopulationSize
        {
            get { return _populationSize; }
            set
            {
                _populationSize = value;
                OnPropertyChanged();
            }
        }

        public int MaxGenerations
        {
            get { return _maxGenerations; }
            set
            {
                _maxGenerations = value;
                OnPropertyChanged();
            }
        }

        public double CrossoverRate
        {
            get { return _crossoverRate; }
            set
            {
                _crossoverRate = value;
                OnPropertyChanged();
            }
        }

        public double MutationRate
        {
            get { return _mutationRate; }
            set
            {
                _mutationRate = value;
                OnPropertyChanged();
            }
        }

        public double TrimLoss
        {
            get { return _trimLoss; }
            set
            {
                _trimLoss = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            InitializeData();
            InitializeCommands();
        }

        private void InitializeData()
        {
            _stockMaterials = new ObservableCollection<StockMaterial>();
            _cuttingItems = new ObservableCollection<CuttingItem>();

            _testDataSetNames = new List<string>
            {
                "测试数据1 - 电缆规格",
                "测试数据2 - 多规格混合",
                "测试数据3 - 大批量需求"
            };

            _algorithm = new GeneticAlgorithm();
            _exportService = new ExcelExportService();

            _solutionResult = new SolutionResult();

            // Initialize algorithm parameters
            _populationSize = 100;
            _maxGenerations = 500;
            _crossoverRate = 0.8;
            _mutationRate = 0.1;
            _trimLoss = 0.1;

            // Load default test data
            SelectedTestDataSet = _testDataSetNames[0];
        }

        private void InitializeCommands()
        {
            _runAlgorithmCommand = new RelayCommand(param => RunAlgorithm());
            _exportToExcelCommand = new RelayCommand(param => ExportToExcel(), param => SolutionResult != null);
            _addStockCommand = new RelayCommand(param => AddStock());
            _removeStockCommand = new RelayCommand(param => RemoveStock());
            _addItemCommand = new RelayCommand(param => AddItem());
            _removeItemCommand = new RelayCommand(param => RemoveItem());
            _newCommand = new RelayCommand(param => New());
            _openCommand = new RelayCommand(param => Open());
            _saveCommand = new RelayCommand(param => Save());
            _exitCommand = new RelayCommand(param => Exit());
            _settingsCommand = new RelayCommand(param => ShowSettings());
            _algorithmSettingsCommand = new RelayCommand(param => ShowAlgorithmSettings());
            _aboutCommand = new RelayCommand(param => ShowAbout());
            _printCommand = new RelayCommand(param => PrintSolution(), param => SolutionResult != null);
        }

        private void LoadTestData(string? dataSetName)
        {
            if (dataSetName == null) return;

            _stockMaterials?.Clear();
            _cuttingItems?.Clear();

            switch (dataSetName)
            {
                case "测试数据1 - 电缆规格":
                    // Stock materials matching the image: spec, length 6000mm, qty 9999
                    _stockMaterials?.Add(new StockMaterial("20*3", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("20*4", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("20*6", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("25*8", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("30*4", 6000, 9999));
                    // Cutting items with matching specifications
                    _cuttingItems?.Add(new CuttingItem("20*3", 1500, 10));
                    _cuttingItems?.Add(new CuttingItem("20*3", 2000, 8));
                    _cuttingItems?.Add(new CuttingItem("20*4", 1200, 12));
                    _cuttingItems?.Add(new CuttingItem("20*4", 1800, 6));
                    _cuttingItems?.Add(new CuttingItem("20*6", 2500, 5));
                    _cuttingItems?.Add(new CuttingItem("25*8", 3000, 4));
                    _cuttingItems?.Add(new CuttingItem("30*4", 1000, 15));
                    break;

                case "测试数据2 - 多规格混合":
                    _stockMaterials?.Add(new StockMaterial("20*3", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("25*4", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("30*6", 6000, 9999));
                    _cuttingItems?.Add(new CuttingItem("20*3", 800, 20));
                    _cuttingItems?.Add(new CuttingItem("20*3", 1200, 15));
                    _cuttingItems?.Add(new CuttingItem("25*4", 1500, 10));
                    _cuttingItems?.Add(new CuttingItem("25*4", 2000, 8));
                    _cuttingItems?.Add(new CuttingItem("30*6", 2500, 6));
                    _cuttingItems?.Add(new CuttingItem("30*6", 3000, 4));
                    break;

                case "测试数据3 - 大批量需求":
                    _stockMaterials?.Add(new StockMaterial("20*3", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("20*4", 6000, 9999));
                    _stockMaterials?.Add(new StockMaterial("25*8", 6000, 9999));
                    _cuttingItems?.Add(new CuttingItem("20*3", 500, 50));
                    _cuttingItems?.Add(new CuttingItem("20*3", 1000, 30));
                    _cuttingItems?.Add(new CuttingItem("20*3", 1500, 25));
                    _cuttingItems?.Add(new CuttingItem("20*4", 800, 40));
                    _cuttingItems?.Add(new CuttingItem("20*4", 2000, 20));
                    _cuttingItems?.Add(new CuttingItem("25*8", 1200, 35));
                    _cuttingItems?.Add(new CuttingItem("25*8", 3000, 10));
                    break;
            }
        }

        private void RunAlgorithm()
        {
            try
            {
                if (_algorithm != null && _stockMaterials != null && _cuttingItems != null)
                {
                    // Create a new genetic algorithm with the current parameters
                    var algorithm = new GeneticAlgorithm(PopulationSize, MaxGenerations, CrossoverRate, MutationRate);
                    var solution = algorithm.Solve(
                        new List<StockMaterial>(StockMaterials),
                        new List<CuttingItem>(CuttingItems),
                        TrimLoss);

                    SolutionResult = solution;
                }
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
                if (SolutionResult != null && _exportService != null)
                {
                    // Create save file dialog
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                        DefaultExt = "xlsx",
                        FileName = $"solution_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        Title = "导出解决方案到Excel"
                    };

                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        _exportService.ExportSolutionToExcel(SolutionResult, filePath);
                        System.Windows.MessageBox.Show($"解决方案已成功导出到:\n{filePath}", "导出完成",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("请先运行算法生成解决方案。", "无解决方案",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"导出Excel时出错: {ex.Message}", "导出错误",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void AddStock()
        {
            if (_stockMaterials != null)
            {
                _stockMaterials.Add(new StockMaterial("20*3", 6000, 9999));
            }
        }

        private void RemoveStock()
        {
            if (_stockMaterials != null && _stockMaterials.Count > 0)
            {
                _stockMaterials.RemoveAt(_stockMaterials.Count - 1);
            }
        }

        private void AddItem()
        {
            if (_cuttingItems != null)
            {
                _cuttingItems.Add(new CuttingItem(10, 1));
            }
        }

        private void RemoveItem()
        {
            if (_cuttingItems != null && _cuttingItems.Count > 0)
            {
                _cuttingItems.RemoveAt(_cuttingItems.Count - 1);
            }
        }

        private void New()
        {
            // Reset the application to initial state
            _stockMaterials?.Clear();
            _cuttingItems?.Clear();
            _solutionResult = new SolutionResult();
        }

        private void Open()
        {
            // Open file dialog
            System.Windows.MessageBox.Show("打开文件功能");
        }

        private void Save()
        {
            // Save file dialog
            System.Windows.MessageBox.Show("保存文件功能");
        }

        private void Exit()
        {
            // Exit application
            System.Windows.Application.Current.Shutdown();
        }

        private void ShowSettings()
        {
            // Show settings
            System.Windows.MessageBox.Show("显示设置");
        }

        private void ShowAlgorithmSettings()
        {
            // Show algorithm settings
            System.Windows.MessageBox.Show("显示算法设置");
        }

        private void ShowAbout()
        {
            // Show about dialog
            System.Windows.MessageBox.Show("关于对话框");
        }

        private void PrintSolution()
        {
            // Print solution
            System.Windows.MessageBox.Show("打印解决方案");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}