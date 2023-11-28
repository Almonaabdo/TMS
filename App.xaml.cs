using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TMS_Project.View;
using TMS_Project.ViewModel;

namespace TMS_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Override start up to show login window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var loginViewModel = new LogInViewModel();
            var loginWindow = new LogInView
            {
                DataContext = loginViewModel
            };

            loginWindow.Show();
        }
    }

    
}
