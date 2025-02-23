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

        public void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, Action<string> onProgressUpdate) {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                string targetSubDir = dir.Replace(source, target);
                Directory.CreateDirectory(targetSubDir);
            }

            foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                string fileName = Path.GetFileName(file);
                string destFile = file.Replace(source, target);
                long fileSize = new FileInfo(file).Length;

                onProgressUpdate?.Invoke(string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName));

                _stateViewModel.TrackFileInState(name, file, destFile, fileSize);

                File.Copy(file, destFile, true);

                _stateViewModel.MarkFileAsProcessed(name, file, fileSize);
            }
        }

        public abstract void RunBackup(string name, string source, string target, bool isFullBackup, Action<string> onProgressUpdate);
        //public abstract void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, Action<string> onProgressUpdate);
    }
}
