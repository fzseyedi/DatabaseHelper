# Database Backup Manager (پشتیبانی و بازیابی اطلاعات)

A comprehensive WPF application for managing SQL Server databases with backup, restore, scheduling, and field management capabilities.

![.NET](https://img.shields.io/badge/.NET-10-blue)
![Language](https://img.shields.io/badge/Language-C%23-purple)
![UI](https://img.shields.io/badge/UI-WPF-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 🎯 Features

### 🔗 Connection Management
- Connect to SQL Server with Windows/SQL Authentication
- Test connection before proceeding
- Persistent connection settings with encryption
- Automatic connection retry on startup

### 💾 Database Backup
- **Full Backup** - Complete database backup
- **Differential Backup** - Changes since last full backup
- **Transaction Log Backup** - Point-in-time recovery
- Backup compression support
- Real-time progress reporting
- Automatic backup path remembering
- View last backup date for each database

### 📥 Database Restore
- Restore from .bak or .trn files
- Custom data and log file paths
- View backup file contents
- Progress tracking
- Automatic database list refresh

### 📅 Backup Scheduling
- Daily, Weekly, Monthly schedules
- Automatic backup execution
- Schedule persistence
- Schedule management interface

### 📋 Field Manager (Add New Columns)
- Add new columns to existing tables
- Support for 31+ SQL Server data types
- Configure column properties:
  - Field name and data type
  - Length (for VARCHAR, NVARCHAR, etc.)
  - Precision & Scale (for DECIMAL, NUMERIC)
  - NULL/NOT NULL constraints
  - Primary Key designation
  - Default values
- Dynamic UI based on selected data type
- Real-time table and column information

### 🌍 User Interface
- **Right-to-Left (RTL)** Persian interface (فارسی)
- Clean, professional MVVM design
- Real-time status messages
- Progress indicators for long operations
- Responsive error handling
- Help information panels

### 💾 Settings Persistence
- Connection settings encrypted with DPAPI
- Backup settings saved (path, compression, type)
- Automatic settings loading on startup
- User-controlled persistence with checkboxes
- Clear saved settings option

## 🛠️ System Requirements

- **OS:** Windows 10 or later
- **.NET:** .NET 10
- **SQL Server:** SQL Server 2016 or later
- **RAM:** 2 GB minimum
- **Display:** 1024x768 minimum resolution

## 📦 Installation

### From GitHub

```bash
git clone https://github.com/fzseyedi/DatabaseHelper.git
cd DatabaseHelper
dotnet build
dotnet run
```

### Requirements

- Visual Studio 2022 or later
- .NET 10 SDK
- SQL Server 2016+

## 🚀 Getting Started

### 1. Initial Setup

1. Launch the application
2. Click **🔗 ارتباط با سرور** (Connection) tab
3. Enter SQL Server details:
   - Server name (e.g., `localhost\SQLEXPRESS`)
   - Authentication type (Windows/SQL)
   - Optional: Username/Password
   - Connection timeout
   - Trust server certificate option
4. Click **تست اتصال** (Test Connection)
5. Click **اتصال** (Connect)

### 2. Backup Database

1. Click **💾 پشتیبانی** (Backup) tab
2. Select database and table
3. Choose backup type:
   - Full
   - Differential  
   - Transaction Log
4. Set backup path
5. Enable compression if desired
6. Check **یادآوری تنظیمات** (Remember settings)
7. Click **▶ شروع پشتیبان‌گیری** (Start Backup)

### 3. Restore Database

1. Click **📥 بازیابی** (Restore) tab
2. Select database
3. Choose backup file
4. Optionally specify data/log file paths
5. Click **▶ بازیابی اطلاعات** (Restore)

### 4. Schedule Backup

1. Click **📅 زمانبندی** (Schedule) tab
2. Select database
3. Choose schedule frequency
4. Set backup location
5. Click **💾 ذخیره** (Save)

### 5. Add New Column

1. Click **📋 افزودن فیلد جدید** (Add Field) tab
2. Select database → Table
3. Enter field configuration:
   - Field name
   - Data type
   - Length/Precision/Scale (if applicable)
   - Constraints
4. Click **✓ افزودن فیلد** (Add Field)

## 🏗️ Architecture

### MVVM Pattern
```
Views (XAML)
    ↓
ViewModels (Logic)
    ↓
Services (Data Access)
    ↓
Models (Data)
```

### Key Components

| Component | Purpose |
|-----------|---------|
| **ISqlServerService** | SQL Server database operations |
| **ConnectionSettingsManager** | Persistent connection settings |
| **BackupSettingsManager** | Persistent backup settings |
| **ViewModelBase** | Base ViewModel with MVVM support |
| **RelayCommand** | Command implementation for buttons |

### Project Structure

```
DatabaseBackupManager/
├── Models/
│   ├── ConnectionSettings.cs
│   ├── DatabaseInfo.cs
│   ├── BackupSchedule.cs
│   ├── TableInfo.cs
│   └── SqlDataType.cs
├── Services/
│   ├── ISqlServerService.cs
│   ├── SqlServerService.cs
│   ├── ConnectionSettingsManager.cs
│   └── BackupSettingsManager.cs
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── ConnectionViewModel.cs
│   ├── BackupViewModel.cs
│   ├── RestoreViewModel.cs
│   ├── ScheduleViewModel.cs
│   └── FieldManagerViewModel.cs
├── Views/
│   ├── MainWindow.xaml
│   ├── ConnectionView.xaml
│   ├── BackupView.xaml
│   ├── RestoreView.xaml
│   ├── ScheduleView.xaml
│   └── FieldManagerView.xaml
└── Helpers/
    └── ObservableObject.cs
```

## 🔐 Security Features

- **Password Encryption:** DPAPI (Data Protection API) for sensitive data
- **Connection String Security:** Encrypted in local storage
- **No Cloud Storage:** All data remains local
- **User-Scoped:** Settings encrypted per Windows user
- **SQL Injection Prevention:** Parameterized queries

## 🎨 Supported Data Types

The Field Manager supports all major SQL Server data types:

### Numeric Types
- INT, BIGINT, SMALLINT, TINYINT
- FLOAT, REAL
- DECIMAL, NUMERIC (with Precision/Scale)
- MONEY, SMALLMONEY

### String Types
- VARCHAR, NVARCHAR, CHAR, NCHAR (with Length)
- TEXT, NTEXT

### Date/Time Types
- DATE, TIME, DATETIME, DATETIME2, DATETIMEOFFSET, SMALLDATETIME

### Binary Types
- BINARY, VARBINARY, IMAGE (with optional Length)

### Other Types
- BIT, UNIQUEIDENTIFIER, XML, JSON

## 🌐 Localization

- **Primary Language:** Persian (فارسی) - RTL Interface
- **RTL Support:** Full right-to-left layout
- **Status Messages:** All in Persian
- **Help Text:** Comprehensive Persian documentation

## 📋 NuGet Dependencies

- `Microsoft.Data.SqlClient` - SQL Server connectivity
- `Microsoft.Extensions.DependencyInjection` - Dependency injection

## 🐛 Known Limitations

- Only supports SQL Server 2016 and later
- Backup/Restore operations require SQL Server native tools
- Schedule functionality requires app to be running
- Primary Key on new columns requires table recreation

## 🚧 Future Enhancements

- [ ] English language support
- [ ] Schedule service for background execution
- [ ] Query builder for complex operations
- [ ] Database comparison tools
- [ ] Performance analysis dashboard
- [ ] Export/Import functionality
- [ ] Multi-server management
- [ ] Backup history tracking

## 📝 Usage Examples

### Backup with Compression
```
1. Select Database → MyDatabase
2. Choose Backup Type → Full
3. Set Path → C:\Backups\
4. Enable Compression ✓
5. Click Start Backup
```

### Restore to Point in Time
```
1. Select Database → MyDatabase
2. Choose Backup File → MyDatabase_Full_20240115_143022.bak
3. Click Restore
4. Monitor progress
```

### Add DECIMAL Column
```
1. Select Database → MyDatabase
2. Select Table → Sales
3. Field Name → UnitPrice
4. Data Type → DECIMAL
5. Precision → 18
6. Scale → 2
7. Allow NULL ✓
8. Click Add Field
```

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

**Farzin Seyedi**
- GitHub: [@fzseyedi](https://github.com/fzseyedi)
- Email: [Contact via GitHub]

## 🙏 Acknowledgments

- Microsoft SQL Server documentation
- WPF MVVM best practices community
- .NET 10 framework team

## 📞 Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Include error messages and steps to reproduce
- Provide screenshot if applicable

## 📚 Additional Resources

- [SQL Server Documentation](https://learn.microsoft.com/en-us/sql/sql-server/)
- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

---

**Last Updated:** 2024
**Version:** 1.0.0
**Status:** Active Development ✅
