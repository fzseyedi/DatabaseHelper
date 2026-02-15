using System.Windows;
using System.Windows.Controls;
using DatabaseBackupManager.ViewModels;

using UserControl = System.Windows.Controls.UserControl;

namespace DatabaseBackupManager.Views;

/// <summary>
/// Interaction logic for DataTransferView.xaml
/// </summary>
public partial class DataTransferView : UserControl
{
    public DataTransferView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the PasswordChanged event for the destination PasswordBox.
    /// </summary>
    private void DestPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is DataTransferViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.DestinationSettings.Password = passwordBox.Password;
        }
    }

    /// <summary>
    /// Populates the destination PasswordBox after async settings loading.
    /// </summary>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (DataContext is DataTransferViewModel viewModel)
            {
                var password = viewModel.GetDestinationPassword();
                if (!string.IsNullOrEmpty(password))
                {
                    DestPasswordBox.Password = password;
                }
            }
        }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
    }
}
