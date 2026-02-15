using System.Windows.Input;
using DatabaseBackupManager.Helpers;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// Base class for all ViewModels in the application.
/// Provides common functionality like busy state and status messages.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private bool _isBusy;
    private string _statusMessage = string.Empty;
    private bool _hasError;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Gets or sets whether the ViewModel is currently busy.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    /// <summary>
    /// Gets or sets the current status message.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Gets or sets whether there is an error.
    /// </summary>
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                HasError = !string.IsNullOrEmpty(value);
            }
        }
    }

    /// <summary>
    /// Clears any error state.
    /// </summary>
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    /// <summary>
    /// Sets an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }
}
