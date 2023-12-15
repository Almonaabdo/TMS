using System;
using System.ComponentModel;
using System.Windows.Input;
using TMS_Project.DataLayer.Context;
using TMS_Project.Helper;
using TMS_Project.Model;

namespace TMS_Project.ViewModel
{
    public sealed class LogInViewModel : ViewModelBase
    {
        #region Fields

        private readonly LogInModel _logInModel;
        private readonly TmsDbContext _dbContext;
        private readonly LoggerModel _loggerModel = LoggerModel.Instance;

        private readonly NavigationService _navigation;

        #endregion

        #region Constructor

        public LogInViewModel()
        {
            _navigation = new NavigationService();
            _dbContext = new TmsDbContext(); // Initialize _dbContext with a valid instance
            _logInModel = new LogInModel(_dbContext);
            LoginCommand = new RelayCommand(Login, CanLogin);
            LogoutCommand = new RelayCommand(Logout);

            // Initialize the message property
            _loginMessage = "";
        }

        #endregion

        #region Properties

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
        public ICommand LogoutCommand { get; }

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

        #endregion

        #region Methods


        /*
        * METHOD NAME: Logout
        * DESCRIPTION: Logs out the current user and to navigate to the login window
        * 
        * RETURN: void
        */
        private void Logout()
        {
            _navigation.NavigateToLogin();
        }

        #region Login methods

       
        /*
        * METHOD NAME: CanLogin
        * DESCRIPTION: Sets Execute method to true
        * 
        * RETURN: bool - true ig not empty, otherwise false
        */
        private bool CanLogin()
        {
            // Add any additional validation logic here
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }


        /*
        * METHOD NAME: Login
        * DESCRIPTION:  Method associated with command to validate login info
        * 
        * RETURN: void
        */
        private void Login()
        {
            try
            {
                bool isAuthenticated = _logInModel.VerifyUser(Username, Password);

                if (isAuthenticated)
                {
                    // Authentication successful, set the success message
                    LoginMessage = "Login successful! Welcome!";
                    _loggerModel.LogInfo("Successful login");

                    // Based on the user, show the appropriate window
                    if (Username.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        _navigation.NavigateToAdmin();
                    }
                    else if (Username.Equals("Buyer", StringComparison.OrdinalIgnoreCase))
                    {
                        _navigation.NavigateToBuyer();
                    }
                    else if (Username.Equals("Planner", StringComparison.OrdinalIgnoreCase))
                    {
                        _navigation.NavigateToPlanner();
                    }
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
                _loggerModel.LogError($"An error occurred during login: {ex.Message}");
                // Optionally: Show a user-friendly error message
                LoginMessage = "An unexpected error occurred during login. Please try again.";
            }
        }

        #endregion

        #endregion
    }
}
