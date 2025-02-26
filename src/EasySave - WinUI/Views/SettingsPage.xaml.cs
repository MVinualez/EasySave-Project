using EasySave___WinUI.Services;
using EasySave___WinUI.ViewModels;
using EasySaveLibrary.Services;
using EasySaveLibrary.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Globalization;
using Windows.Storage;

namespace EasySave___WinUI.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    private LogEntryViewModel logEntryViewModel = LogEntryViewModel.GetLogEntryViewModelInstance();
    public SettingsViewModel ViewModel 
    {
        get;
    }
   
    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        this.Loaded += SettingsPage_Loaded;
        ExtensionsListView.SelectionChanged += ExtensionsListView_SelectionChanged;
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

    private void ExtensionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.SelectedExtensions.Clear();
        foreach (var item in ExtensionsListView.SelectedItems)
        {
            ViewModel.SelectedExtensions.Add(item.ToString());
        }
    }


        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        string currentFormat = logEntryViewModel.GetLogFormat();
        

        if (currentFormat == "JSON" || currentFormat == "XML")
        {
            JsonRadioButton.IsChecked = (currentFormat == "JSON");
            XmlRadioButton.IsChecked = (currentFormat == "XML");
        }
        else
        {
            // Default to JSON if format is not set
            JsonRadioButton.IsChecked = true;
            logEntryViewModel.SetLogFormat("JSON");
        }

        List<object> tempList = new List<object>();
        foreach (var ext in ViewModel.SelectedExtensions)
        {
            tempList.Add(ext);
        }

        ExtensionsListView.SelectedItems.Clear();

        foreach (var ext in tempList)
        {
            ExtensionsListView.SelectedItems.Add(ext);
        }

    }


    private void LogFormat_Checked(object sender, RoutedEventArgs e)
    {
        if (JsonRadioButton.IsChecked == true)
        {
            logEntryViewModel.SetLogFormat("JSON");
        }
        else if (XmlRadioButton.IsChecked == true)
        {
            logEntryViewModel.SetLogFormat("XML");
        }
    }


}
