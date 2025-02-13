using EasySave___WinUI.ViewModels;

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
}
