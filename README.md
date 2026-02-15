# Database Backup Manager (پشتیبانی و بازیابی اطلاعات)

A comprehensive WPF desktop application for managing SQL Server databases — backup, restore, scheduling, field management, and cross-server data transfer.

![.NET](https://img.shields.io/badge/.NET-10-blue)
![Language](https://img.shields.io/badge/Language-C%23%2014-purple)
![UI](https://img.shields.io/badge/UI-WPF-blue)
![Architecture](https://img.shields.io/badge/Architecture-MVVM-green)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 📸 Overview

**Database Backup Manager** is a Persian (RTL) WPF application designed for SQL Server database administrators and developers. It provides a unified interface for:

- 🔗 Connecting to SQL Server instances
- 💾 Backing up databases (Full / Differential / Transaction Log)
- 📥 Restoring databases from backup files
- 📅 Scheduling automated backups
- 📋 Adding new columns to existing tables
- 📤 Transferring data between SQL Server databases

---

## ✨ Features

### 🔗 Connection Management
- Windows & SQL Server authentication
- Test connection before connecting
- Persistent settings with DPAPI encryption
- Connection status indicator (green/red dot)
- Auto-load saved settings on startup

### 💾 Database Backup
- **Full Backup** — Complete database backup
- **Differential Backup** — Changes since last full backup
- **Transaction Log Backup** — Point-in-time recovery support
- Backup compression
- Real-time progress bar
- Persistent backup settings (path, compression, type)
- View last backup date per database

### 📥 Database Restore
- Restore from `.bak` or `.trn` files
- Custom data and log file paths
- View backup file contents before restoring
- Progress tracking
- Automatic database list refresh after restore

### 📅 Backup Scheduling
- Daily, Weekly, Monthly schedules
- Automatic backup execution
- Schedule persistence
- Schedule management interface

### 📋 Field Manager (Add New Columns)
- Add new columns to any existing table
- 31+ SQL Server data types supported:
  - **Numeric:** INT, BIGINT, SMALLINT, TINYINT, FLOAT, REAL, DECIMAL, NUMERIC, MONEY, SMALLMONEY
  - **String:** VARCHAR, NVARCHAR, CHAR, NCHAR, TEXT, NTEXT
  - **Date/Time:** DATE, TIME, DATETIME, DATETIME2, DATETIMEOFFSET, SMALLDATETIME
  - **Binary:** BINARY, VARBINARY, IMAGE
  - **Other:** BIT, UNIQUEIDENTIFIER, XML, JSON
- Dynamic UI: Length/Precision/Scale fields appear based on selected data type
- Configure constraints: NULL/NOT NULL, Primary Key, Default value
- Real-time table information panel

### 📤 Data Transfer (Cross-Server Copy)
A powerful 3-step wizard for transferring data between SQL Server databases:

**Step 1 — Destination Server Setup:**
- Connect to any SQL Server (same or different machine)
- Windows & SQL Server authentication
- Test connection before proceeding
- Encrypted settings persistence (DPAPI)

**Step 2 — Source Data Selection:**
- Select source database and table from connected server
- Two transfer modes:
  - **Table Mode** — Select from list of available tables
  - **Query Mode** — Write custom SQL query
- Preview first 10 rows before transfer
- View total row count

**Step 3 — Destination & Execute:**
- Select destination database
- Editable destination table name (defaults to source table name)
- **Append** — Add to existing data
- **Replace** — Delete existing data first (with warning ⚠️)
- Auto-create destination table if it doesn't exist
- Progress bar with row count
- **Transaction rollback** on failure — no partial data
- Uses SqlBulkCopy for efficient transfer

### 🌍 User Interface
- **Right-to-Left (RTL)** Persian interface (فارسی)
- Professional MVVM design with clean navigation
- Step indicator for Data Transfer wizard (1→2→3)
- Real-time status messages in Persian
- Progress indicators for long operations
- Comprehensive error handling with rollback
- Help information panels on each view

### 💾 Settings Persistence
- Connection settings — encrypted with DPAPI
- Backup settings — path, compression, type
- Destination server settings — encrypted with DPAPI
- Automatic loading on startup
- User-controlled with checkboxes
- Clear saved settings option

---

## 🛠️ System Requirements

| Requirement | Minimum |
|-------------|---------|
| **OS** | Windows 10 or later |
| **.NET** | .NET 10 SDK |
| **SQL Server** | SQL Server 2016 or later |
| **IDE** | Visual Studio 2022+ |
| **RAM** | 2 GB |
| **Display** | 1024×768 |

---

## 📦 Installation

```bash
# Clone the repository
git clone https://github.com/fzseyedi/DatabaseHelper.git

# Navigate to the project
cd DatabaseHelper

# Build the project
dotnet build

# Run the application
dotnet run --project DatabaseBackupManager
```

Or open `DatabaseBackupManager.sln` in Visual Studio and press `F5`.

---

## 🚀 Getting Started

### 1. Connect to SQL Server
1. Launch the application
2. Enter server name (e.g., `localhost\SQLEXPRESS`)
3. Select authentication type
4. Click **تست اتصال** (Test Connection)
5. Click **اتصال** (Connect)

### 2. Backup a Database
1. Click **💾 پشتیبانی** tab
2. Select database → Choose backup type → Set path
3. Click **▶ شروع پشتیبان‌گیری** (Start Backup)

### 3. Restore a Database
1. Click **📥 بازیابی** tab
2. Select database → Browse backup file
3. Click **▶ بازیابی اطلاعات** (Start Restore)

### 4. Add a New Column
1. Click **📋 افزودن فیلد جدید** tab
2. Select database → Select table
3. Enter field name, data type, constraints
4. Click **✓ افزودن فیلد** (Add Field)

### 5. Transfer Data Between Servers
1. Click **📤 انتقال داده** tab
2. **Step 1:** Enter destination server details → Connect
3. **Step 2:** Select source database → Choose table or write query → Preview data
4. **Step 3:** Select destination database → Set table name → Choose Append/Replace → Execute

---

## 🏗️ Architecture

### MVVM Pattern

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│   Views (XAML)   │────▶│   ViewModels     │────▶│   Services       │
│                  │     │   (Logic)        │     │   (Data Access)  │
│ - ConnectionView │     │ - ConnectionVM   │     │ - ISqlServerSvc  │
│ - BackupView     │     │ - BackupVM       │     │ - SqlServerSvc   │
│ - RestoreView    │     │ - RestoreVM      │     │ - SettingsManager│
│ - ScheduleView   │     │ - ScheduleVM     │     │                  │
│ - FieldMgrView   │     │ - FieldMgrVM     │     ├──────────────────┤
│ - DataTransView  │     │ - DataTransVM    │     │   Models         │
└──────────────────┘     └──────────────────┘     │ - ConnectionSettings
                                                  │ - DatabaseInfo   │
                                                  │ - TableInfo      │
                                                  │ - SqlDataType    │
                                                  │ - DataTransReq   │
                                                  │ - TransferProgress
                                                  └──────────────────┘
```

### Project Structure

```
DatabaseBackupManager/
├── Models/
│   ├── ConnectionSettings.cs        # Source server connection
│   ├── DatabaseInfo.cs              # Database metadata
│   ├── BackupSchedule.cs            # Schedule configuration
│   ├── TableInfo.cs                 # Table metadata
│   ├── SqlDataType.cs               # SQL Server data types
│   ├── DestinationServerSettings.cs # Destination server connection
│   ├── DataTransferRequest.cs       # Transfer configuration
│   └── TransferProgress.cs          # Transfer progress tracking
├── Services/
│   ├── ISqlServerService.cs         # Service interface
│   ├── SqlServerService.cs          # SQL Server operations
│   ├── ConnectionSettingsManager.cs # Encrypted source settings
│   ├── BackupSettingsManager.cs     # Backup preferences
│   └── DestinationSettingsManager.cs# Encrypted destination settings
├── ViewModels/
│   ├── ViewModelBase.cs             # Base class (INotifyPropertyChanged)
│   ├── MainViewModel.cs             # Navigation & orchestration
│   ├── ConnectionViewModel.cs       # Connection management
│   ├── BackupViewModel.cs           # Backup operations
│   ├── RestoreViewModel.cs          # Restore operations
│   ├── ScheduleViewModel.cs         # Scheduling
│   ├── FieldManagerViewModel.cs     # Column management
│   └── DataTransferViewModel.cs     # 3-step data transfer wizard
├── Views/
│   ├── ConnectionView.xaml          # Connection UI
│   ├── BackupView.xaml              # Backup UI
│   ├── RestoreView.xaml             # Restore UI
│   ├── ScheduleView.xaml            # Schedule UI
│   ├── FieldManagerView.xaml        # Field manager UI
│   └── DataTransferView.xaml        # Data transfer wizard UI
├── Commands/
│   └── RelayCommand.cs              # ICommand implementation
├── Helpers/
│   └── ObservableObject.cs          # Property change notification
├── MainWindow.xaml                  # Main window with navigation
└── App.xaml                         # Application entry point
```

---

## 🔐 Security

- **DPAPI Encryption** — All passwords encrypted using Windows Data Protection API
- **User-Scoped** — Settings encrypted per Windows user account
- **Local Storage** — No cloud storage; all data stays on machine
- **Parameterized Queries** — SQL injection prevention
- **Transaction Rollback** — Data integrity on transfer failure

### Settings Storage Location
```
%LOCALAPPDATA%\DatabaseBackupManager\
├── connection-settings.json      # Encrypted source server settings
├── destination-settings.json     # Encrypted destination server settings
└── backup-settings.json          # Backup preferences
```

---

## 📋 Data Transfer — Technical Details

The Data Transfer feature uses `SqlBulkCopy` for high-performance data movement:

| Feature | Implementation |
|---------|---------------|
| **Bulk Copy** | SqlBulkCopy with batch size 1000 |
| **Progress** | NotifyAfter for real-time row count |
| **Schema** | Auto-detect from source and create destination table |
| **Validation** | INFORMATION_SCHEMA queries |
| **Rollback** | SQL Transaction wraps entire operation |
| **Replace Mode** | DELETE + INSERT within same transaction |
| **Type Mapping** | Automatic SQL type mapping with precision/scale |

### Transfer Flow
```
Source Server                          Destination Server
┌──────────────┐                      ┌──────────────┐
│ SELECT * FROM│──── SqlBulkCopy ────▶│ INSERT INTO  │
│ [SourceTable]│     (batched)        │ [DestTable]  │
└──────────────┘                      └──────────────┘
       │                                      │
       └──── Transaction (Commit/Rollback) ───┘
```

---

## 📋 NuGet Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.Data.SqlClient` | SQL Server connectivity |
| `Microsoft.Extensions.DependencyInjection` | Dependency injection |
| `System.Security.Cryptography.ProtectedData` | DPAPI encryption |

---

## 🚧 Future Enhancements

- [ ] English language support
- [ ] Background Windows Service for scheduled backups
- [ ] Multi-table transfer in single operation
- [ ] Query builder for complex operations
- [ ] Database comparison tools
- [ ] Performance analysis dashboard
- [ ] Export/Import to CSV/Excel
- [ ] Multi-server management
- [ ] Backup history tracking & reports
- [ ] Dark theme support

---

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📜 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Farzin Seyedi**

[![GitHub](https://img.shields.io/badge/GitHub-fzseyedi-181717?logo=github)](https://github.com/fzseyedi)

---

## 📚 Resources

- [SQL Server Documentation](https://learn.microsoft.com/en-us/sql/sql-server/)
- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [SqlBulkCopy Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlbulkcopy)

---

**Version:** 1.1.0 · **Status:** Active Development ✅ · **Last Updated:** 2025
