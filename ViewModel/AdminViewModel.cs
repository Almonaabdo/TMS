using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public sealed class AdminViewModel : INotifyPropertyChanged
{
    
    // Initialize variables
    private readonly AdminModel _adminModel;
    private ObservableCollection<string> _files; 
    public ObservableCollection<Carrier> CarrierData { get; private set; }
    private string _selectedLogFile;
    
    // Initialize commands
    public ICommand BackUpDbCommand;
    public ICommand SaveChangesCommand { get; }
    public ICommand OpenSelectedFileCommand { get; }


    /// <summary>
    /// Property to hold the selected file
    /// </summary>
    public string SelectedLogFile
    {
        get => _selectedLogFile;
        set
        {
            _selectedLogFile = value;
            OnPropertyChanged(nameof(SelectedLogFile));
        }
    }

    /// <summary>
    /// Property to hold files
    /// </summary>
    public ObservableCollection<string> Files
    {
        get => _files;
        set
        {
            _files = value;
            OnPropertyChanged(nameof(Files));
        }
    }

    /// <summary>
    /// Default constructor to initialize
    /// </summary>
    public AdminViewModel()
    {
        // Initialize
        Files = new ObservableCollection<string>();
        CarrierData = new ObservableCollection<Carrier>();
        _adminModel = new AdminModel();

        // Commands
        BackUpDbCommand = new RelayCommand(BackUp, CanBackUp);
        SaveChangesCommand = new RelayCommand(SaveChanges);
        OpenSelectedFileCommand = new RelayCommand(OpenSelected);
        
        // Methods
        LoadFiles();
        LoadTableData();
    }

    /// <summary>
    /// Method to open the selected file, using default application -- NOTE: Hardcoded path for now
    /// </summary>
    private void OpenSelected()
    {
        var logFolderPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
        if (!string.IsNullOrEmpty(SelectedLogFile))
        {
            // Construct the file path
            string filePath = Path.Combine(logFolderPath, SelectedLogFile);

            try
            {
                // Use the default application for the file type
                Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });

                //LoadFiles();  just for now 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}"); // Testing purposes
            }
        }
    }

    /// <summary>
    /// Method to load all files within the selected directory -- NOTE: Hardcoded path for now
    /// </summary>
    private void LoadFiles()
    {
        // path
        var directoryPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";

        try
        {
            if (Directory.Exists(directoryPath))
            {
                var fileNames = Directory.GetFiles(directoryPath);
                Files.Clear(); // Clear existing items

                foreach (var filename in fileNames) Files.Add(Path.GetFileName(filename));

                if (Files.Count > 0) SelectedLogFile = Files[0];
            }
            else
            {
                LoggerModel.LogError("Error retrieving filenames.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Method to save changes make to db table
    /// </summary>
    private void SaveChanges()
    {
        _adminModel.SaveChanges(CarrierData.ToList()); // Call method to save changes
        LoadTableData(); // Reload to see changes
    }

    /// <summary>
    /// Method to call model and load Carrier table to property
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void LoadTableData()
    {
        CarrierData =
            new ObservableCollection<Carrier>(_adminModel.LoadCarrierTable() ?? throw new InvalidOperationException());
    }


    /// <summary>
    ///  Determines if the backup command can be executed
    /// </summary>
    /// <returns>True if the backup command can be executed</returns>
    private bool CanBackUp()
    {
        return true;
    }

    /// <summary>
    ///  Executes backup command to initiate database back up
    /// </summary>
    private void BackUp()
    {
        _adminModel.BackUpDatabase();
    }

    /// <summary>
    ///     Invokes property Changed event when a property changes
    /// </summary>
    /// <param name="propertyName">Name of property that changed</param>
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}