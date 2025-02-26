using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave___WinUI.Contracts.Services;
using Microsoft.UI.Xaml;
using EasySave___WinUI.Helpers;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace EasySave___WinUI.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly BackupViewModel _backupViewModel;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private ObservableCollection<string> _availableExtensions = new() {".iso", ".txt", ".pdf", ".docx", ".mp4" };

    [ObservableProperty]
    private ObservableCollection<string> _selectedExtensions = new();

    [ObservableProperty]
    private int _maxParallelSizeKb;
    public ICommand SwitchThemeCommand
    {
        get;
    }

    public ICommand SaveSettingsCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, BackupViewModel backupViewModel)
    {
        _themeSelectorService = themeSelectorService;
        _backupViewModel = BackupViewModel.GetBackupViewModelInstance(App.MainWindow.Content.XamlRoot);
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        _selectedExtensions = new ObservableCollection<string>(_backupViewModel.priorityExtensions);
        _maxParallelSizeKb = _backupViewModel.maxParallelSizeKb;

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        SaveSettingsCommand = new RelayCommand(() => {
            _backupViewModel.SetPriorityExtension(_selectedExtensions.ToList());
            _backupViewModel.SetMaxParallelSizeKb(_maxParallelSizeKb);
        });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}";
    }
}
