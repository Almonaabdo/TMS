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
    public ICommand BackUpDbCommand;
    public ICommand SaveChangesCommand { get; }
    public ICommand OpenSelectedFileCommand { get; }

    private readonly AdminModel _adminModel;
    private ObservableCollection<string> _files;
    public ObservableCollection<Carrier> CarrierData { get; private set; }

    private string _selectedLogFile;

    public string SelectedLogFile
    {
        get => _selectedLogFile;
        set
        {
            _selectedLogFile = value;
            OnPropertyChanged(nameof(SelectedLogFile));
        }
    }

    public ObservableCollection<string> Files
    {
        get => _files;
        set
        {
            _files = value;
            OnPropertyChanged(nameof(Files));
        }
    }

    public AdminViewModel()
    {
        Files = new ObservableCollection<string>();


        CarrierData = new ObservableCollection<Carrier>();
        _adminModel = new AdminModel();

        // Commands
        BackUpDbCommand = new RelayCommand(BackUp, CanBackUp);
        SaveChangesCommand = new RelayCommand(SaveChanges);
        OpenSelectedFileCommand = new RelayCommand(OpenSelected);
        LoadFiles();
        LoadData();
    }

    private void OpenSelected()
    {
        var logFolderPath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS - Copy\\bin\\Debug\\net6.0-windows\\Logs";
        if (!string.IsNullOrEmpty(SelectedLogFile))
        {
            string filePath = Path.Combine(logFolderPath, SelectedLogFile);

            try
            {
                // Use the default associated application for the file type
                Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
            }
        }
    }

    private void LoadFiles()
    {
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

    private void SaveChanges()
    {
        _adminModel.SaveChanges(CarrierData.ToList());
        LoadData();
    }

    private void LoadData()
    {
        CarrierData =
            new ObservableCollection<Carrier>(_adminModel.LoadCarrierTable() ?? throw new InvalidOperationException());
    }


    /// <summary>
    ///     Determines if the backup command can be executed
    /// </summary>
    /// <returns>True if the backup command can be executed</returns>
    private bool CanBackUp()
    {
        return true;
    }

    /// <summary>
    ///     Executes backup command to initiate database back up
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