using EasySaveLibrary.Services;
using System;
using System.IO;
using EasySaveLibrary.ViewModels;
using EasySave___WinUI.Models;
using Microsoft.UI.Xaml;
using EasySave___WinUI.ViewModels;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Controls;
using EasySave___WinUI.CryptoSoft;
using System.Reflection;
using System.Diagnostics;

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


        public async Task<double> RunBackup(string name, string source, string destination, bool isFullBackup, TextBlock textBlock) {
            return await Task.Run(async () => {
                Stopwatch stopwatch = Stopwatch.StartNew();
                BackupJob job = new BackupJob(name, source, destination, isFullBackup);

                string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

                string dirName = "Backup";
                string fullPathBackup = Path.Combine(path, dirName);
                if (!Directory.Exists(fullPathBackup)) {
                    Directory.CreateDirectory(fullPathBackup);
                }

                _stateViewModel.RegisterJobState(name);

                try {
                    if (!Directory.Exists(job.Source)) {
                        await _notificationViewModel.ShowPopupDialog(
                            _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                            _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                            string.Empty, "OK", XamlRoot);
                        return 0; // Retourne 0 si la source n'existe pas
                    }

                    await CopyDirectoryReccursively(job.Name, job.Source, job.Destination, job.IsFullBackup, textBlock);
                    await CopyDirectoryReccursively(job.Name, job.Source, fullPathBackup, job.IsFullBackup, textBlock);

                    var fileManager = new FileManager(job.Destination, [".docx", ".txt"], EncryptionKey);
                    fileManager.Transform();
                    _stateViewModel.CompleteJobState(name);

                    textBlock.DispatcherQueue.TryEnqueue(() => {
                        textBlock.Text = _resourceLoader.GetString("BackupPage_BackupFinished");
                    });

                } catch (Exception ex) {
                    Console.WriteLine($"❌ Erreur : {ex.Message}");
                }

                stopwatch.Stop();
                return stopwatch.Elapsed.TotalSeconds; // Retourne le temps écoulé
            });
        }


        public abstract Task CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, TextBlock textBlock);
    }
}
