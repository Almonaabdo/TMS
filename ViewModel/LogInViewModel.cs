using System;
using System.ComponentModel;
using System.Windows.Input;
using TMS_Project.DataLayer.Context;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public sealed class LogInViewModel : INotifyPropertyChanged
    {
        private readonly LogInModel _userService;
        private readonly TmsDbContext _dbContext;
        
        public LogInViewModel()
        {
            _dbContext = new TmsDbContext(); // Initialize _dbContext with a valid instance
            _userService = new LogInModel(_dbContext);
            LoginCommand = new RelayCommand(Login, CanLogin);

            // Initialize the message property
            _loginMessage = "";
        }

        // Properties bound to the UI
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        // Command to trigger the login process
        public ICommand LoginCommand { get; }

        // Message property to display success message
        private string _loginMessage;
        public string LoginMessage
        {
            get { return _loginMessage; }
            set
            {
                if (_loginMessage != value)
                {
                    _loginMessage = value;
                    OnPropertyChanged(nameof(LoginMessage));
                }
            }
        }

        private bool CanLogin(object parameter)
        {
            // Add any additional validation logic here
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private void Login(object parameter)
        {
            try
            {
                bool isAuthenticated = _userService.VerifyUser(Username, Password);

                if (isAuthenticated)
                {
                    // Authentication successful, set the success message
                    LoginMessage = "Login successful! Welcome!";
                    LoggerModel.LogInfo("Successful login");
                }
                else
                {
                    // Authentication failed, set an error message
                    LoginMessage = "Login failed. Please check your credentials.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                LoggerModel.LogError($"An error occurred during login: {ex.Message}");
                // Optionally: Show a user-friendly error message
                LoginMessage = "An unexpected error occurred during login. Please try again.";
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
