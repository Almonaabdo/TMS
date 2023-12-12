using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        #region Fields

        private readonly DataService _dataService;
        private string? _selectedLogFile;
        private string? _selectedBackupFile;
        private int _backUpProgress;
        private Visibility _progressBarVisibility = Visibility.Collapsed;
        private string _logFilesPath;
        private string? _fileContent;
 public ObservableCollection<string?> LogFiles { get; private set; }
        public ObservableCollection<string?> BackupFiles { get; private set; }
        
        #endregion

        #region MyRegion

        

        
        public string? SelectedLogFile
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

        public string? SelectedBackupFile
        {
            get => _selectedBackupFile;
            set
            {
                _selectedBackupFile = value;
                OnPropertyChanged(nameof(SelectedBackupFile));
            }
        }

        public string LogFilesPath
        {
            get => _logFilesPath;
            set
            {
                _logFilesPath = value;
                OnPropertyChanged(nameof(LogFilesPath));
            }
        }

        public string? FileContent
        {
            get => _fileContent;
            set
            {
                _fileContent = value;
                OnPropertyChanged(nameof(FileContent));
            }
        }
        
        #endregion 

        #region Commands

        public RelayCommand OpenSelectedFileCommand { get; }
        public RelayCommand OpenSelectedBackupCommand { get; set; }
        public RelayCommand BackUpDbCommand { get; set; }
        public RelayCommand OpenFileBrowserCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileViewModel"/> class.
        /// </summary>
        public FileViewModel()
        {
            _dataService = new DataService();
            BackUpDbCommand = new RelayCommand(BackUp);
            OpenSelectedBackupCommand = new RelayCommand(OpenSelectedBackup);
            OpenSelectedFileCommand = new RelayCommand(OpenSelectedLog, CanOpenLog);
            OpenFileBrowserCommand = new RelayCommand(OpenFileBrowser);
            LoadLogFiles();
            LoadBackupFiles();
        }

        private bool CanOpenLog()
        {
            return !string.IsNullOrEmpty(SelectedLogFile);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initiates the backup process.
        /// </summary>
        public void BackUp()
        {
            const int numOfIterations = 100;

            Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Visible; });

            Task.Run(() =>
            {
                for (int i = 0; i <= numOfIterations; i++)
                {
                    Thread.Sleep(50);
                    var currentIteration = i;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BackUpProgress = (currentIteration * 100) / numOfIterations;
                    });
                }
            }).ContinueWith(_ =>
            {
                try
                {
                    _dataService.BackUpDatabase();
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
        /// Opens the selected log file using the default application.
        /// </summary>
        private void OpenSelectedLog()
        {
            try
            {
                if (CanOpenLog())
                {
                    if (SelectedLogFile != null)
                    {
                        string allFileContent = File.ReadAllText(SelectedLogFile);
                        FileContent = allFileContent;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
                LoggerModel.LogException("Error opening log file.");
            }
        }

        #region Backup

        /// <summary>
        /// Opens the selected backup file using the default application.
        /// </summary>
        private void OpenSelectedBackup()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var backupFolderPath = configuration["Backups:BackupFolder"];
            
            if (string.IsNullOrEmpty(SelectedBackupFile)) return;
            
            if (backupFolderPath == null) return;
            
            var filePath = Path.Combine(backupFolderPath, SelectedBackupFile);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
                LoggerModel.LogException($"Error opening SQL file.");
            }
        }

        /// <summary>
        /// Loads the backup files.
        /// </summary>
        private void LoadBackupFiles()
        {
            const string backupFolderPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Backup";
            BackupFiles = new ObservableCollection<string?>();
            LoadFiles(backupFolderPath, BackupFiles, ref _selectedBackupFile);
        }

        #endregion

        /// <summary>
        /// Loads the log files.
        /// </summary>
        private void LoadLogFiles()
        {
            LogFiles = new ObservableCollection<string?>();
            LoadFiles(LogFilesPath, LogFiles, ref _selectedLogFile);
        }

        /// <summary>
        /// Method to load files from the directory into the target collection.
        /// </summary>
        /// <param name="directoryPath">Where to load files from.</param>
        /// <param name="targetCollection">Where loaded filenames will be stored.</param>
        /// <param name="selectedFile">Reference to the string variable that will be updated with the first line in targetCollection.</param>
        private static void LoadFiles(string directoryPath, ObservableCollection<string?> targetCollection, ref string? selectedFile)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    var fileNames = Directory.GetFiles(directoryPath);
                    targetCollection.Clear();

                    foreach (var filename in fileNames) targetCollection.Add(filename);

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

        /// <summary>
        /// Opens the file browser dialog to select log files.
        /// </summary>
        public void OpenFileBrowser()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Log Files",
                Filter = "Log Files (*.log)|*.log|All Files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var selectedFilePath in openFileDialog.FileNames)
                    LogFiles.Add(selectedFilePath);

                SelectedLogFile = LogFiles.FirstOrDefault();  // Set the first item as selected
            }
        }

        #endregion
    }
}
