using DatabaseBackupManager.Models;

namespace DatabaseBackupManager.Services;

/// <summary>
/// Interface for SQL Server database operations.
/// </summary>
public interface ISqlServerService
{
    /// <summary>
    /// Tests the connection to the SQL Server.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if connection is successful, false otherwise.</returns>
    Task<bool> TestConnectionAsync(ConnectionSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all databases on the server.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of database information.</returns>
    Task<IList<DatabaseInfo>> GetDatabasesAsync(ConnectionSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a database backup.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="databaseName">The name of the database to backup.</param>
    /// <param name="backupPath">The full path for the backup file.</param>
    /// <param name="backupType">The type of backup to perform.</param>
    /// <param name="compress">Whether to compress the backup.</param>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task BackupDatabaseAsync(
        ConnectionSettings settings,
        string databaseName,
        string backupPath,
        BackupType backupType,
        bool compress,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a database from a backup file.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="databaseName">The name of the database to restore.</param>
    /// <param name="backupPath">The full path to the backup file.</param>
    /// <param name="dataFilePath">Optional path for the data file.</param>
    /// <param name="logFilePath">Optional path for the log file.</param>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RestoreDatabaseAsync(
        ConnectionSettings settings,
        string databaseName,
        string backupPath,
        string? dataFilePath = null,
        string? logFilePath = null,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the file list from a backup file.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="backupPath">The full path to the backup file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of logical file names and their types.</returns>
    Task<IList<BackupFileInfo>> GetBackupFileListAsync(
        ConnectionSettings settings,
        string backupPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all tables in a specific database.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of table information.</returns>
    Task<IList<TableInfo>> GetTablesAsync(
        ConnectionSettings settings,
        string databaseName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all available SQL Server data types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of SQL Server data types.</returns>
    Task<IList<SqlDataType>> GetSqlDataTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new column to a table in the specified database.
    /// </summary>
    /// <param name="settings">The connection settings.</param>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columnName">The name of the new column.</param>
    /// <param name="dataType">The SQL Server data type.</param>
    /// <param name="length">Optional length for variable-length data types (VARCHAR, NVARCHAR, etc.).</param>
    /// <param name="precision">Optional precision for numeric types (DECIMAL, NUMERIC, FLOAT - total number of digits).</param>
    /// <param name="scale">Optional scale for decimal types (DECIMAL, NUMERIC - number of digits after decimal point).</param>
    /// <param name="isNullable">Whether the column allows NULL values.</param>
    /// <param name="isPrimaryKey">Whether the column is a primary key.</param>
    /// <param name="defaultValue">Optional default value for the column.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddColumnAsync(
        ConnectionSettings settings,
        string databaseName,
        string tableName,
        string columnName,
        string dataType,
        int? length = null,
        int? precision = null,
        int? scale = null,
        bool isNullable = true,
        bool isPrimaryKey = false,
        string? defaultValue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the connection to a destination SQL Server.
    /// </summary>
    /// <param name="destinationSettings">The destination server connection settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if connection is successful, false otherwise.</returns>
    Task<bool> TestDestinationConnectionAsync(
        DestinationServerSettings destinationSettings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all databases on the destination server.
    /// </summary>
    /// <param name="destinationSettings">The destination server connection settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of database information.</returns>
    Task<IList<DatabaseInfo>> GetDestinationDatabasesAsync(
        DestinationServerSettings destinationSettings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the row count from a source table or query.
    /// </summary>
    /// <param name="sourceSettings">The source server connection settings.</param>
    /// <param name="databaseName">The source database name.</param>
    /// <param name="tableNameOrQuery">Table name or SQL query.</param>
    /// <param name="isQuery">Whether tableNameOrQuery is a SQL query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The row count.</returns>
    Task<long> GetRowCountAsync(
        ConnectionSettings sourceSettings,
        string databaseName,
        string tableNameOrQuery,
        bool isQuery = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a preview of data from a source table or query (first N rows).
    /// </summary>
    /// <param name="sourceSettings">The source server connection settings.</param>
    /// <param name="databaseName">The source database name.</param>
    /// <param name="tableNameOrQuery">Table name or SQL query.</param>
    /// <param name="isQuery">Whether tableNameOrQuery is a SQL query.</param>
    /// <param name="maxRows">Maximum number of rows to preview.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DataTable with the preview data.</returns>
    Task<System.Data.DataTable> GetDataPreviewAsync(
        ConnectionSettings sourceSettings,
        string databaseName,
        string tableNameOrQuery,
        bool isQuery = false,
        int maxRows = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfers data from source to destination with schema validation, auto-create table, and transaction rollback.
    /// </summary>
    /// <param name="sourceSettings">The source server connection settings.</param>
    /// <param name="destinationSettings">The destination server connection settings.</param>
    /// <param name="request">The data transfer request configuration.</param>
    /// <param name="progress">Progress reporter for transfer progress.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TransferDataAsync(
        ConnectionSettings sourceSettings,
        DestinationServerSettings destinationSettings,
        DataTransferRequest request,
        IProgress<TransferProgress>? progress = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a file within a backup.
/// </summary>
public class BackupFileInfo
{
    public string LogicalName { get; set; } = string.Empty;
    public string PhysicalName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // D = Data, L = Log
    public long SizeInBytes { get; set; }
}
