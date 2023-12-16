using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Devart.Data.MySql;
using TMS_Project.DataLayer.Model;
using TMS_Project.Helper;
using TMS_Project.Model;

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

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));

            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize necessary commands, methods, and variables.
        /// </summary>
        public AdminViewModel()
        {
            _currentDate = DateTime.Today;
            TestDbCommand = new RelayCommand(TestConnection, CanTestConnection);
        }

        public ICommand? Increase { get; set; }



        #endregion

        #region MyRegion

        private string? _server;
        private string? _port;
        private string? _database;
        private string? _username;
        private string? _password;

        public string? Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
            }
        }

        public string? Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public string? Database
        {
            get => _database;
            set
            {
                _database = value;
                OnPropertyChanged(nameof(Database));
            }
        }

        public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        #endregion
       
        public ICommand TestDbCommand { get; }

        #region Methods

        /*
         * METHOD NAME: TestConnection
         * DESCRIPTION: Tests the connection to the remote DB
         *
         * RETURN: void
         */
        private void TestConnection()
        {

            if (int.TryParse(Port, out var portNum))
            {
                var connectionString =
                    $"Server={Server};Port={portNum};Database={Database};User ID={Username};Password={Password};";
                using var connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    MessageBox.Show("Connection successful");
                }
                catch (Exception ex)
                {
                    LoggerModel.Instance.LogException($"{ex.Message}");
                    MessageBox.Show($"Connection failed. Error: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Please enter valid information.", "Connection failure", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

        }

        /*
         * METHOD NAME: CanTestConnection
         * DESCRIPTION: CanExecute method to check if button can eb enabled or not based on text input
         *
         * RETURN: void
         */
        private bool CanTestConnection()
        {
            if (!string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(Username) &&
                !string.IsNullOrEmpty(Password) & !string.IsNullOrEmpty(Database))
            {
                return true;
            }

            return false;
        }

        #endregion
      


    }
}
