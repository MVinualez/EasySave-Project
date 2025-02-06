using easysave_project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace easysave_project.Controller {
    internal class MainViewController : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _selectedIndex;
        private bool _isRunning = true;
        private List<MenuAction> _menuActions = new();
        private readonly LanguageController _languageController;

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

        public MainViewController() {
            _languageController = new LanguageController();
            _languageController.LanguageChanged += (sender, args) => InitializeMenuActions();
            InitializeMenuActions();
        }

        private void InitializeMenuActions() {
            _menuActions = new List<MenuAction>
            {
                new MenuAction(_languageController.GetResource("Backup"), ExecuteBackup),
                new MenuAction(_languageController.GetResource("Restore"), ExecuteRestore),
                new MenuAction(_languageController.GetResource("ChangeLanguage"), _languageController.ShowLanguageMenu),
                new MenuAction(_languageController.GetResource("Exit"), () => IsRunning = false),
            };
        }

        private void ExecuteBackup() {
            Console.Clear();
            Console.WriteLine("🚀 " + _languageController.GetResource("Backup"));
            WaitForKeyPress();
        }

        private void ExecuteRestore() {
            Console.Clear();
            Console.WriteLine("🔄 " + _languageController.GetResource("Restore"));
            WaitForKeyPress();
        }

        private void WaitForKeyPress() {
            Console.WriteLine(_languageController.GetResource("ReturnToMenu"));
            Console.ReadKey();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}
