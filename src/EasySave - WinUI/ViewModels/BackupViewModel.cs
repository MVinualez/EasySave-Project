using CommunityToolkit.Mvvm.ComponentModel;
using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace EasySave___WinUI.ViewModels;

public partial class BackupViewModel : ObservableRecipient {
    private static BackupViewModel? _instance;
    private readonly BackupServiceComplete _backupServiceComplete;
    private readonly BackupServiceDifferential _backupServiceDifferential;
    private readonly NotificationService _notificationService;
    private readonly ProcessChecker _processChecker;

    private BackupViewModel(XamlRoot xamlRoot) {
        _backupServiceComplete = BackupServiceComplete.GetBackupServiceCompleteInstance(xamlRoot);
        _backupServiceDifferential = BackupServiceDifferential.GetBackupServiceDifferentialInstance(xamlRoot);
        _notificationService = NotificationService.GetNotificationServiceInstance();
        _processChecker = new ProcessChecker();
    }

    public static BackupViewModel GetBackupViewModelInstance(XamlRoot xamlRoot) {
        _instance ??= new BackupViewModel(xamlRoot);
        return _instance;
    }

    /// <summary>
    /// Renvoie l'instance de BackupService correspondante en fonction du type de sauvegarde.
    /// </summary>
    public BackupService GetBackupServiceInstance(bool isFullBackup) {
        return isFullBackup ? _backupServiceComplete : _backupServiceDifferential;
    }

    public async Task<bool> CanStartBackup(XamlRoot xamlRoot, bool isFullBackup) {
        BackupService backupService = GetBackupServiceInstance(isFullBackup);

        while (backupService.CanStartBackup()) {
            bool retry = await _notificationService.ShowDialogMessage(
                "Microsoft Office Application Detected",
                "A Microsoft Office application is running. Please close it before starting the backup.",
                "Check Again", "Cancel", xamlRoot
            );

            if (!retry) return false; // L'utilisateur annule la sauvegarde
        }
        return true; // Aucune application Office en cours, on peut continuer
    }

    public void StartBackup(string name, string source, string destination, bool isFullBackup, Action<string> onProgressUpdate) {
        GetBackupServiceInstance(isFullBackup).RunBackup(name, source, destination, isFullBackup, onProgressUpdate);
    }
}
