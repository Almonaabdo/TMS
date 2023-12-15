using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public class DeleteViewModel : ViewModelBase
    {
        #region Fields

        private string? _selectedDeleteOption;
        private List<string>? _deleteOptions;
        private readonly AdminServices _adminServices;
        private ObservableCollection<JoinedRouteTable>? _routeTable;
        private ObservableCollection<Rate>? _rateTable;
        private ObservableCollection<Carrier>? _carrierTable;
        private readonly LoggerModel _loggerModel = LoggerModel.Instance;
        private Carrier? _selectedCarrier;
        private JoinedRouteTable? _selectedRoute;
        private Rate? _selectedRate;

        private Visibility _isRouteTableVisible = Visibility.Hidden;
        private Visibility _isRateTableVisible = Visibility.Hidden;
        private Visibility _isCarrierTableVisible = Visibility.Hidden;

        #endregion

        #region Properties

        public List<string>? DeleteOptions
        {
            get => _deleteOptions;
            set
            {
                _deleteOptions = value;
                OnPropertyChanged(nameof(DeleteOptions));
            }
        }

        public string? SelectedDeleteOption
        {
            get => _selectedDeleteOption;
            set
            {
                _selectedDeleteOption = value;
                LoadTableData();
                OnPropertyChanged(nameof(SelectedDeleteOption));
            }
        }

        public ObservableCollection<JoinedRouteTable>? RouteTable
        {
            get => _routeTable;
            set
            {
                _routeTable = value;
                OnPropertyChanged(nameof(RouteTable));
            }
        }

        public ObservableCollection<Rate>? RateTable
        {
            get => _rateTable;
            set
            {
                _rateTable = value;
                OnPropertyChanged(nameof(RateTable));
            }
        }

        public ObservableCollection<Carrier>? CarrierTable
        {
            get => _carrierTable;
            set
            {
                _carrierTable = value;
                OnPropertyChanged(nameof(CarrierTable));
            }
        }

        #endregion

        #region Commands

        public RelayCommand DeleteCommand { get; }

        #endregion

        #region Visibility Properties

        public Visibility IsRouteTableVisible
        {
            get => _isRouteTableVisible;
            set
            {
                _isRouteTableVisible = value;
                OnPropertyChanged(nameof(IsRouteTableVisible));
            }
        }

        public Visibility IsRateTableVisible
        {
            get => _isRateTableVisible;
            set
            {
                _isRateTableVisible = value;
                OnPropertyChanged(nameof(IsRateTableVisible));
            }
        }

        public Visibility IsCarrierTableVisible
        {
            get => _isCarrierTableVisible;
            set
            {
                _isCarrierTableVisible = value;
                OnPropertyChanged(nameof(IsCarrierTableVisible));
            }
        }

        #endregion

        #region Selected Item Properties

        public Carrier? SelectedCarrier
        {
            get => _selectedCarrier;
            set
            {
                _selectedCarrier = value;
                OnPropertyChanged(nameof(SelectedCarrier));
            }
        }

        public Rate? SelectedRate
        {
            get => _selectedRate;
            set
            {
                _selectedRate = value;
                OnPropertyChanged(nameof(SelectedRate));
            }
        }

        public JoinedRouteTable? SelectedRoute
        {
            get => _selectedRoute;
            set
            {
                _selectedRoute = value;
                OnPropertyChanged(nameof(SelectedRoute));
            }
        }

        #endregion

        #region Constructor

        public DeleteViewModel()
        {
            _adminServices = new AdminServices();
            DeleteOptions = new List<string> { "Route", "Rate", "Carrier" }; // Add other options as needed
            DeleteCommand = new RelayCommand(DeleteData);
        }

        #endregion

        #region Methods


        /*
        * METHOD NAME: LoadTableData
        * DESCRIPTION: Displays data in the specified table 
        * 
        * RETURN: void
        */
        private void LoadTableData()
        {
            switch (SelectedDeleteOption)
            {
                case "Route":
                    RouteTable = new ObservableCollection<JoinedRouteTable>(_adminServices.GetJoinedRouteData()?.Cast<JoinedRouteTable>() ??
                                                                             throw new InvalidOperationException());
                    IsRouteTableVisible = Visibility.Visible;
                    IsRateTableVisible = Visibility.Hidden; // Ensure other tables are hidden
                    IsCarrierTableVisible = Visibility.Hidden;
                    break;
                case "Rate":
                    RateTable = new ObservableCollection<Rate>(_adminServices.RetrieveTable<Rate>()?.Cast<Rate>() ??
                                                               throw new InvalidOperationException());
                    IsRateTableVisible = Visibility.Visible;
                    IsRouteTableVisible = Visibility.Hidden; // Ensure other tables are hidden
                    IsCarrierTableVisible = Visibility.Hidden;
                    break;
                case "Carrier":
                    CarrierTable = new ObservableCollection<Carrier>(
                        _adminServices.RetrieveTable<Carrier>()?.Cast<Carrier>() ?? throw new InvalidOperationException());
                    IsCarrierTableVisible = Visibility.Visible;
                    IsRouteTableVisible = Visibility.Hidden; // Ensure other tables are hidden
                    IsRateTableVisible = Visibility.Hidden;
                    break;
                default:
                    MessageBox.Show("Please select a valid option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }


        /*
        * METHOD NAME: DeleteData
        * DESCRIPTION: Deletes a row of data in the specified table
        * 
        * RETURN: void
        */
        private void DeleteData()
        {
            try
            {
                string? tableId;
                switch (SelectedDeleteOption)
                {
                    case "Route":
                        tableId = SelectedRoute?.RouteId.ToString();
                        DeleteRows<Route>(tableId);
                        break;
                    case "Rate":
                        tableId = SelectedRate?.RateId.ToString();
                        DeleteRows<Rate>(tableId);
                        break;
                    case "Carrier":
                        tableId = SelectedCarrier?.CarrierId.ToString();
                        DeleteRows<Carrier>(tableId);
                        break;
                    default:
                        MessageBox.Show("Please select a valid option.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception e)
            { 
                _loggerModel.LogException($"{e.Message}");
                MessageBox.Show("Error deleting selected row. Please try again later.", "Error saving changes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /*
        * METHOD NAME: DeleteRows
        * DESCRIPTION: Asks the user to confirm table row data deletion
        * PARAM: tableID - uses tableID to locate the row to be deleted
        * RETURN: void
        */
        private void DeleteRows<T>(string? tableId) where T : class
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete the selected row?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (int.TryParse(tableId, out var id))
                    {
                        _adminServices.DeleteData<T>(id);
                    }
                    // Refresh the data after deletion
                    LoadTableData();

                    MessageBox.Show("Selected row deleted successfully!");
                }
            }
            catch (Exception e)
            {
                _loggerModel.LogException($"{e.Message}");
                MessageBox.Show("Error deleting row", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
