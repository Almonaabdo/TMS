using System;
using System.Windows;
using NLog;
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
        try
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #region Admin


    /*
    * METHOD NAME: NavigateToAdmin
    * DESCRIPTION: Method to navigate to the admin window
    *
    * RETURN: void
    */
    public void NavigateToAdmin()
    {
        try
        {
            var adminWindow = new AdminView();
            adminWindow.Show();
            CloseCurrWindow();
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion

    #region Login


    /*
    * METHOD NAME: NavigateToLogin
    * DESCRIPTION: Method to navigate to the login window
    *
    * RETURN: void
    */
    public void NavigateToLogin()
    {
        try
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
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion

    #region Planner


    /*
    * METHOD NAME: NavigateToPlanner
    * DESCRIPTION: Method to navigate to the planner window
    *
    * RETURN: void
    */
    public void NavigateToPlanner()
    {
        try
        {
            var plannerWindow = new PlannerView();
            plannerWindow.Show();
            CloseCurrWindow();
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion

    #region Buyer


    /*
    * METHOD NAME: NavigateToBuyer
    * DESCRIPTION: Method to navigate to the buyer window
    *
    * RETURN: void
    */
    public void NavigateToBuyer()
    {
        try
        {
            var buyerWindow = new BuyerView();
            buyerWindow.Show();
            CloseCurrWindow();
        }
        catch (Exception e)
        {
            LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion
}