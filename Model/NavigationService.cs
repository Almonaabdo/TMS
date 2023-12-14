using System.Windows;
using TMS_Project.View;

namespace TMS_Project.Model;

public class NavigationService
{

    /*
    * METHOD NAME: CloseCurrWindow
    * DESCRIPTION: Closes the current opened window
    *
    * RETURN: void
    */
    private static void CloseCurrWindow()
    {
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Close();
        }
    }


    /*
    * METHOD NAME: NavigateToAdmin
    * DESCRIPTION: Method to navigate to the admin window
    *
    * RETURN: void
    */
    public void NavigateToAdmin()
    {
        var adminWindow = new AdminView();
        adminWindow.Show();
        CloseCurrWindow();
    }


    /*
    * METHOD NAME: NavigateToLogin
    * DESCRIPTION: Method to navigate to the login window
    *
    * RETURN: void
    */
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


    /*
    * METHOD NAME: NavigateToPlanner
    * DESCRIPTION: Method to navigate to the planner window
    *
    * RETURN: void
    */
    public void NavigateToPlanner()
    {
        var plannerWindow = new PlannerView();
        plannerWindow.Show();
        CloseCurrWindow();
    }


    /*
    * METHOD NAME: NavigateToBuyer
    * DESCRIPTION: Method to navigate to the buyer window
    *
    * RETURN: void
    */
    public void NavigateToBuyer()
    {
        var buyerWindow = new BuyerView();
        buyerWindow.Show();
        CloseCurrWindow();
    }
}