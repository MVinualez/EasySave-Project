using easysave_project.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using easysave_project.Services;
using System.Diagnostics;
using EasySaveLibrary.Controllers;
using EasySaveLibrary.Models;

namespace easysave_project.Controllers {
    internal class MainViewController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _selectedIndex;
        private bool _isRunning = true;
        private List<MenuAction> _menuActions = new();
        private readonly BackupJobController _backupJobController;
        private readonly LogController _logController;
        private readonly LanguageController _languageController;


        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<MenuAction> MenuActions => _menuActions;

        public MainViewController()
        {
            _languageController = new LanguageController();
            _languageController.LanguageChanged += (sender, args) => InitializeMenuActions();

            InitializeMenuActions();
            var backupService = new BackupService("");
            _backupJobController = new BackupJobController(backupService);
            _logController = new LogController();


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

        public void stateCreator(string backupJobName, string sourcePath, string destinationPath)
        {
            StateService stateService = new StateService("state/state.json");

            stateService.GetCurrentStateFile();
           
            stateService.StartJob(backupJobName);
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("⚠️ Le dossier source n'existe pas.");
                return;
            }
            string[] files = Directory.GetFiles(sourcePath);

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationPath, fileName);
                long fileSize = new FileInfo(file).Length;
                int fileSizeInt = (int)fileSize;

                stateService.AddFileToState(backupJobName, file, destFile, fileSizeInt);
               
                Console.WriteLine($"📂 Transfert en cours : {fileName}");
                File.Copy(file, destFile, true);

                stateService.UpdateFileTransfer(backupJobName, file, fileSizeInt);
            }
            stateService.CompleteJob(backupJobName);

            Console.WriteLine("🎉 Sauvegarde terminée et état mis à jour !");
        


    }

        private void ExecuteBackup()
        {
            Console.Clear();
            Console.WriteLine("🚀 Début de la sauvegarde...");
            Console.WriteLine("Entrez le nom de la sauvegarde");
            string nomSauvegarde = Console.ReadLine() ?? "";

            Console.Write("📂 Entrez le chemin du dossier source : ");
            string sourcePath = Console.ReadLine() ?? "";
            DirectoryInfo di = new DirectoryInfo(sourcePath);
            long fileSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);

            Console.Write("💾 Entrez le chemin du dossier de destination : ");
            string destinationPath = Console.ReadLine() ?? "";

            Console.Write("🛠️ Type de sauvegarde (1 = complète, 2 = différentielle) : ");
            bool isFullBackup = (Console.ReadLine() ?? "1") == "1";

            stateCreator(nomSauvegarde, sourcePath, destinationPath);


            switch (Console.ReadLine())
            {

                case "2":
                    
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    _backupJobController.StartDiffBackup(nomSauvegarde, sourcePath, destinationPath, isFullBackup);
                    WaitForKeyPress();
                    stopwatch.Stop();
                    double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                    LogEntry logEntry = new LogEntry(nomSauvegarde, sourcePath, destinationPath, fileSize, elapsedTime);
                    _logController.SaveLog(logEntry);
                    break;

                default:

                    Stopwatch stopwatchCase2 = Stopwatch.StartNew();
                    _backupJobController.StartBackup(nomSauvegarde, sourcePath, destinationPath, isFullBackup);
                    WaitForKeyPress();
                    stopwatchCase2.Stop();
                    double elapsedTimeCase2 = stopwatchCase2.Elapsed.TotalSeconds;
                    LogEntry logEntryCase2 = new LogEntry(nomSauvegarde, sourcePath, destinationPath, fileSize, elapsedTimeCase2);
                    _logController.SaveLog(logEntryCase2);
                    break;


            }

        }

        private void ExecuteRestore()
        {
            Console.Clear();
            Console.WriteLine("🔄 Début de la restauration...");

            Console.Write("📂 Nom de la sauvegarde à restaurer : ");
            string backupName = Console.ReadLine() ?? "";

            Console.Write("🛠️ Type de restauration (1 = complète, 2 = différentielle) : ");
            bool isFullRestore = (Console.ReadLine() ?? "1") == "1";

            RestoreService restoreService = new RestoreService();
            restoreService.RestoreBackup(backupName, isFullRestore);

            WaitForKeyPress();
        }

        private void WaitForKeyPress()
        {
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}
