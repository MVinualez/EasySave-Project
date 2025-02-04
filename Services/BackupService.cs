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

        public void RunRestauration(BackupJob job)
        {
            Console.WriteLine($"🔄 Démarrage de la restauration : {job.Name}");

            if (!Directory.Exists(job.Destination))
            {
                Console.WriteLine("⚠️ Aucune sauvegarde trouvée à cet emplacement !");
                return;
            }

            try
            {
                Directory.CreateDirectory(job.Source);

                string[] backupFiles = Directory.GetFiles(job.Destination);
                int restoredFiles = 0;

                foreach (var backupFile in backupFiles)
                {
                    string fileName = Path.GetFileName(backupFile);
                    string originalFile = Path.Combine(job.Source, fileName);

                    File.Copy(backupFile, originalFile, true);
                    Console.WriteLine($"✅ {fileName} restauré !");
                    restoredFiles++;
                }

                if (restoredFiles == 0)
                {
                    Console.WriteLine("✨ Aucun fichier à restaurer.");
                }
                else
                {
                    Console.WriteLine($"🎉 Restauration terminée ! {restoredFiles} fichiers restaurés.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la restauration : {ex.Message}");
            }

        }


    }
}
