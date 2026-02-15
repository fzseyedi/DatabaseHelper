using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatabaseBackupManager.Models;

namespace DatabaseBackupManager.Services;

/// <summary>
/// Manages persistent destination server settings using encrypted JSON storage.
/// </summary>
public class DestinationSettingsManager
{
    private readonly string _settingsFilePath;
    private const string SettingsFileName = "destination-settings.json";

    public DestinationSettingsManager()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "DatabaseBackupManager");
        Directory.CreateDirectory(appFolder);
        _settingsFilePath = Path.Combine(appFolder, SettingsFileName);
    }

    /// <summary>
    /// Saves destination server settings to encrypted JSON file.
    /// </summary>
    public async Task SaveDestinationSettingsAsync(DestinationServerSettings settings)
    {
        try
        {
            var settingsData = new SavedDestinationSettings
            {
                ServerName = settings.ServerName,
                AuthenticationType = settings.AuthenticationType,
                Username = settings.Username,
                EncryptedPassword = EncryptPassword(settings.Password),
                ConnectionTimeout = settings.ConnectionTimeout,
                TrustServerCertificate = settings.TrustServerCertificate,
                DatabaseName = settings.DatabaseName,
                SavedDate = DateTime.Now
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settingsData, options);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در ذخیره تنظیمات مقصد: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads destination server settings from encrypted JSON file if it exists.
    /// </summary>
    public async Task<DestinationServerSettings?> LoadDestinationSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
                return null;

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var settingsData = JsonSerializer.Deserialize<SavedDestinationSettings>(json);

            if (settingsData == null)
                return null;

            return new DestinationServerSettings
            {
                ServerName = settingsData.ServerName,
                AuthenticationType = settingsData.AuthenticationType,
                Username = settingsData.Username,
                Password = DecryptPassword(settingsData.EncryptedPassword),
                ConnectionTimeout = settingsData.ConnectionTimeout,
                TrustServerCertificate = settingsData.TrustServerCertificate,
                DatabaseName = settingsData.DatabaseName
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در بارگذاری تنظیمات مقصد: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deletes saved destination server settings.
    /// </summary>
    public void DeleteDestinationSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
                File.Delete(_settingsFilePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در حذف تنظیمات مقصد: {ex.Message}");
        }
    }

    /// <summary>
    /// Encrypts password using DPAPI (Data Protection API).
    /// </summary>
    private static string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        try
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var encryptedBytes = ProtectedData.Protect(passwordBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }
        catch
        {
            return password;
        }
    }

    /// <summary>
    /// Decrypts password using DPAPI.
    /// </summary>
    private static string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            return string.Empty;

        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);
            var passwordBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(passwordBytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}

/// <summary>
/// Represents saved destination server settings in JSON format.
/// </summary>
public class SavedDestinationSettings
{
    public string ServerName { get; set; } = string.Empty;
    public AuthenticationType AuthenticationType { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public int ConnectionTimeout { get; set; }
    public bool TrustServerCertificate { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public DateTime SavedDate { get; set; }
}
