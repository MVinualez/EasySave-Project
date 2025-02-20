using EasySave___WinUI.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave___WinUI.ViewModels {
    internal class NotificationViewModel {
        private static NotificationViewModel? _instance;
        NotificationService _notificationService;

        private NotificationViewModel() {
            _notificationService = NotificationService.GetNotificationServiceInstance();
        }

        public static NotificationViewModel GetNotificationViewModelInstance() {
            _instance ??= new NotificationViewModel();
            return _instance;
        }

        /// <summary>
        /// Displays a popup dialog to the user with a message and optional action buttons.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="closeButtonText"></param>
        /// <param name="primaryButtonText"></param>
        /// <param name="xamlRoot"></param>
        public async Task<bool> ShowPopupDialog(string title, string content, string closeButtonText, string primaryButtonText, XamlRoot xamlRoot) {
            return await _notificationService.ShowDialogMessage(title, content, closeButtonText, primaryButtonText, xamlRoot);
        }
    }
}
