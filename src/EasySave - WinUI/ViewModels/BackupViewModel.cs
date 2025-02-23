using CommunityToolkit.Mvvm.ComponentModel;
using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using EasySaveLibrary.Models;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using EasySaveLibrary.ViewModels;

namespace EasySave___WinUI.ViewModels;

public partial class BackupViewModel : ObservableRecipient {
    private static BackupViewModel? _instance;
    private readonly NotificationViewModel _notificationViewModel;
    private readonly ResourceLoader _resourceLoader;
    private readonly ProcessChecker _processChecker;
    private readonly LogEntryViewModel _logEntryViewModel;

    private XamlRoot _xamlRoot;

    private BackupViewModel(XamlRoot xamlRoot) {
        _xamlRoot = xamlRoot;

        _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
        _logEntryViewModel = LogEntryViewModel.GetLogEntryViewModelInstance();
        _resourceLoader = new ResourceLoader();
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
        return isFullBackup ? BackupServiceComplete.GetBackupServiceCompleteInstance(_xamlRoot) : BackupServiceDifferential.GetBackupServiceDifferentialInstance(_xamlRoot);
    }

    public async Task<bool> CanStartBackup(bool isFullBackup) {
        BackupService backupService = GetBackupServiceInstance(isFullBackup);

        while (backupService.CanStartBackup()) {
            bool retry = await _notificationViewModel.ShowPopupDialog(
                "Microsoft Office Application Detected",
                "A Microsoft Office application is running. Please close it before starting the backup.",
                "Cancel", "Check Again", _xamlRoot
            );

            if (!retry) return false; // L'utilisateur annule la sauvegarde
        }
        return true; // Aucune application Office en cours, on peut continuer
    }

    public async void StartBackup(string name, string source, string destination, bool isFullBackup, string backupEncryptionKey, TextBlock textBlock) {

        // Récupération de l'instance de service de sauvegarde appropriée
        var backupService = GetBackupServiceInstance(isFullBackup);
        backupService.EncryptionKey = backupEncryptionKey;

        // Vérification des processus Office avant de lancer la sauvegarde
        bool canStart = await CanStartBackup(isFullBackup);
        if (!canStart) {
            await _notificationViewModel.ShowPopupDialog(
                _resourceLoader.GetString("Backup_OfficeCanceled"),
                _resourceLoader.GetString("Backup_OfficeCanceled"),
                String.Empty, "OK", _xamlRoot);
            return;
        }

        if (string.IsNullOrWhiteSpace(name) ||
            source == _resourceLoader.GetString("BackupPage_NoFolderSelected") ||
            destination == _resourceLoader.GetString("BackupPage_NoFolderSelected")) {
            await _notificationViewModel.ShowPopupDialog(
                _resourceLoader.GetString("BackupPage_FillAllFieldsError"),
                _resourceLoader.GetString("BackupPage_FillAllFieldsError"),
                String.Empty, "OK", _xamlRoot);
            return;
        }

        DirectoryInfo di = new DirectoryInfo(source);
        long fileSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);

        try {
            double elapsedTime = await GetBackupServiceInstance(isFullBackup).RunBackup(name, source, destination, isFullBackup, textBlock); ;

            _logEntryViewModel.WriteLog(name, source, destination, fileSize, elapsedTime);
        } catch (Exception ex) {
            await _notificationViewModel.ShowPopupDialog(
                $"{_resourceLoader.GetString("BackupPage_BackupError")} {ex.Message}",
                $"{_resourceLoader.GetString("BackupPage_BackupError")} {ex.Message}",
                String.Empty, "OK", _xamlRoot);
        }        
    }
}
