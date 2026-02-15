using System.IO;
using System.Text.Json;

namespace DatabaseBackupManager.Services;

/// <summary>
/// Manages persistent backup settings using JSON storage.
/// </summary>
public class BackupSettingsManager
{
    private readonly string _settingsFilePath;
    private const string SettingsFileName = "backup-settings.json";

    public BackupSettingsManager()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "DatabaseBackupManager");
        Directory.CreateDirectory(appFolder);
        _settingsFilePath = Path.Combine(appFolder, SettingsFileName);
    }

    /// <summary>
    /// Saves backup settings to JSON file.
    /// </summary>
    public async Task SaveBackupSettingsAsync(string backupPath, bool compress, string backupType)
    {
        try
        {
            var settingsData = new SavedBackupSettings
            {
                BackupPath = backupPath,
                CompressBackup = compress,
                SelectedBackupType = backupType,
                SavedDate = DateTime.Now
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settingsData, options);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در ذخیره تنظیمات پشتیبان‌گیری: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads backup settings from JSON file if it exists.
    /// </summary>
    public async Task<SavedBackupSettings?> LoadBackupSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
                return null;

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var settingsData = JsonSerializer.Deserialize<SavedBackupSettings>(json);

            return settingsData;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در بارگذاری تنظیمات پشتیبان‌گیری: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deletes saved backup settings.
    /// </summary>
    public void DeleteBackupSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
                File.Delete(_settingsFilePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در حذف تنظیمات پشتیبان‌گیری: {ex.Message}");
        }
    }
}

/// <summary>
/// Represents saved backup settings in JSON format.
/// </summary>
public class SavedBackupSettings
{
    public string BackupPath { get; set; } = string.Empty;
    public bool CompressBackup { get; set; }
    public string SelectedBackupType { get; set; } = "Full";
    public DateTime SavedDate { get; set; }
}
