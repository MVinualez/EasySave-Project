using EasySave___WinUI.CryptoSoft;
using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using EasySave___WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel.Resources;
using Windows.UI.StartScreen;

namespace EasySave___WinUI.Services {
    internal class BackupServiceComplete : BackupService {
        private static BackupServiceComplete? _instance;
        private readonly NotificationViewModel _notificationViewModel;
        private readonly StateViewModel _stateViewModel;
        private readonly ResourceLoader _resourceLoader;

        private BackupServiceComplete(XamlRoot xamlRoot) : base(xamlRoot) {
            _stateViewModel = StateViewModel.GetStateViewModelInstance(xamlRoot);
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
            _resourceLoader = new ResourceLoader();
        }

        public static BackupServiceComplete GetBackupServiceCompleteInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupServiceComplete(xamlRoot);
            return _instance;
        }

        public override async Task CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, TextBlock textBlock) {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                string targetSubDir = dir.Replace(source, target);
                Directory.CreateDirectory(targetSubDir);
            }

            foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                string fileName = Path.GetFileName(file);
                string destFile = file.Replace(source, target);
                long fileSize = new FileInfo(file).Length;

                textBlock.DispatcherQueue.TryEnqueue(() => {
                    textBlock.Text = string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName);
                });

                _stateViewModel.TrackFileInState(name, file, destFile, fileSize);

                await Task.Run(() => File.Copy(file, destFile, true));

                _stateViewModel.MarkFileAsProcessed(name, file, fileSize);
            }
        }
    }
}
