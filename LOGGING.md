# Logging Configuration

## Overview

This application uses log4net for file-based logging. Logs are written to files in the application data directory rather than the console to keep the console output clean for the TUI interface.

## Log File Location

Logs are stored in:
- **Windows**: `%LOCALAPPDATA%\JonasTodo\logs\`
- **Linux/Mac**: `~/.local/share/JonasTodo/logs/`

## Log Files

The application creates two separate log files:

1. **JonasTodo.log** - Main application logs
   - All application events, user actions, and business logic
   - **Filename pattern**: `JonasTodo-2025-11-11.log` (includes date)
   
2. **JonasTodo.EntityFramework.log** - Database logs
   - Entity Framework Core logs (Warning level and above)
   - Database queries and operations
   - **Filename pattern**: `JonasTodo.EntityFramework-2025-11-11.log` (includes date)

## Log File Format

- **Pattern**: Each log entry includes timestamp, thread ID, log level, logger name, and message
- **Example**: `2025-11-11 16:33:23,447 [1] INFO  Program - Application started`

## Log Rotation

The logging system automatically manages log files with the following rules:

1. **Daily Rolling**: A new log file is created each day with the date appended (e.g., `JonasTodo-2025-11-11.log`)
2. **Size Rolling**: If a single day's log exceeds 10MB, it rolls to a numbered backup
3. **Backup Retention**: Up to 10 backup files are retained
4. **Automatic Cleanup**: Older backups beyond the retention limit are automatically deleted

## Log Levels

- **INFO**: General application flow (default level)
- **WARN**: Warning messages
- **ERROR**: Error messages

## What Gets Logged

### Application Events
- Application startup and shutdown
- User menu selections and navigation
- Table operations (Topics, Subtopics)

### Database Operations
- Entity retrieval with counts
- Entity creation with details
- Entity updates
- Warning when entities are not found

### EF Core Logging
Entity Framework Core logging is reduced to WARNING level to minimize noise. Only significant database issues are logged. These logs are written to a separate file (`JonasTodo.EntityFramework.log`) to keep the main application log clean.

## Configuration

The logging configuration is stored in `log4net.config`. To modify logging behavior:

1. Change log level in the `<root>` section
2. Adjust file size limit in `<maximumFileSize>`
3. Change backup count in `<maxSizeRollBackups>`
4. Modify date pattern in `<datePattern>`

## Troubleshooting

If logs are not being created:
1. Check that the application has write permissions to the log directory
2. Verify that `log4net.config` is present in the application directory
3. Check that the log directory is being created (the application creates it automatically)

## Disabling/Changing Logging

To disable logging or change the logging provider, modify the logging configuration in `Program.cs` in the `ConfigureLogging` section.
