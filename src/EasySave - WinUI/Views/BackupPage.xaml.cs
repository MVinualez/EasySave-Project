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
using Windows.ApplicationModel.Resources;

namespace EasySave___WinUI.Views;

public sealed partial class BackupPage : Page
{
    private BackupJobController _backupJobController;
    private readonly LogController _logController;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private BackupService backupService;

    public BackupPage()
    {
        InitializeComponent();

        this._logController = LogController.GetInstanceLogController();
    }

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

    private async void StartBackup_Click(object sender, RoutedEventArgs e)
    {
        BackupService backupService = BackupService.getInstanceBackupService();
        backupService.encryptionKey = BackupEncryptionKeyTextBox.Text;
        this._backupJobController = new BackupJobController(backupService);

        // Check if Office apps are running before proceeding
        bool canStart = await _backupJobController.CanStartBackup(this.XamlRoot);
        if (!canStart)
        {
            ShowMessage("Backup canceled due to running Office applications.");
            return;
        }

        var backupName = BackupNameTextBox?.Text ?? "";
        var sourcePath = SourcePathText?.Text;
        var destinationPath = DestinationPathText?.Text;

        DirectoryInfo di = new DirectoryInfo(sourcePath);
        long fileSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);

        if (string.IsNullOrWhiteSpace(backupName) || sourcePath == _resourceLoader.GetString("BackupPage_NoFolderSelected") || destinationPath == _resourceLoader.GetString("BackupPage_NoFolderSelected"))
        {
            string errorMessage = _resourceLoader.GetString("BackupPage_FillAllFieldsError");
            ShowMessage(errorMessage);
            return;
        }

        try
        {
            // j'ai mis des commentaires car cette méthode copie les fichiers mais sans les chiffrer  |  stateCreator(backupName, sourcePath, destinationPath);

            Stopwatch copyStopwatch = Stopwatch.StartNew();
            Stopwatch encryptionStopwatch = new Stopwatch();
            if (DifferentialBackupRadioButton.IsChecked == true)
            {
                _backupJobController.StartDiffBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true,copyStopwatch, encryptionStopwatch);
            }
            else
            {
                _backupJobController.StartBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true, copyStopwatch, encryptionStopwatch);
            }
            double elapsedTime = copyStopwatch.Elapsed.TotalSeconds;
            double encryptionTime = encryptionStopwatch.Elapsed.TotalSeconds;


            LogEntry logEntry = new LogEntry(backupName, sourcePath, destinationPath, fileSize, elapsedTime, encryptionTime);
            _logController.SaveLog(logEntry);
        }
        catch (Exception ex)
        {
            ShowMessage($"{_resourceLoader.GetString("BackupPage_BackupError")} {ex.Message}");
        }
        ProgressTextBox.Text = _resourceLoader.GetString("BackupPage_BackupFinished");

    }

    // Lancer la sauvegarde


    private void stateCreator(string backupName, string sourcePath, string destinationPath)
    {
        StateService stateService = new StateService("state/state.json");

        stateService.GetCurrentStateFile();

        stateService.StartJob(backupName);
        if (!Directory.Exists(sourcePath))
        {
            ShowMessage(_resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"));
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
            ProgressTextBox.Text = String.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName);
            //File.Copy(file, destFile, true);

            stateService.UpdateFileTransfer(backupName, file, fileSizeInt);
        }
        stateService.CompleteJob(backupName);
        ProgressTextBox.Text = _resourceLoader.GetString("BackupPage_BackupFinished");
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
