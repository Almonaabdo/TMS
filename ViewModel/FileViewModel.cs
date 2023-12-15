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

        private readonly LoggerModel _loggerModel = LoggerModel.Instance;
        private readonly ConfigService _configService;
        private readonly AdminServices _adminServices;
        private string? _selectedLogFile;
        private string? _selectedBackupFile;
        private int _backUpProgress;
        private Visibility _progressBarVisibility = Visibility.Collapsed;
        private string _logFilesPath = null!;
        private string? _fileContent;
         public ObservableCollection<string?> LogFiles { get; private set; } = null!;
         public ObservableCollection<string?> BackupFiles { get; private set; } = null!;

         #endregion


        #region Log File Properties

        public string? SelectedLogFile
        {
            get => _selectedLogFile;
            set
            {
                _selectedLogFile = value;
                OnPropertyChanged(nameof(SelectedLogFile));
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

        #endregion
        
        
        #region Backup Properties

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

        #endregion
        
        
        #region File Content Properties

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
            _adminServices = new AdminServices();
            _configService = new ConfigService();
            BackUpDbCommand = new RelayCommand(BackUp);
            OpenSelectedBackupCommand = new RelayCommand(OpenSelectedBackup);
            OpenSelectedFileCommand = new RelayCommand(OpenSelectedLog, CanOpenLog);
            OpenFileBrowserCommand = new RelayCommand(OpenFileBrowser);
            LoadLogFiles();
            LoadBackupFiles();
        }


        /*
        * METHOD NAME: CanOpenLog
        * DESCRIPTION: Checks whether to open log fie or not
        * 
        * RETURN: bool
        */
        private bool CanOpenLog()
        {
            return !string.IsNullOrEmpty(SelectedLogFile);
        }

        #endregion


        #region Backup

       
        /*
        * METHOD NAME: BackUp
        * DESCRIPTION: Initiates the backup process
        * 
        * RETURN: void
        */
        public void BackUp()
        {
            const int numOfIterations = 100;

            Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Visible; });

            Task.Run(() =>
            {
                for (int i = 0; i <= numOfIterations; i++)
                {
                    Thread.Sleep(10);
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
                    _adminServices.BackUpDatabase();
                    Application.Current.Dispatcher.Invoke(() => { BackUpProgress = 100; });
                    
                }
                catch (Exception e)
                {
                   _loggerModel.LogException($"Error performing backup.{e.Message}");
                }
                finally
                {
                    LoadBackupFiles();
                    Application.Current.Dispatcher.Invoke(() => { ProgressBarVisibility = Visibility.Hidden; });
                    MessageBox.Show("Backup completed successfully!", "Backup operation", MessageBoxButton.OK);
                }
            });
        }


        /*
        * METHOD NAME: OpenSelectedBackup
        * DESCRIPTION: Opens the selected backup file using the default application.
        * 
        * RETURN: void
        */
        private void OpenSelectedBackup()
        {
            // IConfigurationRoot configuration = new ConfigurationBuilder()
            //     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //     .AddJsonFile("appsettings.json")
            //     .Build();

            var backupFolderPath = _configService.GetBackupPath();
            
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
                _loggerModel.LogException($"Error opening SQL file.{ex.Message}");
            }
        }


        /*
        * METHOD NAME: LoadBackupFiles
        * DESCRIPTION: Loads the backup files.
        * 
        * RETURN: void
        */
        private void LoadBackupFiles()
        {
            // Specify path for storing backups
            // IConfigurationRoot backUpFolder = new ConfigurationBuilder()
            //     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //     .AddJsonFile("appsettings.json")
            //     .Build();

            var path = _configService.GetBackupPath();
            BackupFiles = new ObservableCollection<string?>();
            if (path != null) LoadFiles(path, BackupFiles, ref _selectedBackupFile);
        }

        #endregion


        #region  Log files

        /*
        * METHOD NAME: OpenSelectedLog
        * DESCRIPTION: Opens the selected log file using the default application
        * 
        * RETURN: void
        */
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
                _loggerModel.LogException($"Error opening log file.{ex.Message}");
            }
        }


        /*
        * METHOD NAME: LoadLogFiles
        * DESCRIPTION: Loads the log files
        * 
        * RETURN: void
        */
        private void LoadLogFiles()
        {
            LogFiles = new ObservableCollection<string?>();
            LoadFiles(LogFilesPath, LogFiles, ref _selectedLogFile);
        }


        /*
        * METHOD NAME: LoadFiles
        * DESCRIPTION: Method to load files from the directory into the target collection.
        * PARAM: directoryPath - Where to load files from, 
        *        targetCollection - Where loaded filenames will be stored, 
        *        selectedFile - Reference to the string variable that will be updated with the first line in targetCollection
        * RETURN: void
        */
        private void LoadFiles(string directoryPath, ObservableCollection<string?> targetCollection, ref string? selectedFile)
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
                    _loggerModel.LogError("Error retrieving filenames.");
                }
            }
            catch (Exception e)
            {
                _loggerModel.LogException($"Error loading files.{e.Message}");
            }
        }

       

        /*
        * METHOD NAME: OpenFileBrowser
        * DESCRIPTION: Opens the file browser dialog to select log files.
        * 
        * RETURN: void
        */
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
