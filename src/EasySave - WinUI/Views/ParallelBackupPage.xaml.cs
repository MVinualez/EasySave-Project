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
                bool isFullBackup = true;
                string encryptionKey = "";

                // Trouver le parent StackPanel des boutons
                var buttonStackPanel = button.Parent as StackPanel;
                if (buttonStackPanel != null) {
                    // Trouver le parent général de l'item (StackPanel principal de l'élément de la ListView)
                    var mainStackPanel = buttonStackPanel.Parent as StackPanel;
                    if (mainStackPanel != null) {
                        // Trouver le TextBlock dans le StackPanel principal
                        var progressTextBlock = mainStackPanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Name == "ProgressTextBlock");

                        if (progressTextBlock != null) {
                            await _backupViewModel.StartBackup(job.Name, job.Source, job.Destination, isFullBackup, encryptionKey, progressTextBlock);
                            ViewModel.BackupStatus = $"Sauvegarde {job.Name} en cours...";
                        }
                    }
                }
            }
        }

        private void OnPauseBackup(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is BackupJobInfoModel job) {
                _backupViewModel.PauseBackup(job.Name);
                ViewModel.BackupStatus = $"Sauvegarde {job.Name} en pause";
            }
        }

        private void OnResumeBackup(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is BackupJobInfoModel job) {
                _backupViewModel.ResumeBackup(job.Name);
                ViewModel.BackupStatus = $"Sauvegarde {job.Name} en cours...";
            }
        }

        private void OnStopBackup(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is BackupJobInfoModel job) {
                _backupViewModel.StopBackup(job.Name);
                ViewModel.BackupStatus = $"Sauvegarde {job.Name} arrêtée";
            }
        }
    }
}
