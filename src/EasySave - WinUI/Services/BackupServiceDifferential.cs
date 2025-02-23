using EasySave___WinUI.CryptoSoft;
using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel.Resources;

namespace EasySave___WinUI.Services {
    internal class BackupServiceDifferential : BackupService {
        private static BackupServiceDifferential? _instance; 
        private readonly NotificationViewModel _notificationViewModel;
        private readonly StateViewModel _stateViewModel;
        private readonly ResourceLoader _resourceLoader;

        private BackupServiceDifferential(XamlRoot xamlRoot) : base(xamlRoot) {
            _stateViewModel = StateViewModel.GetStateViewModelInstance(xamlRoot);
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
            _resourceLoader = new ResourceLoader();
        }

        public static BackupServiceDifferential GetBackupServiceDifferentialInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupServiceDifferential(xamlRoot);
            return _instance;
        }

        public override async void RunBackup(string name, string source, string destination, bool isFullBackup, Action<string> onProgressUpdate) {
            BackupJob job = new BackupJob(name, source, destination, isFullBackup);

            try {
                if (!Directory.Exists(job.Source)) {
                    await _notificationViewModel.ShowPopupDialog(
                    _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                    _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                    string.Empty, "OK", XamlRoot);
                    return;
                }

                Directory.CreateDirectory(job.Destination);

                string[] files = Directory.GetFiles(job.Source);
                string[] filesDestination = Directory.GetFiles(job.Destination);

                HashSet<string> existingFiles = new HashSet<string>(filesDestination.Select(Path.GetFileName));

                int copiedFiles = 0;
                _stateViewModel.RegisterJobState(name);


                foreach (var file in files) {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);

                    if (!existingFiles.Contains(fileName) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile)) {
                        long fileSize = new FileInfo(file).Length;
                        int fileSizeInt = (int)fileSize;

                        _stateViewModel.TrackFileInState(name, file, destFile, fileSizeInt);
                        onProgressUpdate?.Invoke(string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName));

                        CopyDirectoryReccursively(job.Name, job.Source, job.Destination, job.IsFullBackup, onProgressUpdate);
                        Console.WriteLine($"✅ {fileName} copié !");
                        copiedFiles++;

                        _stateViewModel.MarkFileAsProcessed(name, file, fileSizeInt);
                    }
                }
                var fileManager = new FileManager(job.Destination, new List<string> { ".pdf", ".docx", ".txt" }, EncryptionKey);
                fileManager.Transform();
                _stateViewModel.CompleteJobState(name);
                if (copiedFiles == 0) {
                    Console.WriteLine("✨ Aucun nouveau fichier, rien à bouger.");
                } else {
                    onProgressUpdate?.Invoke(_resourceLoader.GetString("BackupPage_BackupFinished"));
                }
            } catch (Exception ex) {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        //public override void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup) {
        //    foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
        //        string targetSubDir = dir.Replace(source, target);
        //        if (!Directory.Exists(targetSubDir)) {
        //            Directory.CreateDirectory(targetSubDir);
        //        }
        //    }

        //    foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
        //        string destFile = file.Replace(source, target);

        //        if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile)) {
        //            File.Copy(file, destFile, true);
        //            Console.WriteLine($"✅ {file} → {destFile}");
        //        }
        //    }
        //}
    }
}
