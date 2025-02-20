using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Controllers {
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

        public void StartBackup(string name, string source, string destination, bool isFullBackup)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunBackup(job);
        }

        public void StartDiffBackup(string name, string source, string destination, bool isFullBackup)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunDifferentialBackup(job);

        }
        public void StartRestauration(string name, string source, string destination, bool isFullBackup)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunRestauration(job);
        }
    }
}
