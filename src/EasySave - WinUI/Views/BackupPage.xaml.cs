using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using EasySave___WinUI.Services;
using EasySaveLibrary.Services;
using System.Diagnostics;
using EasySaveLibrary.Models;
using Windows.ApplicationModel.Resources;
using EasySave___WinUI.ViewModels;

namespace EasySave___WinUI.Views;

public sealed partial class BackupPage : Page
{
    private readonly BackupViewModel _backupViewModel;

    public BackupPage()
    {
        InitializeComponent();
        
        _backupViewModel = BackupViewModel.GetBackupViewModelInstance(this.XamlRoot);
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

    private async void StartBackup_Click(object sender, RoutedEventArgs e) {
        bool isFullBackup = CompleteBackupRadioButton.IsChecked ?? true;

        var backupName = BackupNameTextBox?.Text ?? "";
        var sourcePath = SourcePathText?.Text;
        var destinationPath = DestinationPathText?.Text;
        var encryptionKey = BackupEncryptionKeyTextBox?.Text;

        _backupViewModel.StartBackup(backupName, sourcePath, destinationPath, isFullBackup, encryptionKey, ProgressTextBox);
    }
}
