using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BybitTrader.Components
{
    public class UpdateManager
    {
        private const string GitHubApiUrl = "https://api.github.com/repos/Skaikru0518/BybitTrader/releases/latest";
        private const string UpdateDownloadUrl = "https://github.com/Skaikru0518/BybitTrader/releases/latest/download/BybitTrader.exe";

        public event Action<string> UpdateAvailable;
        public event Action<string> UpdateFailed;
        public bool IsUpdateAvailable { get; private set; } = false;
        public string LatestVersion { get; private set; }

        public async Task CheckForUpdates()
        {
            string currentVersion = AppInfo.Current.VersionString;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "BybitTrader-Updater");

                    HttpResponseMessage response = await client.GetAsync(GitHubApiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var json = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                        string latestVersion = json.GetProperty("tag_name").GetString().Replace("v", "");

                        Debug.WriteLine($"🔍 Latest Version: {latestVersion}, Current Version: {currentVersion}");

                        if (Version.Parse(latestVersion) > Version.Parse(currentVersion))
                        {
                            LatestVersion = latestVersion;
                            IsUpdateAvailable = true;
                            UpdateAvailable?.Invoke(latestVersion);
                        }
                        else
                        {
                            IsUpdateAvailable = false;
                        }
                    }
                    else
                    {
                        UpdateFailed?.Invoke($"❌ Failed to check updates: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateFailed?.Invoke($"⚠️ Error checking for updates: {ex.Message}");
            }
        }

        public async Task DownloadAndUpdate()
        {
            string tempFolder = Path.Combine(AppContext.BaseDirectory, "Temp");
            Directory.CreateDirectory(tempFolder);

            string newExePath = Path.Combine(tempFolder, "BybitTrader.exe");
            string oldExePath = Environment.ProcessPath;
            string batchScriptPath = Path.Combine(tempFolder, "update_script.bat");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "BybitTrader-Updater");

                    HttpResponseMessage response = await client.GetAsync(UpdateDownloadUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(newExePath, fileBytes);
                    }
                    else
                    {
                        UpdateFailed?.Invoke($"Download failed. HTTP Status: {response.StatusCode}");
                        return;
                    }
                }

                File.WriteAllText(batchScriptPath, $@"
@echo off
title Updating BybitTrader...
setlocal enabledelayedexpansion
set total=50
set progress=0
set /a increment=100 / %total%

:progress
set /a progress+=1
set /a percent=progress * increment / 100
set bar=
for /L %%i in (1,1,!progress!) do set bar=!bar!#
for /L %%i in (!progress!,1,%total%) do set bar=!bar!-
cls
echo Updating BybitTrader...
echo [!bar!] !percent!%%
timeout /t 1 >nul
if !progress! lss %total% goto progress

taskkill /F /IM BybitTrader.exe >nul 2>&1
timeout /t 2 /nobreak >nul
del /f /q ""{oldExePath}""
move /y ""{newExePath}"" ""{oldExePath}""
rd /s /q ""{tempFolder}""
start """" ""{oldExePath}""
del /f /q ""%~f0""
exit
");

                Process.Start(new ProcessStartInfo
                {
                    FileName = batchScriptPath,
                    CreateNoWindow = false,
                    UseShellExecute = false
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                UpdateFailed?.Invoke($"Update failed: {ex.Message}");
            }
        }
    }
}
