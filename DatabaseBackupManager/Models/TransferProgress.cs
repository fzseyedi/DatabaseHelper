namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents the progress of a data transfer operation.
/// </summary>
public class TransferProgress
{
    /// <summary>
    /// Gets or sets the total number of rows to transfer.
    /// </summary>
    public long TotalRows { get; set; }

    /// <summary>
    /// Gets or sets the number of rows transferred so far.
    /// </summary>
    public long TransferredRows { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage (0-100).
    /// </summary>
    public int ProgressPercentage => TotalRows > 0 ? (int)(TransferredRows * 100 / TotalRows) : 0;

    /// <summary>
    /// Gets or sets the current status message.
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the transfer is complete.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Gets or sets whether the transfer was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if transfer failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
