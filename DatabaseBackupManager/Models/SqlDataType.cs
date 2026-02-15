namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents a SQL Server data type.
/// </summary>
public class SqlDataType
{
    /// <summary>
    /// Gets or sets the name of the data type (e.g., VARCHAR, INT, DECIMAL).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this data type requires a length parameter.
    /// For example: VARCHAR, NVARCHAR, CHAR, NCHAR, BINARY, VARBINARY.
    /// </summary>
    public bool RequiresLength { get; set; }

    /// <summary>
    /// Gets or sets whether this data type requires precision and scale parameters.
    /// For example: DECIMAL, NUMERIC, FLOAT, REAL.
    /// </summary>
    public bool RequiresPrecision { get; set; }

    /// <summary>
    /// Gets or sets an optional description of the data type.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Returns the data type name for display.
    /// </summary>
    public override string ToString() => Name;
}
