using EasySave___WinUI.ViewModels;
using EasySave___WinUI.Models;
using EasySaveLibrary.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Views {
    public sealed partial class ParallelBackupPage : Page {
        public ParallelBackupViewModel ViewModel { get; }
        private readonly BackupViewModel _backupViewModel;

        public ParallelBackupPage() {
            this.InitializeComponent();
            ViewModel = new ParallelBackupViewModel();
            _backupViewModel = BackupViewModel.GetBackupViewModelInstance(App.MainWindow.Content.XamlRoot);
            this.DataContext = ViewModel;
        }

        private async void OnRunBackup(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is BackupJobInfoModel job) {
                bool isFullBackup = true;  // Par défaut, exécution en mode complet
                string encryptionKey = ""; // Pas de clé de chiffrement pour l'instant

                await _backupViewModel.StartBackup(job.Name, job.Source, job.Destination, isFullBackup, encryptionKey, ProgressTextBox);
                ViewModel.BackupStatus = $"Sauvegarde {job.Name} en cours...";
            }
        }

        private void OnPauseBackup(object sender, RoutedEventArgs e) {
            _backupViewModel.PauseBackup();
            ViewModel.BackupStatus = "Sauvegarde en pause";
        }

        private void OnResumeBackup(object sender, RoutedEventArgs e) {
            _backupViewModel.ResumeBackup();
            ViewModel.BackupStatus = "Sauvegarde en cours...";
        }

        private void OnStopBackup(object sender, RoutedEventArgs e) {
            _backupViewModel.StopBackup();
            ViewModel.BackupStatus = "Sauvegarde arrêtée";
        }
    }
}
