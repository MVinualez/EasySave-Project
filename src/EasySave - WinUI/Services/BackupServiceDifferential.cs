using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Services {
    internal class BackupServiceDifferential : BackupService {
        private static BackupServiceDifferential? _instance;
        private readonly NotificationViewModel _notificationViewModel;

        private BackupServiceDifferential(XamlRoot xamlRoot) : base(xamlRoot) {
            _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
        }

        public static BackupServiceDifferential GetBackupServiceDifferentialInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupServiceDifferential(xamlRoot);
            return _instance;
        }

        // Copier uniquement si le fichier n'existe pas ou s'il est plus récent
        protected override bool ShouldCopyFile(string sourceFile, string destFile) {
            return !File.Exists(destFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destFile);
        }
    }
}
