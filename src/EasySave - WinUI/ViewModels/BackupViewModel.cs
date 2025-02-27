using CommunityToolkit.Mvvm.ComponentModel;
using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using EasySave___WinUI.ViewModels;
using EasySaveLibrary.ViewModels;
using System.Diagnostics;

namespace EasySave___WinUI.ViewModels {

    public enum BackupState {
        Running,
        Paused,
        Stopped
    }

    public partial class BackupViewModel : ObservableRecipient {
        private static BackupViewModel? _instance;
        private readonly NotificationViewModel _notificationViewModel;
        private readonly ResourceLoader _resourceLoader;
        private readonly LogEntryViewModel _logEntryViewModel;
        private XamlRoot _xamlRoot;
        private BackupService? _currentBackupService;
        private BackupSocketServer? _socketServer;

        private readonly List<Thread> _backupThreads = new();
        private readonly List<BackupService> _activeBackupServices = new();
        public List<string> priorityExtensions { get; set; } = new List<string> { ".iso" };
        public int maxParallelSizeKb { get; private set; } = 50000;

        public BackupState CurrentBackupState { get; set; } = BackupState.Stopped;

        public BackupViewModel()
        {
        }

        private BackupViewModel(XamlRoot xamlRoot) {
            _socketServer = new BackupSocketServer();
            _ = _socketServer.StartServer();
            _xamlRoot = xamlRoot;
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
            _logEntryViewModel = LogEntryViewModel.GetLogEntryViewModelInstance();
            _resourceLoader = new ResourceLoader();
        }

        public static BackupViewModel GetBackupViewModelInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupViewModel(xamlRoot);
            return _instance;
        }

        public BackupService GetBackupServiceInstance(bool isFullBackup) {
            BackupService backupService = isFullBackup
                ? BackupServiceComplete.GetBackupServiceCompleteInstance(_xamlRoot)
                : BackupServiceDifferential.GetBackupServiceDifferentialInstance(_xamlRoot);

            _socketServer?.SetBackupService(backupService);

            return backupService;
        }

        public async Task StartBackup(string name, string source, string destination, bool isFullBackup, string backupEncryptionKey, TextBlock textBlock) {
            var backupService = GetBackupServiceInstance(isFullBackup);
            backupService.priorityExtensions = priorityExtensions;
            backupService.EncryptionKey = backupEncryptionKey;

            _activeBackupServices.Add(backupService);

            Thread backupThread = new Thread(async () =>
            {
                try {
                    Debug.WriteLine(backupService.priorityExtensions.ToString());
                    List<double> elapsedTimes = await backupService.RunBackup(name, source, destination, isFullBackup, textBlock);
                    _logEntryViewModel.WriteLog(name, source, destination, new DirectoryInfo(source).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length), elapsedTimes[0], elapsedTimes[1]);
                } finally {
                    _activeBackupServices.Remove(backupService);
                }
            });

            _backupThreads.Add(backupThread);
            backupThread.Start();
        }



        public void PauseBackup() {
            if (_currentBackupService != null && CurrentBackupState == BackupState.Running) {
                _currentBackupService.PauseBackup();
                CurrentBackupState = BackupState.Paused;
            }
        }

        public void ResumeBackup() {
            if (_currentBackupService != null && CurrentBackupState == BackupState.Paused) {
                _currentBackupService.ResumeBackup();
                CurrentBackupState = BackupState.Running;
            }
        }

        public void StopBackup() {
            if (_currentBackupService != null && CurrentBackupState != BackupState.Stopped) {
                _currentBackupService.StopBackup();
                CurrentBackupState = BackupState.Stopped;
            }
        }
        public void SetPriorityExtension(List<string> extensions)
        {
            priorityExtensions = new List<string>(extensions);
        }

        public void SetMaxParallelSizeKb(int sizeKb)
        {
            maxParallelSizeKb = sizeKb;
        }

    }
}
