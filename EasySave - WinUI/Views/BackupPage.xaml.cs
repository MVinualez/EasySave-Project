using EasySave___WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Views;

public sealed partial class BackupPage : Page
{
    public BackupViewModel ViewModel
    {
        get;
    }

    public BackupPage()
    {
        ViewModel = App.GetService<BackupViewModel>();
        InitializeComponent();
    }

    // Sélection du dossier source
    private async void SelectSourceFolder_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow); 
        InitializeWithWindow.Initialize(picker, hwnd);

        picker.FileTypeFilter.Add("*");
        StorageFolder folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            SourcePathText.Text = folder.Path;
        }
    }

    // Sélection du dossier destination
    private async void SelectDestinationFolder_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        picker.FileTypeFilter.Add("*");
        StorageFolder folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            DestinationPathText.Text = folder.Path;
        }
    }

    // Lancer la sauvegarde
    private void StartBackup_Click(object sender, RoutedEventArgs e)
    {
        var backupName = BackupNameTextBox?.Text;
        var sourcePath = SourcePathText?.Text;
        var destinationPath = DestinationPathText?.Text;

        if (string.IsNullOrWhiteSpace(backupName) || sourcePath == "Aucun dossier sélectionné" || destinationPath == "Aucun dossier sélectionné")
        {
            ShowMessage("Veuillez remplir tous les champs avant de lancer la sauvegarde.");
            return;
        }

        try
        {
            

            ShowMessage($"Sauvegarde '{backupName}' effectuée avec succès !");
        } catch (Exception ex)
        {
            ShowMessage($"Erreur lors de la sauvegarde : {ex.Message}");
        }
    }

    private async void ShowMessage(string message)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Information",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
