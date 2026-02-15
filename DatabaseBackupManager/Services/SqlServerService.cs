using System.IO;
using DatabaseBackupManager.Models;
using Microsoft.Data.SqlClient;

namespace DatabaseBackupManager.Services;

/// <summary>
/// Implementation of SQL Server database operations.
/// </summary>
public class SqlServerService : ISqlServerService
{
    /// <inheritdoc />
    public async Task<bool> TestConnectionAsync(ConnectionSettings settings, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(settings.BuildConnectionString());
            await connection.OpenAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<IList<DatabaseInfo>> GetDatabasesAsync(ConnectionSettings settings, CancellationToken cancellationToken = default)
    {
        var databases = new List<DatabaseInfo>();

        const string query = @"
            SELECT 
                d.name,
                d.state_desc,
                d.recovery_model_desc,
                CAST(SUM(mf.size) * 8 / 1024 AS BIGINT) AS SizeInMB,
                MAX(b.backup_finish_date) AS LastBackupDate
            FROM sys.databases d
            LEFT JOIN sys.master_files mf ON d.database_id = mf.database_id
            LEFT JOIN msdb.dbo.backupset b ON d.name = b.database_name AND b.type = 'D'
            WHERE d.database_id > 4  -- Exclude system databases
            GROUP BY d.name, d.state_desc, d.recovery_model_desc
            ORDER BY d.name";

        await using var connection = new SqlConnection(settings.BuildConnectionString());
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            databases.Add(new DatabaseInfo
            {
                Name = reader.GetString(0),
                Status = reader.GetString(1),
                RecoveryModel = reader.GetString(2),
                SizeInMB = reader.IsDBNull(3) ? 0 : reader.GetInt64(3),
                LastBackupDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
            });
        }

        return databases;
    }

    /// <inheritdoc />
    public async Task BackupDatabaseAsync(
        ConnectionSettings settings,
        string databaseName,
        string backupPath,
        BackupType backupType,
        bool compress,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var backupTypeClause = backupType switch
        {
            BackupType.Full => "DATABASE",
            BackupType.Differential => "DATABASE",
            BackupType.TransactionLog => "LOG",
            _ => "DATABASE"
        };

        var withClauses = new List<string> { "INIT", "STATS = 1" };
        
        if (backupType == BackupType.Differential)
            withClauses.Add("DIFFERENTIAL");
        
        if (compress)
            withClauses.Add("COMPRESSION");

        var query = $@"
            BACKUP {backupTypeClause} [{databaseName}] 
            TO DISK = @BackupPath 
            WITH {string.Join(", ", withClauses)}";

        await using var connection = new SqlConnection(settings.BuildConnectionString());
        
        // Subscribe to info messages for progress
        connection.InfoMessage += (sender, e) =>
        {
            foreach (SqlError error in e.Errors)
            {
                if (error.Message.Contains("percent processed"))
                {
                    var percentText = error.Message.Split(' ')[0];
                    if (int.TryParse(percentText, out var percent))
                    {
                        progress?.Report(percent);
                    }
                }
            }
        };

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection)
        {
            CommandTimeout = 0 // No timeout for backup operations
        };
        command.Parameters.AddWithValue("@BackupPath", backupPath);

