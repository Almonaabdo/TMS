// ViewModel class
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    /// <summary>
    /// ViewModel for the admin functionalities.
    /// </summary>
    public sealed class AdminViewModel : INotifyPropertyChanged
    {
        // Commands
        public ICommand BackUpDbCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand OpenSelectedFileCommand { get; }
        public ICommand OpenSelectedBackupCommand { get; }
        public ICommand CreateNewCarrierCommand { get; }

        // Variables
        private readonly AdminModel _adminModel;
        private readonly CarrierModel _carrierModel;
        private readonly TmsDbContext _dbContext;
        public ObservableCollection<string> LogFiles { get; set; } = null!;
        public ObservableCollection<string> BackupFiles { get; set; } = null!;
        private string _selectedLogFile = null!;
        private string _selectedBackupFile = null!;
        private int _backUpProgress;
        private Visibility _progressBarVisibility = Visibility.Collapsed;
        public ObservableCollection<Carrier>? CarrierData { get; private set; }

     
        /////////////////////////        Carrier Data        ///////////////////////////////////////////////////

        private string? _companyName;
        private int _ftla;
        private int _ltla;
        private double _ftlaRate;
        private double _ltlaRate;
        private double _reefCharge;

        public string? CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        public int Ftla
        {
            get => _ftla;
            set
            {
                _ftla = value;
               OnPropertyChanged(nameof(Ftla));
            }
        }

        public int Ltla
        {
            get => _ltla;
            set
            {
                _ltla = value;
               OnPropertyChanged(nameof(Ltla));
            }
        }

        public double FtlaRate
        {
            get => _ftlaRate;
            set
            {
                _ftlaRate = value;
                OnPropertyChanged(nameof(FtlaRate));
            }
        }

        public double LtlaRate
        {
            get => _ltlaRate;
            set
            {
                _ltlaRate = value;
                OnPropertyChanged(nameof(LtlaRate));
            }
        }

        public double ReefCharge
        {
            get => _reefCharge;
            set
            {
                _reefCharge = value;
               OnPropertyChanged(nameof(ReefCharge));
            }
        }

        /////////////////////////        Carrier Data        ///////////////////////////////////////////////////

        // Properties
        public string SelectedLogFile
        {
            get => _selectedLogFile;
            set
            {
                _selectedLogFile = value;
                OnPropertyChanged(nameof(SelectedLogFile));
            }
        }

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        public int BackUpProgress
        {
            get => _backUpProgress;
            set
            {
                _backUpProgress = value;
                OnPropertyChanged(nameof(BackUpProgress));
            }
        }

        public string SelectedBackupFile
        {
            get => _selectedBackupFile;
            set
            {
                _selectedBackupFile = value;
                OnPropertyChanged(nameof(SelectedBackupFile));
            }
        }

        /// <summary>
        /// Constructor to initialize necessary commands, methods, and variables.
        /// </summary>
        public AdminViewModel()
        {
            _dbContext = new TmsDbContext();
            _adminModel = new AdminModel(_dbContext);
            _carrierModel = new CarrierModel(_dbContext);
            // Commands
            BackUpDbCommand = new RelayCommand(BackUp);
            SaveChangesCommand = new RelayCommand(SaveCarrierChanges);
            OpenSelectedBackupCommand = new RelayCommand(OpenSelectedBackup);
            OpenSelectedFileCommand = new RelayCommand(OpenSelectedLog);
            CreateNewCarrierCommand = new RelayCommand(CreateCarrier);
            // Methods to invoke upon call
            LoadLogFiles();
            LoadBackupFiles();
            LoadData();
        }

        private void CreateCarrier()
        {
            _carrierModel.CreateCarrier(CompanyName, Ftla,Ltla, FtlaRate,LtlaRate,ReefCharge);
            LoadData();
        }

        /// <summary>
        /// Method to load data.
        /// </summary>
        private void LoadData()
        {
            CarrierData = new ObservableCollection<Carrier>(_adminModel.LoadCarrierTable() ?? throw new InvalidOperationException());
        }

        /// <summary>
        /// Method to save carrier changes.
        /// </summary>
        private void SaveCarrierChanges()
        {
            try
            {
                if (CarrierData != null) _adminModel.SaveChanges(CarrierData.ToList());
                LoadData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            MessageBox.Show("New Carrier added successfully!");
            
             
        }

        /// <summary>
        /// Method to perform the backup operation.
        /// </summary>
        private void BackUp()
        {
            const int numOfIterations = 100;

            Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Visible; });

            Task.Run(() =>
            {
                for (int i = 0; i <= numOfIterations; i++)
                {
                    Thread.Sleep(50);
                    var currentIteration = i;
                    Application.Current.Dispatcher.Invoke(() => { BackUpProgress = (currentIteration * 100) / numOfIterations; });
                }
            }).ContinueWith(_ =>
            {
                try
                {
                    _adminModel.BackUpDatabase();
                    Application.Current.Dispatcher.Invoke(() => { BackUpProgress = 100; });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LoggerModel.LogException("Error performing backup.");
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Hidden; });
                    MessageBox.Show("Backup completed successfully!", "Backup operation", MessageBoxButton.OK);
                }
            });
        }

        /// <summary>
        /// Method to open the selected log file.
        /// </summary>
        private void OpenSelectedLog()
        {
            var logFolderPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
            if (!string.IsNullOrEmpty(SelectedLogFile)) // Check if the selected string is empty or not
            {
                string filePath = Path.Combine(logFolderPath, SelectedLogFile); // Construct file path

                try
                {
                    Process.Start(new ProcessStartInfo // Start opening process, using default application 
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening file: {ex.Message}");
                    LoggerModel.LogException("Error opening log file.");
                }
            }
        }

        /// <summary>
        /// Method to open the selected backup file.
        /// </summary>
        private void OpenSelectedBackup()
        {
            const string backupFolderPath = @"C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\bin\\Debug\\net6.0-windows\\Backup";
            if (!string.IsNullOrEmpty(SelectedBackupFile)) // Check if selected string is empty or not
            {
                var filePath = Path.Combine(backupFolderPath, SelectedBackupFile); // Construct filepath

                try
                {
                    Process.Start(new ProcessStartInfo // Open file with default application
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening file: {ex.Message}");
                    LoggerModel.LogException($"Error opening sql file.");
                }
            }
        }

        /// <summary>
        /// Method to load log files.
        /// </summary>
        private void LoadLogFiles()
        {
            var directoryPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
            LogFiles = new ObservableCollection<string>();
            LoadFiles(directoryPath, LogFiles, ref _selectedLogFile);
        }

        /// <summary>
        /// Method to load backup files.
        /// </summary>
        private void LoadBackupFiles()
        {
            const string backupFolderPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Backup";
            BackupFiles = new ObservableCollection<string>();
            LoadFiles(backupFolderPath, BackupFiles, ref _selectedBackupFile);
        }

        /// <summary>
        /// Method to load files from the directory into the target collection.
        /// </summary>
        /// <param name="directoryPath">Where to load files from.</param>
        /// <param name="targetCollection">Where loaded filenames will be stored.</param>
        /// <param name="selectedFile">Reference to the string variable that will be updated with the first line in targetCollection.</param>
        private void LoadFiles(string directoryPath, ObservableCollection<string> targetCollection, ref string selectedFile)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    var fileNames = Directory.GetFiles(directoryPath);
                    targetCollection.Clear();

                    foreach (var filename in fileNames) targetCollection.Add(Path.GetFileName(filename));

                    if (targetCollection.Count > 0) selectedFile = targetCollection[0];
                }
                else
                {
                    LoggerModel.LogError("Error retrieving filenames.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoggerModel.LogException("Error loading files.");
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
