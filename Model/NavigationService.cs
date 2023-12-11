using System.Windows;
using TMS_Project.View;

namespace TMS_Project.Model;

public class NavigationService
{
    private static void CloseCurrWindow()
    {
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Close();
        }
    }
    /// <summary>
    /// Method to navigate to the admin window
    /// </summary>
    public void NavigateToAdmin()
    {
        var adminWindow = new AdminView();
        adminWindow.Show();
        CloseCurrWindow();
        
    }

    public void NavigateToLogin()
    {
        var loginWindow = new LogInView();
        loginWindow.Show();

        // Close the previous window (assuming it's not the main window)
        foreach (var window in Application.Current.Windows)
        {
            if (window != loginWindow)
            {
                ((Window)window).Close();
            }
        }
    }


    /// <summary>
    /// Method to navigate to planner view
    /// </summary>
    public void NavigateToPlanner()
    {
        var plannerWindow = new PlannerView();
        plannerWindow.Show();
        CloseCurrWindow();
    }

    /// <summary>
    /// Method to navigate to buyer window
    /// </summary>
    public void NavigateToBuyer()
    {
        var buyerWindow = new BuyerView();
        buyerWindow.Show();
        CloseCurrWindow();
    }
}