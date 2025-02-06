using easysave_project.View;
using easysave_project.ViewModels;
using easysave_project.Controllers;
using easysave_project.Models;
using EasySaveLibrary.Controllers;
using EasySaveLibrary.Models;

class Program
{
    static void Main(String[] args)
    {
        //var viewModel = new MainViewModel();
        //var menuView = new MenuView(viewModel);
        //menuView.Run();

        //// Initialisation du contrôleur de logs
        LogController logController = new LogController();

        //////// Création d'un logEntry
        LogEntry log = new LogEntry(
           "TestBackup44",
            "C:\\source\\file.txt",
            "C:\\destination\\file.txt",
            2048,
            2.5
        );
        ////////// Sauvegarde du log
        logController.SaveLog(log);

        Console.WriteLine("✅ Log sauvegardé avec succès ! Vérifie le dossier 'Logs/'.");

        //var logentry = new LogEntry("Test_Backup344", "c:\\source\\test", "", 1024, 6.2);
        //Console.WriteLine("📝 LogEntry Test");
        //Console.WriteLine($"Nom : {logentry.Name}");
        //Console.WriteLine($"Source : {logentry.FileSource}");
        //Console.WriteLine($"Destination : {logentry.FileTarget}");
        //Console.WriteLine($"Taille : {logentry.FileSize} octets");
        //Console.WriteLine($"Temps : {logentry.FileTransferTime} sec");
        //Console.WriteLine($"Horodatage : {logentry.Time}");

    }
}