        await command.ExecuteNonQueryAsync(cancellationToken);
        progress?.Report(100);
    }

    /// <inheritdoc />
    public async Task RestoreDatabaseAsync(
        ConnectionSettings settings,
        string databaseName,
        string backupPath,
        string? dataFilePath = null,
        string? logFilePath = null,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // First, get the file list from the backup
        var fileList = await GetBackupFileListAsync(settings, backupPath, cancellationToken);
        
        var moveStatements = new List<string>();
        
        foreach (var file in fileList)
        {
            string newPath;
            if (file.Type == "D" && !string.IsNullOrEmpty(dataFilePath))
            {
                newPath = Path.Combine(dataFilePath, $"{databaseName}.mdf");
            }
            else if (file.Type == "L" && !string.IsNullOrEmpty(logFilePath))
            {
                newPath = Path.Combine(logFilePath, $"{databaseName}_log.ldf");
            }
            else
            {
                // Use original path with new database name
                var directory = Path.GetDirectoryName(file.PhysicalName) ?? "";
                var extension = Path.GetExtension(file.PhysicalName);
                newPath = Path.Combine(directory, $"{databaseName}{(file.Type == "L" ? "_log" : "")}{extension}");
            }
            
            moveStatements.Add($"MOVE '{file.LogicalName}' TO '{newPath}'");
        }

        // Set database to single user mode to kill existing connections
        var setSingleUserQuery = $@"
            IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
            END";

        var restoreQuery = $@"
            RESTORE DATABASE [{databaseName}]
            FROM DISK = @BackupPath
            WITH REPLACE, STATS = 1{(moveStatements.Count > 0 ? ", " + string.Join(", ", moveStatements) : "")}";

        var setMultiUserQuery = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";

        await using var connection = new SqlConnection(settings.BuildConnectionString());

        connection.InfoMessage += (sender, e) =>
        {
            foreach (SqlError error in e.Errors)
            {
                if (error.Message.Contains("percent processed"))
                {
                    var percentText = error.Message.Split(' ')[0];
                    if (int.TryParse(percentText, out var percent))
                    {
                        progress?.Report(percent);
                    }
                }
            }
        };

        await connection.OpenAsync(cancellationToken);

        // Set single user mode
        await using (var command = new SqlCommand(setSingleUserQuery, connection))
        {
            command.CommandTimeout = 60;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        // Perform restore
        await using (var command = new SqlCommand(restoreQuery, connection))
        {
            command.CommandTimeout = 0;
            command.Parameters.AddWithValue("@BackupPath", backupPath);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        // Set multi-user mode
        await using (var command = new SqlCommand(setMultiUserQuery, connection))
        {
            command.CommandTimeout = 60;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        progress?.Report(100);
    }

    /// <inheritdoc />
    public async Task<IList<BackupFileInfo>> GetBackupFileListAsync(
        ConnectionSettings settings,
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        var files = new List<BackupFileInfo>();

        const string query = "RESTORE FILELISTONLY FROM DISK = @BackupPath";

        await using var connection = new SqlConnection(settings.BuildConnectionString());
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@BackupPath", backupPath);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            files.Add(new BackupFileInfo
            {
                LogicalName = reader["LogicalName"]?.ToString() ?? string.Empty,
                PhysicalName = reader["PhysicalName"]?.ToString() ?? string.Empty,
                Type = reader["Type"]?.ToString() ?? string.Empty,
                SizeInBytes = reader["Size"] is long size ? size : 0
            });
        }

        return files;
    }

    /// <inheritdoc />
    public async Task<IList<TableInfo>> GetTablesAsync(
        ConnectionSettings settings,
        string databaseName,
        CancellationToken cancellationToken = default)
    {
        var tables = new List<TableInfo>();

        const string query = @"
            SELECT 
                t.TABLE_SCHEMA,
                t.TABLE_NAME,
                COUNT(c.COLUMN_NAME) AS ColumnCount
            FROM INFORMATION_SCHEMA.TABLES t
            LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
            WHERE t.TABLE_TYPE = 'BASE TABLE'
            GROUP BY t.TABLE_SCHEMA, t.TABLE_NAME
            ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME";

        // Build connection string with the specified database
        var connectionString = settings.BuildConnectionString();
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = databaseName
        };

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            tables.Add(new TableInfo
            {
                Name = reader["TABLE_NAME"]?.ToString() ?? string.Empty,
                Schema = reader["TABLE_SCHEMA"]?.ToString() ?? "dbo",
                ColumnCount = reader["ColumnCount"] is int count ? count : 0
            });
        }

        return tables;
    }

    /// <inheritdoc />
    public async Task<IList<SqlDataType>> GetSqlDataTypesAsync(CancellationToken cancellationToken = default)
    {
        var dataTypes = new List<SqlDataType>
        {
            // Exact numerics
            new SqlDataType { Name = "INT", RequiresLength = false, RequiresPrecision = false, Description = "Integer" },
            new SqlDataType { Name = "BIGINT", RequiresLength = false, RequiresPrecision = false, Description = "Big Integer" },
            new SqlDataType { Name = "SMALLINT", RequiresLength = false, RequiresPrecision = false, Description = "Small Integer" },
            new SqlDataType { Name = "TINYINT", RequiresLength = false, RequiresPrecision = false, Description = "Tiny Integer" },

            // Approximate numerics
            new SqlDataType { Name = "FLOAT", RequiresLength = false, RequiresPrecision = true, Description = "Float" },
            new SqlDataType { Name = "REAL", RequiresLength = false, RequiresPrecision = true, Description = "Real" },

            // Decimal
            new SqlDataType { Name = "DECIMAL", RequiresLength = false, RequiresPrecision = true, Description = "Decimal" },
            new SqlDataType { Name = "NUMERIC", RequiresLength = false, RequiresPrecision = true, Description = "Numeric" },

            // Monetary
            new SqlDataType { Name = "MONEY", RequiresLength = false, RequiresPrecision = false, Description = "Money" },
            new SqlDataType { Name = "SMALLMONEY", RequiresLength = false, RequiresPrecision = false, Description = "Small Money" },

            // Date and Time
            new SqlDataType { Name = "DATE", RequiresLength = false, RequiresPrecision = false, Description = "Date" },
            new SqlDataType { Name = "TIME", RequiresLength = false, RequiresPrecision = false, Description = "Time" },
            new SqlDataType { Name = "DATETIME", RequiresLength = false, RequiresPrecision = false, Description = "DateTime" },
            new SqlDataType { Name = "DATETIME2", RequiresLength = false, RequiresPrecision = false, Description = "DateTime2" },
            new SqlDataType { Name = "DATETIMEOFFSET", RequiresLength = false, RequiresPrecision = false, Description = "DateTimeOffset" },
            new SqlDataType { Name = "SMALLDATETIME", RequiresLength = false, RequiresPrecision = false, Description = "Small DateTime" },

            // Character strings
            new SqlDataType { Name = "CHAR", RequiresLength = true, RequiresPrecision = false, Description = "Character" },
            new SqlDataType { Name = "VARCHAR", RequiresLength = true, RequiresPrecision = false, Description = "Variable Character" },
            new SqlDataType { Name = "TEXT", RequiresLength = false, RequiresPrecision = false, Description = "Text" },

            // Unicode character strings
            new SqlDataType { Name = "NCHAR", RequiresLength = true, RequiresPrecision = false, Description = "Unicode Character" },
            new SqlDataType { Name = "NVARCHAR", RequiresLength = true, RequiresPrecision = false, Description = "Unicode Variable Character" },
            new SqlDataType { Name = "NTEXT", RequiresLength = false, RequiresPrecision = false, Description = "Unicode Text" },

            // Binary
            new SqlDataType { Name = "BINARY", RequiresLength = true, RequiresPrecision = false, Description = "Binary" },
            new SqlDataType { Name = "VARBINARY", RequiresLength = true, RequiresPrecision = false, Description = "Variable Binary" },
            new SqlDataType { Name = "IMAGE", RequiresLength = false, RequiresPrecision = false, Description = "Image" },

            // Other
            new SqlDataType { Name = "BIT", RequiresLength = false, RequiresPrecision = false, Description = "Bit" },
            new SqlDataType { Name = "UNIQUEIDENTIFIER", RequiresLength = false, RequiresPrecision = false, Description = "Unique Identifier" },
            new SqlDataType { Name = "XML", RequiresLength = false, RequiresPrecision = false, Description = "XML" },
            new SqlDataType { Name = "JSON", RequiresLength = false, RequiresPrecision = false, Description = "JSON" }
        };

        return await Task.FromResult(dataTypes);
    }

    /// <inheritdoc />
    public async Task AddColumnAsync(
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
        CancellationToken cancellationToken = default)
    {
        // Build the data type string
        var dataTypeString = dataType;
        if (length.HasValue && (dataType == "VARCHAR" || dataType == "NVARCHAR" || dataType == "CHAR" || dataType == "NCHAR" || dataType == "BINARY" || dataType == "VARBINARY"))
        {
            dataTypeString = $"{dataType}({length})";
        }
        else if ((dataType == "DECIMAL" || dataType == "NUMERIC") && precision.HasValue)
        {
            // For DECIMAL and NUMERIC, use both precision and scale
            var scaleValue = scale ?? 0;
            dataTypeString = $"{dataType}({precision},{scaleValue})";
        }
        else if (dataType == "FLOAT" && precision.HasValue)
        {
            dataTypeString = $"{dataType}({precision})";
        }

        // Build the nullable clause
        var nullableClause = isNullable ? "NULL" : "NOT NULL";

        // Build the default value clause
        var defaultClause = !string.IsNullOrWhiteSpace(defaultValue) ? $"DEFAULT {defaultValue}" : string.Empty;

        // Build the ALTER TABLE query
        var query = $"ALTER TABLE [{tableName}] ADD [{columnName}] {dataTypeString} {nullableClause}";
        if (!string.IsNullOrWhiteSpace(defaultClause))
        {
            query += $" {defaultClause}";
        }

        // Build connection string with the specified database
        var connectionString = settings.BuildConnectionString();
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = databaseName
        };

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection)
        {
            CommandTimeout = 60
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
