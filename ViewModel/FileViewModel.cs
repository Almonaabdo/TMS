using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        #region Fields

        private readonly DataService _dataService;
        private string _selectedLogFile;
        private string _selectedBackupFile;
        private int _backUpProgress;
        private Visibility _progressBarVisibility = Visibility.Collapsed;

        #endregion

        #region Properties

        public ObservableCollection<string> LogFiles { get; private set; }
        public ObservableCollection<string> BackupFiles { get; private set; }

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

        #endregion

        #region Commands

        public RelayCommand OpenSelectedFileCommand { get; }
        public RelayCommand OpenSelectedBackupCommand { get; set; }
        public RelayCommand BackUpDbCommand { get; set; }

        #endregion

        #region Constructor

        public FileViewModel()
        {

            _dataService = new DataService();
            BackUpDbCommand = new RelayCommand(BackUp);
            OpenSelectedBackupCommand = new RelayCommand(OpenSelectedBackup);
            OpenSelectedFileCommand = new RelayCommand(OpenSelectedLog);
            LoadLogFiles();
            LoadBackupFiles();
        }

        #endregion

        #region Methods

        public void BackUp()
        {
            const int numOfIterations = 100;

            Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Visible; });

#pragma warning disable CA2008
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
                    _dataService.BackUpDatabase("Server=localhost;Database=tms;User=root;Password=root;");
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
#pragma warning restore CA2008
        }

        private void OpenSelectedLog()
        {
            var logFolderPath =
                "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
            if (!string.IsNullOrEmpty(SelectedLogFile)) // Check if the selected string is empty or not
            {
                string filePath = Path.Combine(logFolderPath, SelectedLogFile); // Construct file path

                try
                {
                    Process.Start(new ProcessStartInfo // Start opening process, using the default application 
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

        private void OpenSelectedBackup()
        {
            const string backupFolderPath =
                @"C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\bin\\Debug\\net6.0-windows\\Backup";
            if (!string.IsNullOrEmpty(SelectedBackupFile)) // Check if the selected string is empty or not
            {
                var filePath = Path.Combine(backupFolderPath, SelectedBackupFile); // Construct filepath

                try
                {
                    Process.Start(new ProcessStartInfo // Open file with the default application
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
        }

        private void LoadLogFiles()
        {
            var directoryPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
            LogFiles = new ObservableCollection<string>();
            LoadFiles(directoryPath, LogFiles, ref _selectedLogFile);
        }

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
        private static void LoadFiles(string directoryPath, ObservableCollection<string> targetCollection,
            ref string selectedFile)
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

        #endregion
    }
}
