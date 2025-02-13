using EasySave___WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using easysave_project.Services;
using easysave_project.Controllers;
using EasySaveLibrary.Controllers;
using System.Diagnostics;
using Windows.UI.StartScreen;
using EasySaveLibrary.Models;

namespace EasySave___WinUI.Views;

public sealed partial class BackupPage : Page
{

    private readonly BackupJobController _backupJobController;
    private readonly LogController _logController;

    public BackupViewModel ViewModel
    {
        get;
    }

    public BackupPage()
    {
        ViewModel = App.GetService<BackupViewModel>();
        InitializeComponent();

        var backupService = new BackupService();
        _backupJobController = new BackupJobController(backupService);
        _logController = new LogController();
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
        var backupName = BackupNameTextBox?.Text ?? "";
        var sourcePath = SourcePathText?.Text;
        var destinationPath = DestinationPathText?.Text;

        DirectoryInfo di = new DirectoryInfo(sourcePath);
        long fileSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);

        if (string.IsNullOrWhiteSpace(backupName) || sourcePath == "Aucun dossier sélectionné" || destinationPath == "Aucun dossier sélectionné")
        {
            ShowMessage("Veuillez remplir tous les champs avant de lancer la sauvegarde.");
            return;
        }

        try
        {
            stateCreator(backupName, sourcePath, destinationPath);

            if(DifferentialBackupRadioButton.IsChecked == true)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _backupJobController.StartDiffBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true);
                stopwatch.Stop();
                double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                LogEntry logEntry = new LogEntry(backupName, sourcePath, destinationPath, fileSize, elapsedTime);
                _logController.SaveLog(logEntry);
            } else
            {
                Stopwatch stopwatchCase2 = Stopwatch.StartNew();
                _backupJobController.StartBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true);
                stopwatchCase2.Stop();
                double elapsedTimeCase2 = stopwatchCase2.Elapsed.TotalSeconds;
                LogEntry logEntryCase2 = new LogEntry(backupName, sourcePath, destinationPath, fileSize, elapsedTimeCase2);
                _logController.SaveLog(logEntryCase2);
            }

            //ShowMessage($"Sauvegarde '{backupName}' effectuée avec succès !");
        } catch (Exception ex)
        {
            ShowMessage($"Erreur lors de la sauvegarde : {ex.Message}");
        }
    }

    private void stateCreator(string backupName, string sourcePath, string destinationPath)
    {
        StateService stateService = new StateService("state/state.json");

        stateService.GetCurrentStateFile();

        stateService.StartJob(backupName);
        if (!Directory.Exists(sourcePath))
        {
            ShowMessage("⚠️ Le dossier source n'existe pas.");
            return;
        }
        string[] files = Directory.GetFiles(sourcePath);

        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationPath, fileName);
            long fileSize = new FileInfo(file).Length;
            int fileSizeInt = (int)fileSize;

            stateService.AddFileToState(backupName, file, destFile, fileSizeInt);
            //ShowMessage($"📂 Transfert en cours : {fileName}");
            File.Copy(file, destFile, true);

            stateService.UpdateFileTransfer(backupName, file, fileSizeInt);
        }
        stateService.CompleteJob(backupName);

        //ShowMessage("🎉 Sauvegarde terminée et état mis à jour !");
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
