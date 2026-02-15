namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents a data transfer request from source to destination.
/// </summary>
public class DataTransferRequest
{
    /// <summary>
    /// Gets or sets the source database name.
    /// </summary>
    public string SourceDatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transfer mode (Table or Query).
    /// </summary>
    public DataTransferMode TransferMode { get; set; } = DataTransferMode.Table;

    /// <summary>
    /// Gets or sets the source table name (when TransferMode is Table).
    /// </summary>
    public string SourceTableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the custom SQL query (when TransferMode is Query).
    /// </summary>
    public string CustomQuery { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination table name.
    /// </summary>
    public string DestinationTableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets how to handle existing data in destination table.
    /// </summary>
    public DataTransferAction TransferAction { get; set; } = DataTransferAction.Append;

    /// <summary>
    /// Gets or sets the destination database name.
    /// </summary>
    public string DestinationDatabaseName { get; set; } = string.Empty;
}

/// <summary>
/// Specifies the data transfer mode.
/// </summary>
public enum DataTransferMode
{
    /// <summary>
    /// Copy data from a specific table.
    /// </summary>
    Table,

    /// <summary>
    /// Copy data using a custom SQL query.
    /// </summary>
    Query
}

/// <summary>
/// Specifies how to handle existing data in the destination table.
/// </summary>
public enum DataTransferAction
{
    /// <summary>
    /// Append new data to existing data.
    /// </summary>
    Append,

    /// <summary>
    /// Replace existing data with new data.
    /// </summary>
    Replace
}
