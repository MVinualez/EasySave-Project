using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Services {
    internal class BackupServiceComplete : BackupService {
        private static BackupServiceComplete? _instance;
        private readonly NotificationViewModel _notificationViewModel;

        private BackupServiceComplete(XamlRoot xamlRoot) : base(xamlRoot) {
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
        }

        public static BackupServiceComplete GetBackupServiceCompleteInstance(XamlRoot xamlRoot) {
            _instance = new BackupServiceComplete(xamlRoot);
            return _instance;
        }

        // Toujours copier tous les fichiers (sauvegarde complète)
        protected override bool ShouldCopyFile(string sourceFile, string destFile) {
            return true;
        }
    }
}
