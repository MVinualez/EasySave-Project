using System;
using System.Globalization;
using System.Resources;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;
using EasySaveLibrary.Models;

namespace easysave_project.Controllers {
    internal class LanguageController {
        private ResourceManager _resourceManager;
        private string _currentLanguage = "en";

        public event EventHandler? LanguageChanged;

        public string CurrentLanguage {
            get => _currentLanguage;
            private set {
                if (_currentLanguage != value) {
                    _currentLanguage = value;
                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(_currentLanguage);
                    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(_currentLanguage);
                    _resourceManager = new ResourceManager($"easysave_project.Ressources.ressource_{_currentLanguage}", typeof(LanguageController).Assembly);
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        public LanguageController() {
            _resourceManager = new ResourceManager($"easysave_project.Ressources.ressource_{_currentLanguage}", typeof(LanguageController).Assembly);
        }

        public string GetResource(string name) {
            try {
                string? value = _resourceManager.GetString(name);
                if (value != null) {
                    return value;
                } else {
                    return $"Resource '{name}' not found";
                }
            } catch {
                return "";
            }

        }

        public string ShowParameterMenu() {
            int languageIndex = Array.IndexOf(new[] { "fr", "en", "es", "de" }, _currentLanguage);
            if (languageIndex == -1) languageIndex = 0;

            string _currentFileFormat = "JSON";
            int logParameterIndex = Array.IndexOf(new[] { "JSON", "XML" }, _currentFileFormat);
            if (logParameterIndex == -1) logParameterIndex = 0;

            ConsoleKey key;
            string[] languages = { "fr", "en", "es", "de", "Cancel" };
            string[] parameters = { "JSON", "XML" };

            do {
                Console.Clear();
                Console.WriteLine(GetResource("SelectLanguage"));

                for (int i = 0; i < languages.Length; i++) {
                    if (i == languageIndex) {
                        if (languages[i] == "Cancel") {
                            Console.ForegroundColor = ConsoleColor.Red;
                        } else {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.WriteLine($" ➜ {languages[i].ToUpper()}");
                        Console.ResetColor();
                    } else {
                        Console.WriteLine($"{languages[i].ToUpper()}");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && languageIndex > 0) {
                    languageIndex--;
                } else if (key == ConsoleKey.DownArrow && languageIndex < languages.Length - 1) {
                    languageIndex++;
                }

            } while (key != ConsoleKey.Enter);

            if (languages[languageIndex] == "Cancel") {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ " + GetResource("CancelLanguageSelection"));
                Console.ResetColor();
                WaitForKeyPress();
                return "JSON";
            }

            ChangeLanguage(languages[languageIndex]);

            do
            {
                Console.Clear();
                Console.WriteLine("🛠️Select the Log file format");

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == logParameterIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($" ➜ {parameters[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"   {parameters[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && logParameterIndex > 0)
                {
                    logParameterIndex--;
                }
                else if (key == ConsoleKey.DownArrow && logParameterIndex < parameters.Length - 1)
                {
                    logParameterIndex++;
                }

            } while (key != ConsoleKey.Enter);

            _currentFileFormat = parameters[logParameterIndex];
            Console.WriteLine($"📜 Format de log sélectionné : {_currentFileFormat}");
            return _currentFileFormat;
        }

        private void ChangeLanguage(string newLanguage, bool clear = true) {
            CurrentLanguage = newLanguage;

            if (GetResource("SelectLanguage") == "") {
                Console.Clear();
                Console.WriteLine("Requested language not found, use English");
                ChangeLanguage("en", false);
            } else {
                if (clear) {
                    Console.Clear();
                }
                Console.WriteLine("✅ " + GetResource("LanguageChanged") + $" {newLanguage.ToUpper()}");
                WaitForKeyPress();
            }


        }

        private void WaitForKeyPress() {
            Console.WriteLine(GetResource("ReturnToMenu"));
            Console.ReadKey();
        }
    }
}
