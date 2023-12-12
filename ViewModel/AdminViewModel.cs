using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TMS_Project.Helper;

namespace TMS_Project.ViewModel
{
    /// <summary>
    /// ViewModel for the admin functionalities.
    /// </summary>
    public sealed class AdminViewModel : ViewModelBase
    {
        #region Fields

        // Various view models for different functionalities
        public CarrierViewModel CarrierViewModel { get; } = new();
        public FileViewModel FileViewModel { get; } = new();
        public DeleteViewModel DeleteViewModel { get; } = new();

        // Private fields for Login and RateRoute view models
        private LogInViewModel _logInViewModel = new LogInViewModel();
        private RateRouteViewModel _rateRouteViewModel = new RateRouteViewModel();

        #endregion

        #region Properties

        // Public properties for accessing LogInViewModel and RateRouteViewModel
        public LogInViewModel LogInViewModel
        {
            get => _logInViewModel;
            set
            {
                _logInViewModel = value;
                OnPropertyChanged(nameof(LogInViewModel));
            }
        }

        public RateRouteViewModel RateRouteViewModel
        {
            get => _rateRouteViewModel;
            set
            {
                _rateRouteViewModel = value;
                OnPropertyChanged(nameof(RateRouteViewModel));
            }
        }

        #endregion

        #region Date

        private DateTime _currentDate;

        public ICommand IncrementTimeCommand { get; set; }

        public string CurrentDate => _currentDate.ToString("MM-dd-yyyy");

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize necessary commands, methods, and variables.
        /// </summary>
        public AdminViewModel()
        {
            _currentDate = DateTime.Now;
            IncrementTimeCommand = new RelayCommand(IncrementDate);
        }

        private void IncrementDate()
        {
            _currentDate = _currentDate.AddDays(1);
            OnPropertyChanged(nameof(CurrentDate));
        }

        #endregion
    }
}
