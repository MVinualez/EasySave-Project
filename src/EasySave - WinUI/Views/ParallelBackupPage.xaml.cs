using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Views {
    public sealed partial class ParallelBackupPage : Page {
        public ParallelBackupViewModel ViewModel { get; }

        public ParallelBackupPage() {
            this.InitializeComponent();
            ViewModel = new ParallelBackupViewModel();
            this.DataContext = ViewModel;
        }
    }
}
