using EasySaveLibrary.Services;
using System;
using System.IO;
using EasySaveLibrary.ViewModels;
using EasySave___WinUI.Models;
using Microsoft.UI.Xaml;
using EasySave___WinUI.ViewModels;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Services {
    public abstract class BackupService {
        private readonly LogEntryViewModel _logEntryViewModel;
        private readonly ProcessChecker _processChecker;
        private readonly StateViewModel _stateViewModel;
        private readonly NotificationViewModel _notificationViewModel;
        private readonly ResourceLoader _resourceLoader;

        public XamlRoot XamlRoot { get; }

        public string EncryptionKey { get; set; }

        protected BackupService(XamlRoot xamlRoot) {
            XamlRoot = xamlRoot;
            EncryptionKey = string.Empty;
            _logEntryViewModel = LogEntryViewModel.GetLogEntryViewModelInstance();
            _stateViewModel = StateViewModel.GetStateViewModelInstance(XamlRoot);
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
            _processChecker = new ProcessChecker();
            _resourceLoader = new ResourceLoader();
        }

        public bool CanStartBackup() {
            return _processChecker.IsOfficeAppRunning();
        }

        public async void StateCreator(string backupName, string sourcePath, string destinationPath, Action<string> onProgressUpdate) {
            _stateViewModel.RegisterJobState(backupName);
            if (!Directory.Exists(sourcePath)) {
                await _notificationViewModel.ShowPopupDialog(
                    _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                    _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                    string.Empty, "OK", XamlRoot);
                return;
            }

            string[] files = Directory.GetFiles(sourcePath);
            foreach (var file in files) {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationPath, fileName);
                long fileSize = new FileInfo(file).Length;
                int fileSizeInt = (int)fileSize;

                _stateViewModel.TrackFileInState(backupName, file, destFile, fileSizeInt);

                // Notification de progression via callback
                onProgressUpdate?.Invoke(string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName));

                _stateViewModel.MarkFileAsProcessed(backupName, file, fileSizeInt);
            }
            _stateViewModel.CompleteJobState(backupName);
        }

        public abstract void RunBackup(string name, string source, string target, bool isFullBackup);
        public abstract void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup);
    }
}
