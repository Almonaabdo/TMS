using System.Windows.Input;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

public sealed class AdminViewModel
{
    public ICommand BackUpDbCommand { get; }
    private readonly AdminModel _adminModel;

    public AdminViewModel()
    {
        _adminModel = new AdminModel();
        // Command to trigger backup operation
        BackUpDbCommand = new RelayCommand(BackUp, CanBackUp);
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
    /// Executes backup command to initiate database back up
    /// </summary>
    private void BackUp()
    {
        _adminModel.BackUpDatabase();
    }
}