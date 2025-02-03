using easysave_project.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace easysave_project.ViewModels {
    internal class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _selectedIndex;
        private bool _isRunning = true;
        private List<MenuAction> _menuActions = new();

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
