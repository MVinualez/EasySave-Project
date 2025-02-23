using EasySave___WinUI.CryptoSoft;
using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using EasySave___WinUI.Views;
using Microsoft.UI.Xaml;
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

        public override async void RunBackup(string name, string source, string destination, bool isFullBackup, Action<string> onProgressUpdate) {
            BackupJob job = new BackupJob(name, source, destination, isFullBackup);

            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            string dirName = "Backup";
            string fullPathBackup = Path.Combine(path, dirName);
            if (!Directory.Exists(Path.Combine(path, dirName))) {
                Directory.CreateDirectory(Path.Combine(path, dirName));
            }

            _stateViewModel.RegisterJobState(name);

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

                onProgressUpdate?.Invoke("coucou");


                foreach (var file in files) {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);
                    string destFileBackup = Path.Combine(fullPathBackup, fileName);
                    long fileSize = new FileInfo(file).Length;
                    
                    //Encrypt_Recursively(destFile, key);  
                    //File.Copy(file, destFileBackup, true);
                }

                CopyDirectoryReccursively(job.Name, job.Source, job.Destination, job.IsFullBackup, onProgressUpdate);
                CopyDirectoryReccursively(job.Name, job.Source, fullPathBackup, job.IsFullBackup, onProgressUpdate);

                var fileManager = new FileManager(job.Destination, [".docx", ".txt"], EncryptionKey);
                fileManager.Transform();
                _stateViewModel.CompleteJobState(name);

                onProgressUpdate?.Invoke(_resourceLoader.GetString("BackupPage_BackupFinished"));
            } catch (Exception ex) {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        //public override void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, Action<string> onProgressUpdate) {
        //    foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
        //        string targetSubDir = dir.Replace(source, target);
        //        Directory.CreateDirectory(targetSubDir);
        //    }

        //    foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
        //        string fileName = Path.GetFileName(file);
        //        string destFile = file.Replace(source, target);
        //        long fileSize = new FileInfo(file).Length;

        //        onProgressUpdate?.Invoke(string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName));

        //        _stateViewModel.TrackFileInState(name, file, destFile, fileSize);

        //        File.Copy(file, destFile, true);

        //        _stateViewModel.MarkFileAsProcessed(name, file, fileSize);
        //    }
        //}
    }
}
