using DatabaseBackupManager.Helpers;

namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents connection settings for a destination SQL Server.
/// </summary>
public class DestinationServerSettings : ObservableObject
{
    private string _serverName = string.Empty;
    private AuthenticationType _authenticationType = AuthenticationType.Windows;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private int _connectionTimeout = 30;
    private bool _trustServerCertificate = true;
    private string _databaseName = string.Empty;

    /// <summary>
    /// Gets or sets the destination SQL Server name.
    /// </summary>
    public string ServerName
    {
        get => _serverName;
        set => SetProperty(ref _serverName, value);
    }

    /// <summary>
    /// Gets or sets the authentication type for the destination server.
    /// </summary>
    public AuthenticationType AuthenticationType
    {
        get => _authenticationType;
        set => SetProperty(ref _authenticationType, value);
    }

    /// <summary>
    /// Gets or sets the username for SQL Server authentication.
    /// </summary>
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    /// <summary>
    /// Gets or sets the password for SQL Server authentication.
    /// </summary>
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeout
    {
        get => _connectionTimeout;
        set => SetProperty(ref _connectionTimeout, value);
    }

    /// <summary>
    /// Gets or sets whether to trust the server certificate.
    /// </summary>
    public bool TrustServerCertificate
    {
        get => _trustServerCertificate;
        set => SetProperty(ref _trustServerCertificate, value);
    }

    /// <summary>
    /// Gets or sets the destination database name.
    /// </summary>
    public string DatabaseName
    {
        get => _databaseName;
        set => SetProperty(ref _databaseName, value);
    }

    /// <summary>
    /// Builds a connection string for the destination server.
    /// </summary>
    public string BuildConnectionString()
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
        {
            DataSource = ServerName,
            ConnectTimeout = ConnectionTimeout,
            TrustServerCertificate = TrustServerCertificate
        };

        if (AuthenticationType == AuthenticationType.Windows)
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            builder.IntegratedSecurity = false;
            builder.UserID = Username;
            builder.Password = Password;
        }

        if (!string.IsNullOrWhiteSpace(DatabaseName))
        {
            builder.InitialCatalog = DatabaseName;
        }

        return builder.ConnectionString;
    }
}
