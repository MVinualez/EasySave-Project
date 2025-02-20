using EasySave___WinUI.ViewModels;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Views;

public sealed partial class RestorePage : Page
{
    public RestoreViewModel ViewModel
    {
        get;
    }

    public RestorePage()
    {
        ViewModel = App.GetService<RestoreViewModel>();
        InitializeComponent();
    }

    private void StartRestore_Click(object sender, RoutedEventArgs e)
    {
        string backupName = BackupNameTextBox.Text;

        bool isFullRestore = CompleteRestoreRadioButton.IsChecked == true;

        RestoreService restoreService = new RestoreService();
        restoreService.RestoreBackup(backupName, isFullRestore);
    }

}
