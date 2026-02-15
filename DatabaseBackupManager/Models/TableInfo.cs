using DatabaseBackupManager.Helpers;

namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents a table in a SQL Server database.
/// </summary>
public class TableInfo : ObservableObject
{
    private string _name = string.Empty;
    private string _schema = "dbo";
    private int _columnCount;

    /// <summary>
    /// Gets or sets the table name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the schema name.
    /// </summary>
    public string Schema
    {
        get => _schema;
        set => SetProperty(ref _schema, value);
    }

    /// <summary>
    /// Gets or sets the number of columns in the table.
    /// </summary>
    public int ColumnCount
    {
        get => _columnCount;
        set => SetProperty(ref _columnCount, value);
    }

    /// <summary>
    /// Gets the full table name including schema.
    /// </summary>
    public string FullName => $"{Schema}.{Name}";

    /// <summary>
    /// Returns the table name for display.
    /// </summary>
    public override string ToString() => Name;
}
