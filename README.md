# NoOneNoticed | 🇺🇸

A cross-platform terminal-based system monitor written in C# (.NET).
It monitors the entire file system and running processes in real time, logging important events and alerting on excessive CPU/RAM usage.
Press **ESC** to stop the session and save the report, or **SHIFT + ESC** to stop, save, and open the report automatically.

## Features

- **File monitoring** — tracks file/folder creation, deletion, and renaming within a given directory (and subdirectories)
- **Process monitoring** — detects processes starting/stopping and flags high CPU or RAM usage
- **Real-time logging** — color-coded console output by severity (`Info`, `Warning`, `Danger`, `Error`)
- **On-demand report export** — writes the full session log to a timestamped `.txt` file in the system temp folder (SHIFT+ESC to access it directly)

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (for building/running from source)

## Running

```bash
dotnet run --project NoOneNoticed -- [options]
```

Or, after publishing, run the executable directly:

```bash
NoOneNoticed.exe [options]
```

### Options

| Flag | Description | Default |
|------|-------------|---------|
| `--path <path>` | Root directory to monitor | `C:\` |
| `--range <ms>` | Process polling interval, in milliseconds | `2000` |
| `--ram <mb>` | RAM usage threshold (MB) that triggers a warning | `500` |

Example:

```bash
NoOneNoticed.exe --path C:\Users --range 3000 --ram 300
```

### Controls

- **ESC** — stop monitoring and export the report
- **SHIFT + ESC** — stop monitoring, export the report, and open it automatically

The report is saved to the system temp folder (`%TEMP%` on Windows) and its path is printed to the console when monitoring stops.

## Publishing a standalone executable

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

This produces a single `.exe` that includes the .NET runtime, so it can run on machines without .NET installed.

## Notes

- Monitoring an entire drive (e.g. `C:\`) can generate a very high volume of file system events; the underlying `FileSystemWatcher` may occasionally drop events under heavy load (logged as an `Error` entry when it happens). For more reliable tracking, consider pointing `--path` at a narrower folder.
- Reading some system-owned processes may require running the terminal as Administrator.

Name inspired by "No One Noticed", a song from The Marías' album *Submarine*.