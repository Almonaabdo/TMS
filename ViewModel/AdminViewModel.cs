// ViewModel class

using System;
using System.Collections.ObjectModel;
using TMS_Project.DataLayer.Model;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    /// <summary>
    /// ViewModel for the admin functionalities.
    /// </summary>
    public sealed class AdminViewModel : ViewModelBase
    {
        // Variables
        private readonly DataService _dataService;
        public CarrierViewModel CarrierViewModel { get; } = new();
        public FileViewModel FileViewModel { get; } = new();
        public DeleteViewModel DeleteViewModel { get; } = new();
        public ObservableCollection<Route> RouteData { get; private set; }
        public ObservableCollection<Rate> RatesData { get; private set; }






        /// <summary>
        /// Constructor to initialize necessary commands, methods, and variables.
        /// </summary>
        public AdminViewModel()
        {
            _dataService = new DataService();
            LoadData();
        }

        /// <summary>
        /// Method to load data.
        /// </summary>
        private void LoadData()
        {
            RouteData = new ObservableCollection<Route>(_dataService.RetrieveTable<Route>() ??
                                                        throw new InvalidOperationException());
            RatesData = new ObservableCollection<Rate>(_dataService.RetrieveTable<Rate>() ??
                                                       throw new InvalidOperationException());
        }




    }
}