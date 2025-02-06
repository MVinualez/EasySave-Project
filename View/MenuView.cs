using easysave_project.Controllers;
using System.ComponentModel;
using System.Text;

namespace easysave_project.View
{
    internal class MenuView {
        private readonly MainViewController _viewModel;
            
        public MenuView(MainViewController viewModel) {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(MainViewController.SelectedIndex)) {
                Display();
            }
        }

        public void Run() {
            while (_viewModel.IsRunning) {
                Display();
                HandleInput();
            }
        }

        private void Display() {
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;

            int maxNameLength = _viewModel.MenuActions.Max(a => a.name.Length);
            int padding = 6;
            int boxWidth = maxNameLength + padding;

            DrawBorder("╔", "═", "╗", boxWidth);
            DisplayTitle("EasySave Backup", boxWidth);
            DrawBorder("╠", "═", "╣", boxWidth);

            for (int i = 0; i < _viewModel.MenuActions.Count; i++) {
                string actionText = _viewModel.MenuActions[i].name.PadRight(maxNameLength);
                Console.Write("║ ");

                if (i == _viewModel.SelectedIndex) {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write($"→ {actionText} ");
                    Console.ResetColor();
                } else {
                    Console.Write($"  {actionText} ");
                }

                Console.WriteLine("  ║");
            }

            DrawBorder("╚", "═", "╝", boxWidth);
            Console.WriteLine("↑ ↓ : Naviguer | Entrée : Sélectionner");
        }

        private void DrawBorder(string left, string middle, string right, int width) {
            Console.WriteLine($"{left}{new string(middle[0], width)}{right}");
        }

        private void DisplayTitle(string title, int boxWidth) {
            int titlePadding = (boxWidth - title.Length) / 2;
            Console.WriteLine($"║{new string(' ', titlePadding)}{title}{new string(' ', boxWidth - title.Length - titlePadding)}║");
        }

        private void HandleInput() {
            var key = Console.ReadKey(true);

            switch (key.Key) {
                case ConsoleKey.UpArrow:
                    _viewModel.SelectedIndex = (_viewModel.SelectedIndex - 1 + _viewModel.MenuActions.Count) % _viewModel.MenuActions.Count;
                    break;
                case ConsoleKey.DownArrow:
                    _viewModel.SelectedIndex = (_viewModel.SelectedIndex + 1) % _viewModel.MenuActions.Count;
                    break;
                case ConsoleKey.Enter:
                    _viewModel.MenuActions[_viewModel.SelectedIndex].execute();
                    break;
            }
        }
    }
}
