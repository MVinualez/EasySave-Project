using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using EasySave___WinUI.Models;

namespace EasySave___WinUI.Services {
    internal class NotificationService {
        private static NotificationService? _instance;


        private NotificationService() {}

        public static NotificationService GetNotificationServiceInstance() {
            _instance ??= new NotificationService();
            return _instance;
        }

        public async Task<bool> ShowDialogMessage(string title, string content, string closeButtonText, string primaryButtonText, XamlRoot xamlRoot) {
            NotificationModel notificationModel = new NotificationModel(title, content, closeButtonText, primaryButtonText, xamlRoot); 
            
            ContentDialog dialog = new() {
                Title = notificationModel.Title,
                Content = notificationModel.Content,
                CloseButtonText = notificationModel.CloseButtonText,
                PrimaryButtonText = notificationModel.PrimaryButtonText,
                XamlRoot = notificationModel.XamlRoot,
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
