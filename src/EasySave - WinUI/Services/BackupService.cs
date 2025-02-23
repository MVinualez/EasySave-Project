﻿using EasySaveLibrary.Services;
using System;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<bool> CanStartBackup() {
            return !_processChecker.IsOfficeAppRunning(); // Retourne false si Word est ouvert
        }

        /// <summary>
        /// Attend que Word ou une application bloquante soit fermée avant de continuer la sauvegarde.
        /// </summary>
        private async Task WaitForProcessToClose(TextBlock textBlock) {
            while (!await CanStartBackup()) {
                textBlock.DispatcherQueue.TryEnqueue(() => {
                    textBlock.Text = _resourceLoader.GetString("BackupPage_BackupPaused");
                });

                await Task.Delay(2000); // Vérifie toutes les 2 secondes si Word est fermé
            }

            textBlock.DispatcherQueue.TryEnqueue(() => {
                textBlock.Text = _resourceLoader.GetString("BackupPage_BackupResumed");
            });
        }

        public async Task<double> RunBackup(string name, string source, string destination, bool isFullBackup, TextBlock textBlock) {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try {
                if (!Directory.Exists(source)) {
                    await _notificationViewModel.ShowPopupDialog(
                        _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                        _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"),
                        string.Empty, "OK", XamlRoot);
                    return 0;
                }

                _stateViewModel.RegisterJobState(name);

                string fullPathBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Backup");
                Directory.CreateDirectory(fullPathBackup);

                await CopyDirectoryReccursively(name, source, destination, isFullBackup, textBlock);
                await CopyDirectoryReccursively(name, source, fullPathBackup, isFullBackup, textBlock);

                var fileManager = new FileManager(destination, [".docx", ".txt"], EncryptionKey);
                fileManager.Transform();
                _stateViewModel.CompleteJobState(name);

                textBlock.DispatcherQueue.TryEnqueue(() => {
                    textBlock.Text = _resourceLoader.GetString("BackupPage_BackupFinished");
                });

            } catch (Exception ex) {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalSeconds;
        }

        public async Task CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup, TextBlock textBlock) {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                string targetSubDir = dir.Replace(source, target);
                Directory.CreateDirectory(targetSubDir);
            }

            foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(target, fileName);
                long fileSize = new FileInfo(file).Length;

                await WaitForProcessToClose(textBlock); // Attendre que Word soit fermé avant de copier

                await Task.Run(() => {
                    if (ShouldCopyFile(file, destFile)) {
                        textBlock.DispatcherQueue.TryEnqueue(() => {
                            textBlock.Text = string.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName);
                        });

                        _stateViewModel.TrackFileInState(name, file, destFile, fileSize);
                        File.Copy(file, destFile, true);
                        _stateViewModel.MarkFileAsProcessed(name, file, fileSize);
                    }
                });
            }
        }

        protected abstract bool ShouldCopyFile(string sourceFile, string destFile);
    }
}
