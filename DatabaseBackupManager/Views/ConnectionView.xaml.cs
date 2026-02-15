using System.Windows;
using System.Windows.Controls;
using DatabaseBackupManager.ViewModels;

using UserControl = System.Windows.Controls.UserControl;

namespace DatabaseBackupManager.Views;

/// <summary>
/// Interaction logic for ConnectionView.xaml
/// </summary>
public partial class ConnectionView : UserControl
{
    public ConnectionView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Sets focus to the ServerName TextBox when the view loads.
    /// Also populates the PasswordBox with the saved password after async loading completes.
    /// </summary>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Set focus to ServerName TextBox immediately
        ServerNameTextBox.Focus();

        // Delay password box population to allow async settings loading to complete
        Dispatcher.BeginInvoke(() =>
        {
            if (DataContext is ConnectionViewModel viewModel)
            {
                var password = viewModel.GetCurrentPassword();
                if (!string.IsNullOrEmpty(password))
                {
                    PasswordBox.Password = password;
                }
            }
        }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
    }

    /// <summary>
    /// Handles password changes since PasswordBox doesn't support binding.
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ConnectionViewModel viewModel)
        {
            viewModel.ConnectionSettings.Password = PasswordBox.Password;
        }
    }
}
