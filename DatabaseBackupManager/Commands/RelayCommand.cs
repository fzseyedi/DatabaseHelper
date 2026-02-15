using System.Windows.Input;

namespace DatabaseBackupManager.Commands;

/// <summary>
/// A command implementation that delegates execution to provided actions.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Creates a new RelayCommand with the specified execute action.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Optional predicate to determine if the command can execute.</param>
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Creates a new RelayCommand with a parameterless execute action.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Optional function to determine if the command can execute.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
        : this(_ => execute(), canExecute is null ? null : _ => canExecute())
    {
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
}

/// <summary>
/// A generic command implementation that delegates execution to provided actions.
/// </summary>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Creates a new RelayCommand with the specified execute action.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Optional predicate to determine if the command can execute.</param>
    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

    public void Execute(object? parameter) => _execute((T?)parameter);

    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
}
