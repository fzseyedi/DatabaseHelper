using System.Collections.ObjectModel;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for managing SQL Server table columns (adding new fields).
/// </summary>
public class FieldManagerViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionViewModel _connectionViewModel;
    private DatabaseInfo? _selectedDatabase;
    private TableInfo? _selectedTable;
    private SqlDataType? _selectedDataType;
    private string _fieldName = string.Empty;
    private int _fieldLength = 50;
    private int _fieldPrecision = 18;
    private int _fieldScale = 0;
    private bool _isNullable = true;
    private bool _isPrimaryKey;
    private string _defaultValue = string.Empty;
    private bool _isAddingField;

    public FieldManagerViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
    {
        _sqlServerService = sqlServerService;
        _connectionViewModel = connectionViewModel;

        Tables = new ObservableCollection<TableInfo>();
        DataTypes = new ObservableCollection<SqlDataType>();

        // Initialize commands
        LoadTablesCommand = new RelayCommand(async () => await LoadTablesAsync(), CanLoadTables);
        LoadDataTypesCommand = new RelayCommand(async () => await LoadDataTypesAsync());
        AddFieldCommand = new RelayCommand(async () => await AddFieldAsync(), CanAddField);
        ClearFormCommand = new RelayCommand(ClearForm);

        // Load data types on initialization
        _ = LoadDataTypesAsync();
    }

    /// <summary>
    /// Gets the list of databases from the connection view model.
    /// </summary>
    public ObservableCollection<DatabaseInfo> Databases => _connectionViewModel.Databases;

    /// <summary>
    /// Gets the list of tables in the selected database.
    /// </summary>
    public ObservableCollection<TableInfo> Tables { get; }

    /// <summary>
    /// Gets the list of available SQL Server data types.
    /// </summary>
    public ObservableCollection<SqlDataType> DataTypes { get; }

    /// <summary>
    /// Gets or sets the selected database.
    /// </summary>
    public DatabaseInfo? SelectedDatabase
    {
        get => _selectedDatabase;
        set
        {
            if (SetProperty(ref _selectedDatabase, value))
            {
                _ = LoadTablesAsync();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected table.
    /// </summary>
    public TableInfo? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }

    /// <summary>
    /// Gets or sets the selected data type.
    /// </summary>
    public SqlDataType? SelectedDataType
    {
        get => _selectedDataType;
        set
        {
            if (SetProperty(ref _selectedDataType, value))
            {
                // Notify that IsLengthRequired and IsPrecisionRequired have changed
                OnPropertyChanged(nameof(IsLengthRequired));
                OnPropertyChanged(nameof(IsPrecisionRequired));
            }
        }
    }

    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName
    {
        get => _fieldName;
        set => SetProperty(ref _fieldName, value);
    }

    /// <summary>
    /// Gets or sets the field length (for VARCHAR, NVARCHAR, etc.).
    /// </summary>
    public int FieldLength
    {
        get => _fieldLength;
        set => SetProperty(ref _fieldLength, value);
    }

    /// <summary>
    /// Gets or sets the field precision (for DECIMAL, NUMERIC, FLOAT).
    /// </summary>
    public int FieldPrecision
    {
        get => _fieldPrecision;
        set => SetProperty(ref _fieldPrecision, value);
    }

    /// <summary>
    /// Gets or sets the field scale (number of digits after decimal point for DECIMAL, NUMERIC).
    /// </summary>
    public int FieldScale
    {
        get => _fieldScale;
        set => SetProperty(ref _fieldScale, value);
    }

    /// <summary>
    /// Gets or sets whether the field allows NULL values.
    /// </summary>
    public bool IsNullable
    {
        get => _isNullable;
        set => SetProperty(ref _isNullable, value);
    }

    /// <summary>
    /// Gets or sets whether the field is a primary key.
    /// </summary>
    public bool IsPrimaryKey
    {
        get => _isPrimaryKey;
        set => SetProperty(ref _isPrimaryKey, value);
    }

    /// <summary>
    /// Gets or sets the default value for the field.
    /// </summary>
    public string DefaultValue
    {
        get => _defaultValue;
        set => SetProperty(ref _defaultValue, value);
    }

    /// <summary>
    /// Gets whether the selected data type requires length parameter.
    /// </summary>
    public bool IsLengthRequired => SelectedDataType?.RequiresLength ?? false;

    /// <summary>
    /// Gets whether the selected data type requires precision parameter.
    /// </summary>
    public bool IsPrecisionRequired => SelectedDataType?.RequiresPrecision ?? false;

    /// <summary>
    /// Gets whether a field is currently being added.
    /// </summary>
    public bool IsAddingField
    {
        get => _isAddingField;
        private set => SetProperty(ref _isAddingField, value);
    }

    // Commands
    public ICommand LoadTablesCommand { get; }
    public ICommand LoadDataTypesCommand { get; }
    public ICommand AddFieldCommand { get; }
    public ICommand ClearFormCommand { get; }

    private bool CanLoadTables() => SelectedDatabase != null && !IsBusy && _connectionViewModel.IsConnected;
    private bool CanAddField() =>
        SelectedDatabase != null &&
        SelectedTable != null &&
        SelectedDataType != null &&
        !string.IsNullOrWhiteSpace(FieldName) &&
        !IsAddingField &&
        !IsBusy &&
        _connectionViewModel.IsConnected;

    /// <summary>
    /// Loads tables for the selected database.
    /// </summary>
    private async Task LoadTablesAsync()
    {
        if (SelectedDatabase == null) return;

        IsBusy = true;
        ClearError();
        Tables.Clear();
        StatusMessage = "در حال بارگذاری جداول...";

        try
        {
            var tables = await _sqlServerService.GetTablesAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedDatabase.Name);

            foreach (var table in tables)
            {
                Tables.Add(table);
            }

            StatusMessage = $"{Tables.Count} جدول بارگذاری شد.";
        }
        catch (Exception ex)
        {
            SetError($"بارگذاری جداول ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads all available SQL Server data types.
    /// </summary>
    private async Task LoadDataTypesAsync()
    {
        IsBusy = true;

        try
        {
            var dataTypes = await _sqlServerService.GetSqlDataTypesAsync();

            DataTypes.Clear();
            foreach (var dataType in dataTypes)
            {
                DataTypes.Add(dataType);
            }

            StatusMessage = "انواع داده بارگذاری شد.";
        }
        catch (Exception ex)
        {
            SetError($"بارگذاری انواع داده ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Adds a new field to the selected table.
    /// </summary>
    private async Task AddFieldAsync()
    {
        if (SelectedDatabase == null || SelectedTable == null || SelectedDataType == null) return;

        IsAddingField = true;
        IsBusy = true;
        ClearError();

        try
        {
            StatusMessage = $"در حال افزودن فیلد '{FieldName}' به جدول '{SelectedTable.Name}'...";

            await _sqlServerService.AddColumnAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedDatabase.Name,
                SelectedTable.Name,
                FieldName,
                SelectedDataType.Name,
                IsLengthRequired ? FieldLength : null,
                IsPrecisionRequired ? FieldPrecision : null,
                IsPrecisionRequired ? FieldScale : null,
                IsNullable,
                IsPrimaryKey,
                !string.IsNullOrWhiteSpace(DefaultValue) ? DefaultValue : null);

            StatusMessage = $"فیلد '{FieldName}' با موفقیت به جدول '{SelectedTable.Name}' افزوده شد.";

            // Reload tables to update column count
            await LoadTablesAsync();

            // Clear the form
            ClearForm();
        }
        catch (Exception ex)
        {
            SetError($"افزودن فیلد ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsAddingField = false;
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears the form fields.
    /// </summary>
    private void ClearForm()
    {
        FieldName = string.Empty;
        FieldLength = 50;
        FieldPrecision = 18;
        FieldScale = 0;
        IsNullable = true;
        IsPrimaryKey = false;
        DefaultValue = string.Empty;
        SelectedDataType = null;
        StatusMessage = "فرم پاک شد.";
    }
}
