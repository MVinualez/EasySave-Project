using easysave_project.Models;
using easysave_project.Controllers;
using System;
using System.IO;
using System.Xml.Linq;

namespace easysave_project.Services
{
    internal class BackupService
    {
        LogController logController = new LogController();
        LogEntry logEntry;
        
        public void RunBackup(BackupJob job)
        {
            Console.WriteLine($"Démarrage de la sauvegarde : {job.Name}");
            Console.WriteLine($"Source : {job.Source}");
            Console.WriteLine($"Destination : {job.Destination}");

            try
            {
                if (!Directory.Exists(job.Source))
                {
                    Console.WriteLine("⚠️ Dossier source introuvable !");
                    return;
                }

                Directory.CreateDirectory(job.Destination);

                string[] files = Directory.GetFiles(job.Source);

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);
                    File.Copy(file, destFile, true);
                    Console.WriteLine($"✅ {fileName} copié !");
                }

                Console.WriteLine("🎉 Sauvegarde terminée !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        public void RunDifferentialBackup(BackupJob job)
        {
            Console.WriteLine($"🔄 Démarrage de la sauvegarde différentielle : {job.Name}");
            Console.WriteLine($"📂 Source : {job.Source}");
            Console.WriteLine($"💾 Destination : {job.Destination}");

            try
            {
                if (!Directory.Exists(job.Source))
                {
                    Console.WriteLine("⚠️ Dossier source introuvable !");
                    return;
                }

                Directory.CreateDirectory(job.Destination);

                string[] files = Directory.GetFiles(job.Source);
                string[] filesDestination = Directory.GetFiles(job.Destination);

                HashSet<string> existingFiles = new HashSet<string>(filesDestination.Select(Path.GetFileName));

                int copiedFiles = 0;

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);

                    if (!existingFiles.Contains(fileName) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
                    {
                        File.Copy(file, destFile, true);
                        Console.WriteLine($"✅ {fileName} copié !");
                        copiedFiles++;
                    }
                }

                if (copiedFiles == 0)
                {
                    Console.WriteLine("✨ Aucun fichier n'a été modifié, rien à copier.");
                }
                else
                {
                    Console.WriteLine($"🎉 Sauvegarde terminée ! {copiedFiles} fichiers copiés.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }


    }
}
