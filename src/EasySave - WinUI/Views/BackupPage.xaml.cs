using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using EasySave___WinUI.Services;
using EasySave___WinUI.Controllers;
using EasySaveLibrary.Controllers;
using System.Diagnostics;
using EasySaveLibrary.Models;
using Windows.ApplicationModel.Resources;
using EasySave___WinUI.ViewModels;

namespace EasySave___WinUI.Views;

public sealed partial class BackupPage : Page
{
    private BackupJobController _backupJobController;
    private readonly LogController _logController;
    private readonly ResourceLoader _resourceLoader;
    private readonly NotificationViewModel _notificationViewModel;
    private readonly StateViewModel _stateViewModel;

    public BackupPage()
    {
        InitializeComponent();

        _resourceLoader = new ResourceLoader();
        _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
        _stateViewModel = StateViewModel.GetStateViewModelInstance(this.XamlRoot);
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
            await _notificationViewModel.ShowPopupDialog(_resourceLoader.GetString("Backup_OfficeCanceled"), _resourceLoader.GetString("Backup_OfficeCanceled"), String.Empty, "OK", this.XamlRoot);
            return;
        }

        var backupName = BackupNameTextBox?.Text ?? "";
        var sourcePath = SourcePathText?.Text;
        var destinationPath = DestinationPathText?.Text;

        DirectoryInfo di = new DirectoryInfo(sourcePath);
        long fileSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);

        if (string.IsNullOrWhiteSpace(backupName) || sourcePath == _resourceLoader.GetString("BackupPage_NoFolderSelected") || destinationPath == _resourceLoader.GetString("BackupPage_NoFolderSelected"))
        {
            await _notificationViewModel.ShowPopupDialog(_resourceLoader.GetString("BackupPage_FillAllFieldsError"), _resourceLoader.GetString("BackupPage_FillAllFieldsError"), String.Empty, "OK", this.XamlRoot);
            return;
        }

        try
        {
            stateCreator(backupName, sourcePath, destinationPath);

            Stopwatch stopwatch = Stopwatch.StartNew();
            if (DifferentialBackupRadioButton.IsChecked == true)
            {
                _backupJobController.StartDiffBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true);
            }
            else
            {
                _backupJobController.StartBackup(backupName, sourcePath, destinationPath, CompleteBackupRadioButton.IsChecked ?? true);
            }
            stopwatch.Stop();

            double elapsedTime = stopwatch.Elapsed.TotalSeconds;
            LogEntry logEntry = new LogEntry(backupName, sourcePath, destinationPath, fileSize, elapsedTime);
            _logController.SaveLog(logEntry);
        }
        catch (Exception ex)
        {
            await _notificationViewModel.ShowPopupDialog($"{_resourceLoader.GetString("BackupPage_BackupError")} {ex.Message}", $"{_resourceLoader.GetString("BackupPage_BackupError")} {ex.Message}", String.Empty, "OK", this.XamlRoot);
        }
    }

    // Lancer la sauvegarde
  

    private async void stateCreator(string backupName, string sourcePath, string destinationPath)
    {
        _stateViewModel.RegisterJobState(backupName);
        if (!Directory.Exists(sourcePath))
        {
            await _notificationViewModel.ShowPopupDialog(_resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"), _resourceLoader.GetString("BackupPage_SourceFolderDoesntExists"), String.Empty, "OK", this.XamlRoot);
            return;
        }
        string[] files = Directory.GetFiles(sourcePath);

        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationPath, fileName);
            long fileSize = new FileInfo(file).Length;
            int fileSizeInt = (int)fileSize;

            _stateViewModel.TrackFileInState(backupName, file, destFile, fileSizeInt);
            ProgressTextBox.Text = String.Format(_resourceLoader.GetString("BackupPage_BackupInProgress"), fileName);
            //File.Copy(file, destFile, true);

            _stateViewModel.MarkFileAsProcessed(backupName, file, fileSizeInt);
        }
        _stateViewModel.CompleteJobState(backupName);
        ProgressTextBox.Text = _resourceLoader.GetString("BackupPage_BackupFinished");
    }
}
