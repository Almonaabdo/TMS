using System.Windows;
using System.Windows.Controls;
using TMS_Project.ViewModel;

namespace TMS_Project.View;

public partial class LogInView
{
    /// <summary>
    /// Initializes a new instance of the LogInView class.
    /// </summary>
    public LogInView()
    {
        InitializeComponent();

        // Set the DataContext to an instance of LogInViewModel for data binding
        DataContext = new LogInViewModel();
    }

    /// <summary>
    /// Event handler for the PasswordBox's PasswordChanged event.
    /// Updates the Password property in the associated LogInViewModel.
    /// </summary>
    /// <param name="sender">The event sender (PasswordBox).</param>
    /// <param name="e">The event arguments.</param>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        // Check if the sender is a PasswordBox
        if (sender is not PasswordBox passwordBox) return;

        // Check if the DataContext is an instance of LogInViewModel
        if (DataContext is LogInViewModel viewModel)
        {
            // Update the Password property in the view model with the entered password
            viewModel.Password = passwordBox.Password;
        }
    }
}