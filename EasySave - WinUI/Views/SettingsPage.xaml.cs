using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;
using Windows.Storage;

namespace EasySave___WinUI.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageSelector.SelectedItem is ComboBoxItem selectedLanguage)
        {
            string languageTag = selectedLanguage.Tag.ToString();

            // Sauvegarde la langue sélectionnée
            ApplicationData.Current.LocalSettings.Values["AppLanguage"] = languageTag;

            // Applique la langue immédiatement
            ApplicationLanguages.PrimaryLanguageOverride = languageTag;

            // Redémarre l'application (ou recharge la fenêtre principale)
            ((App)Application.Current).RestartApp();
        }
    }
}
