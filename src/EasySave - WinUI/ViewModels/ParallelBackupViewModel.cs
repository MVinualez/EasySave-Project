using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave___WinUI.Models;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.ViewModels;

public partial class ParallelBackupViewModel : ObservableRecipient {
    private readonly BackupParallelService _backupParallelService;

    public ObservableCollection<BackupJobInfoModel> SavedBackups { get; }

    public ICommand AddBackupCommand { get; }

    [ObservableProperty]
    private string backupStatus = "Aucune sauvegarde en cours";

    public ParallelBackupViewModel() {
        _backupParallelService = new BackupParallelService();
        SavedBackups = new ObservableCollection<BackupJobInfoModel>();
        AddBackupCommand = new RelayCommand(async () => await AddBackup());

        LoadBackups();
    }

    private async void LoadBackups() {
        var backups = await _backupParallelService.LoadBackupsAsync();
        SavedBackups.Clear();
        foreach (var job in backups)
            SavedBackups.Add(job);
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
