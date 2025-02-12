using easysave_project.Models;
using easysave_project.Controllers;
using EasySaveLibrary.Controllers;
using EasySaveLibrary.Models;
using System;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace easysave_project.Services
{
    internal class BackupService
    {
        LogController logController = new LogController() ;
        LogEntry logEntry;
        
        public void RunBackup(BackupJob job)
        {
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            string dirName = "Backup";
            string fullPathBackup = Path.Combine(path, dirName);
            if (!Directory.Exists(Path.Combine(path, dirName)))
            {
                Directory.CreateDirectory(Path.Combine(path, dirName));
            }

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
                    string destFileBackcup = Path.Combine(fullPathBackup, fileName);
                    CopyDirectoryRecursively(job.Source, job.Destination);
                    CopyDirectoryRecursively(job.Source, fullPathBackup);
                    File.Copy(file, destFileBackcup, true);
                    Console.WriteLine($"✅ {fileName} copié !");
                    Console.WriteLine($"✅ {fileName} copié dans el Backup !");
                }

                Console.WriteLine("🎉 Sauvegarde terminée !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        private void CopyDirectoryRecursively(string sourceDir, string targetDir)
        {
            foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                string targetSubDir = dir.Replace(sourceDir, targetDir);
                Directory.CreateDirectory(targetSubDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                string destFile = file.Replace(sourceDir, targetDir);
                File.Copy(file, destFile, true);
                Console.WriteLine($"✅ {file} → {destFile}");
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
                        CopyModifiedFilesRecursively(job.Source, job.Destination);
                        Console.WriteLine($"✅ {fileName} copié !");
                        copiedFiles++;
                    }
                }

                if (copiedFiles == 0)
                {
                    Console.WriteLine("✨ Aucun nouveau fichier, rien à bouger.");
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

        private void CopyModifiedFilesRecursively(string sourceDir, string targetDir)
        {
            foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                string targetSubDir = dir.Replace(sourceDir, targetDir);
                if (!Directory.Exists(targetSubDir))
                {
                    Directory.CreateDirectory(targetSubDir);
                }
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                string destFile = file.Replace(sourceDir, targetDir);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
                {
                    File.Copy(file, destFile, true);
                    Console.WriteLine($"✅ {file} → {destFile}");
                }
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
