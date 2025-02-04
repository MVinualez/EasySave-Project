using easysave_project.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using easysave_project.Controllers;
using easysave_project.Services;

namespace easysave_project.ViewModels {
    internal class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _selectedIndex;
        private bool _isRunning = true;
        private List<MenuAction> _menuActions = new();
        private readonly BackupJobController _backupJobController;


        public int SelectedIndex {
            get => _selectedIndex;
            set {
                if (_selectedIndex != value) {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRunning {
            get => _isRunning;
            set {
                if (_isRunning != value) {
                    _isRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<MenuAction> MenuActions => _menuActions;

        public MainViewModel() {
            InitializeMenuActions();
            var backupService = new BackupService();
            _backupJobController = new BackupJobController(backupService);
    
        }

        private void InitializeMenuActions() {
            _menuActions = new List<MenuAction>
            {
                new MenuAction("Lancer une sauvegarde", ExecuteBackup),
                new MenuAction("Restaurer une sauvegarde", ExecuteRestore),
                new MenuAction("Quitter", () => IsRunning = false),
            };
        }

        private void ExecuteBackup() {
            Console.Clear();
            Console.WriteLine("🚀 Début de la sauvegarde...");
            Console.Write("📂 Entrez le chemin du dossier source : ");
            string sourcePath = Console.ReadLine() ?? "";

            Console.Write("💾 Entrez le chemin du dossier de destination : ");
            string destinationPath = Console.ReadLine() ?? "";

            Console.Write("🛠️ Type de sauvegarde (1 = complète, 2 = différentielle) : ");
            bool isFullBackup = (Console.ReadLine() ?? "1") == "1";

            _backupJobController.StartBackup("Sauvegarde utilisateur", sourcePath, destinationPath, isFullBackup);
            WaitForKeyPress();
        }

        private void ExecuteRestore() {
            Console.Clear();
            Console.WriteLine("🔄 Début de la restauration...");
            WaitForKeyPress();
        }

        private void WaitForKeyPress() {
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}
