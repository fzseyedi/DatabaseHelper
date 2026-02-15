using DatabaseBackupManager.Helpers;
using Microsoft.Data.SqlClient;

namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents SQL Server connection settings.
/// </summary>
public class ConnectionSettings : ObservableObject
{
    private string _serverName = string.Empty;
    private AuthenticationType _authenticationType = AuthenticationType.Windows;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private int _connectionTimeout = 30;
    private bool _trustServerCertificate = true;

    /// <summary>
    /// Gets or sets the SQL Server instance name.
    /// </summary>
    public string ServerName
    {
        get => _serverName;
        set => SetProperty(ref _serverName, value);
    }

    /// <summary>
    /// Gets or sets the authentication type.
    /// </summary>
    public AuthenticationType AuthenticationType
    {
        get => _authenticationType;
        set => SetProperty(ref _authenticationType, value);
    }

    /// <summary>
    /// Gets or sets the SQL Server username (for SQL Authentication).
    /// </summary>
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    /// <summary>
    /// Gets or sets the SQL Server password (for SQL Authentication).
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
    /// Builds the connection string based on current settings.
    /// </summary>
    public string BuildConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = ServerName,
            IntegratedSecurity = AuthenticationType == AuthenticationType.Windows,
            ConnectTimeout = ConnectionTimeout,
            TrustServerCertificate = TrustServerCertificate
        };

        if (AuthenticationType == AuthenticationType.SqlServer)
        {
            builder.UserID = Username;
            builder.Password = Password;
        }

        return builder.ConnectionString;
    }
}

/// <summary>
/// SQL Server authentication types.
/// </summary>
public enum AuthenticationType
{
    /// <summary>
    /// Windows Authentication (Integrated Security).
    /// </summary>
    Windows,

    /// <summary>
    /// SQL Server Authentication (Username/Password).
    /// </summary>
    SqlServer
}
