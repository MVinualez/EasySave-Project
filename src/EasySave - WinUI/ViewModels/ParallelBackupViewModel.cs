using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using EasySaveLibrary.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.ViewModels;

public partial class ParallelBackupViewModel : ObservableRecipient {
    private readonly BackupParallelService _backupParallelService;
    private readonly BackupViewModel _backupViewModel;

    public ObservableCollection<BackupJobInfoModel> SavedBackups { get; }

    public ICommand RunBackupCommand { get; }
    public ICommand PauseBackupCommand { get; }
    public ICommand ResumeBackupCommand { get; }
    public ICommand StopBackupCommand { get; }
    public ICommand AddBackupCommand { get; }

    [ObservableProperty]
    private string backupStatus = "État : Stoppé";

    public ParallelBackupViewModel() {
        _backupParallelService = new BackupParallelService();
        _backupViewModel = BackupViewModel.GetBackupViewModelInstance(null);

        SavedBackups = new ObservableCollection<BackupJobInfoModel>();

        RunBackupCommand = new RelayCommand<BackupJobInfoModel>(async job => await RunBackup(job));
        PauseBackupCommand = new RelayCommand(PauseBackup);
        ResumeBackupCommand = new RelayCommand(ResumeBackup);
        StopBackupCommand = new RelayCommand(StopBackup);
        AddBackupCommand = new RelayCommand(async () => await AddBackup());

        LoadBackups();
    }

    private async void LoadBackups() {
        var backups = await _backupParallelService.LoadBackupsAsync();
        SavedBackups.Clear();
        foreach (var job in backups)
            SavedBackups.Add(job);
    }

    private async Task RunBackup(BackupJobInfoModel job) {
        if (_backupViewModel.CurrentBackupState == BackupState.Running) return;

        TextBlock textBlock = new TextBlock();
        await _backupViewModel.StartBackup(job.Name, job.Source, job.Destination, true, "", textBlock);

        BackupStatus = "État : En cours...";
    }

    private void PauseBackup() {
        if (_backupViewModel.CurrentBackupState == BackupState.Running) {
            _backupViewModel.PauseBackup();
            BackupStatus = "État : En pause";
        }
    }

    private void ResumeBackup() {
        if (_backupViewModel.CurrentBackupState == BackupState.Paused) {
            _backupViewModel.ResumeBackup();
            BackupStatus = "État : En cours...";
        }
    }

    private void StopBackup() {
        if (_backupViewModel.CurrentBackupState != BackupState.Stopped) {
            _backupViewModel.StopBackup();
            BackupStatus = "État : Stoppé";
        }
    }

    private async Task AddBackup() {
        var dialog = new ContentDialog {
            Title = "Nouvelle sauvegarde",
            Content = new StackPanel {
                Children =
                {
                    new TextBox { Header = "Nom", Name = "NameBox" },
                    new TextBox { Header = "Source", Name = "SourceBox" },
                    new TextBox { Header = "Destination", Name = "DestinationBox" }
                }
            },
            PrimaryButtonText = "Ajouter",
            CloseButtonText = "Annuler",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary) {
            var panel = (StackPanel)dialog.Content;
            var nameBox = (TextBox)panel.Children[0];
            var sourceBox = (TextBox)panel.Children[1];
            var destinationBox = (TextBox)panel.Children[2];

            if (!string.IsNullOrWhiteSpace(nameBox.Text) && !string.IsNullOrWhiteSpace(sourceBox.Text) && !string.IsNullOrWhiteSpace(destinationBox.Text)) {
                var newBackup = new BackupJobInfoModel(nameBox.Text, sourceBox.Text, destinationBox.Text);
                await _backupParallelService.AddBackupAsync(newBackup);
                SavedBackups.Add(newBackup);
            }
        }
    }
}
