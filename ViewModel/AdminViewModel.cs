// ViewModel class

using System;
using System.Collections.ObjectModel;
using TMS_Project.DataLayer.Model;
using TMS_Project.Model;

namespace TMS_Project.ViewModel;

/// <summary>
///     ViewModel for the admin functionalities.
/// </summary>
public sealed class AdminViewModel : ViewModelBase
{
    #region Fields

    private readonly DataService _dataService;
    public CarrierViewModel CarrierViewModel { get; } = new();
    public FileViewModel FileViewModel { get; } = new();
    public DeleteViewModel DeleteViewModel { get; } = new();
    public ObservableCollection<JoinedRouteTable> RouteData { get; private set; }
    public ObservableCollection<Rate> RatesData { get; private set; }
    public LogInViewModel LogInViewModel { get; private set; } = new();

    #endregion

    #region Constructor

    /// <summary>
    ///     Constructor to initialize necessary commands, methods, and variables.
    /// </summary>
    public AdminViewModel()
    {
        _dataService = new DataService();
        LoadData();
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Method to load data.
    /// </summary>
    private void LoadData()
    {
        RouteData = new ObservableCollection<JoinedRouteTable>(_dataService.GetJoinedRouteDatas());

        RatesData = new ObservableCollection<Rate>(_dataService.RetrieveTable<Rate>() ??
                                                   throw new InvalidOperationException());
    }

    #endregion
}