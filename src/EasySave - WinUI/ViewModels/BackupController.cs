using System.Diagnostics;
using EasySave___WinUI.Models;
using easysave_project.Models;
using easysave_project.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace easysave_project.Controllers
{
    internal class BackupJobController
    {
        private readonly BackupService _backupService;

        private readonly ProcessChecker _processChecker = new ProcessChecker();
        public async Task<bool> ShowOfficeWarningDialog(XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title = "Microsoft Office Application Detected",
                Content = "A Microsoft Office application is running. Please close it before starting the backup.",
                PrimaryButtonText = "Check Again",
                CloseButtonText = "Cancel",
                XamlRoot = xamlRoot
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
        public async Task<bool> CanStartBackup(XamlRoot xamlRoot)
        {
            while (_processChecker.IsOfficeAppRunning())
            {
                bool retry = await ShowOfficeWarningDialog(xamlRoot);
                if (!retry) return false; // User cancels the backup
            }
            return true; // No Office apps running, continue
        }

        public BackupJobController(BackupService backupService)
        {
            _backupService = backupService;
        }

        public void StartBackup(string name, string source, string destination, bool isFullBackup, Stopwatch copyStopwatch, Stopwatch encryptionStopwatch)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunBackup(job, copyStopwatch, encryptionStopwatch);
        }

        public void StartDiffBackup(string name, string source, string destination, bool isFullBackup, Stopwatch copyStopwatch, Stopwatch encryptionStopwatch)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunDifferentialBackup(job, copyStopwatch, encryptionStopwatch);

        }
        public void StartRestauration(string name, string source, string destination, bool isFullBackup)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunRestauration(job);
        }
    }
}
