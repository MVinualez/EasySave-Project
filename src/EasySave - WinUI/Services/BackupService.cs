using EasySaveLibrary.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using EasySaveLibrary.ViewModels;
using EasySave___WinUI.Models;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using EasySave___WinUI.ViewModels;

namespace EasySave___WinUI.Services {
    public abstract class BackupService {
        private readonly StateViewModel _stateViewModel;
        private readonly NotificationViewModel _notificationViewModel;
        private readonly EncryptionViewModel _encryptionViewModel;
        private readonly ResourceLoader _resourceLoader;
        private volatile bool _isPaused = false;
        private volatile bool _isStopped = false;
        private Stopwatch _copyStopwatch;
        private Stopwatch _encryptionStopwatch;
        public List<string> priorityExtensions { get; set;  } = new List<string> { ".iso" };
        public int maxParallelSizeKb { get; private set; } = 50000; // Exemple de valeur paramétrable
        private volatile bool largeFileInProgress = false;        
        public XamlRoot XamlRoot { get; }
        public string EncryptionKey { get; set; }
        public string JobName { get; set; }

        protected BackupService(XamlRoot xamlRoot) {
            XamlRoot = xamlRoot;
            EncryptionKey = string.Empty;
            _stateViewModel = StateViewModel.GetStateViewModelInstance(XamlRoot);
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
            _encryptionViewModel = EncryptionViewModel.GetEncryptionViewModelInstance();
            _resourceLoader = new ResourceLoader();

            _copyStopwatch = new Stopwatch();
            _encryptionStopwatch = new Stopwatch();
        }

        public void SetPriorityExtension(List<string> extensions)
        {
            priorityExtensions = new List<string>(extensions);
        }

        public void SetMaxParallelSizeKb(int sizeKb)
        {
            maxParallelSizeKb = sizeKb;
        }

        public void PauseBackup() {
            _isPaused = true;
        }

        public void ResumeBackup() {
            _isPaused = false;
        }

        public void StopBackup() {
            _isStopped = true;
        }

        public async Task<bool> CanStartBackup() {
            return !new ProcessChecker().IsOfficeAppRunning();
        }

        private async Task WaitForProcessToClose(TextBlock textBlock) {
            while (!await CanStartBackup()) {
                textBlock.DispatcherQueue.TryEnqueue(() => {
                    textBlock.Text = _resourceLoader.GetString("BackupPage_BackupPaused");
                });
                await Task.Delay(2000);
            }

            textBlock.DispatcherQueue.TryEnqueue(() => {
                textBlock.Text = _resourceLoader.GetString("BackupPage_BackupResumed");
            });
        }
      
        public async Task<List<double>> RunBackup(string name, string source, string destination, bool isFullBackup, TextBlock textBlock) {
            JobName = name;

            _copyStopwatch.Start();
            try
            {
                if (!Directory.Exists(source))
                {
                    await _notificationViewModel.ShowPopupDialog(
                        _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                        _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                        string.Empty, "OK", XamlRoot);
                    return new List<double> { 0 };
                }
          
                _stateViewModel.RegisterJobState(name);
                string fullPathBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Backup");
                Directory.CreateDirectory(fullPathBackup);

                await CopyDirectoryReccursively(name, source, destination, isFullBackup, textBlock);
                await CopyDirectoryReccursively(name, source, fullPathBackup, isFullBackup, textBlock);

                _copyStopwatch.Stop();
                _encryptionStopwatch.Start();

                await _encryptionViewModel.EncryptFile(destination, new List<string> { ".pdf", ".docx", ".txt", ".mp4" }, EncryptionKey);

                _encryptionStopwatch.Stop();

                _stateViewModel.CompleteJobState(name);

                textBlock.DispatcherQueue.TryEnqueue(() =>
                {
                    textBlock.Text = _resourceLoader.GetString("BackupPage_BackupFinished");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }

            return new List<double> { _copyStopwatch.Elapsed.TotalSeconds, _encryptionStopwatch.Elapsed.TotalSeconds };
        }

        public async Task CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, TextBlock textBlock)
        {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                string targetSubDir = dir.Replace(source, target);
                Directory.CreateDirectory(targetSubDir);
            }

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            var priorityFiles = files.Where(f => priorityExtensions.Contains(Path.GetExtension(f))).ToList();
            var nonPriorityFiles = files.Except(priorityFiles).ToList();
           

            foreach (string file in priorityFiles.Concat(nonPriorityFiles))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(target, fileName);
                long fileSize = new FileInfo(file).Length;
                long fileSizeKb = fileSize / 1024;

                await WaitForProcessToClose(textBlock);

                while (_isPaused)
                {
                    await Task.Delay(500);
                }

                if (_isStopped) return;

                ///
                if (fileSizeKb > maxParallelSizeKb)
                {
                    lock (this)
                    {
                        while (largeFileInProgress) // Attendre la fin du transfert en cours
                        {
                            Task.Delay(500).Wait();
                        }
                        largeFileInProgress = true; // Marquer qu'un fichier volumineux est en cours
                    }
                }
                ///


                await Task.Run(() =>
                {
                    if (ShouldCopyFile(file, destFile))
                    {
                        textBlock.DispatcherQueue.TryEnqueue(() =>
                        {
                            textBlock.Text = string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName);
                        });

                        _stateViewModel.TrackFileInState(name, file, destFile, fileSize);
                        File.Copy(file, destFile, true);
                        _stateViewModel.MarkFileAsProcessed(name, file, fileSize);
                    }
                });
                if (fileSizeKb > maxParallelSizeKb)
                {
                    lock (this)
                    {
                        largeFileInProgress = false;
                    }
                }
            }
        }
                
        protected abstract bool ShouldCopyFile(string sourceFile, string destFile);
    }
}
