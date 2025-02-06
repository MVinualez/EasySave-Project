using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace easysave_project.Services
{
    public class RestoreService
    {
        private readonly string _backupDirectory = "Backups";  
        private readonly string _restoreDirectory = "Restored_Backups";

        public void RestoreBackup(string backupName, bool isFullRestore)
        {
            string backupPath = Path.Combine("Backups", backupName);
            string restorePath = Path.Combine("Restored_Backups", $"{backupName}_{DateTime.Now:yyyyMMdd_HHmmss}");

            if (!Directory.Exists(backupPath))
            {
                Console.WriteLine("⚠️ La sauvegarde spécifiée n'existe pas !");
                return;
            }

            Directory.CreateDirectory(restorePath);

            Console.WriteLine(isFullRestore ? "🔄 Restauration complète en cours..." : "🔄 Restauration différentielle en cours...");

            CopyFilesRecursively(backupPath, restorePath);

            if (!isFullRestore)
            {
                string[] diffBackups = Directory.GetDirectories("Backups", $"{backupName}_diff_*");
                foreach (var diffBackup in diffBackups.OrderBy(Directory.GetCreationTime))
                {
                    CopyFilesRecursively(diffBackup, restorePath);
                }
            }

            Console.WriteLine("✅ Restauration terminée !");
        }

        private void RestoreFullBackup(string backupPath, string restorePath)
        {
            Console.WriteLine("🔄 Restauration complète en cours...");
            CopyFilesRecursively(backupPath, restorePath);
            Console.WriteLine("✅ Restauration complète terminée !");
        }

        private void RestoreDifferentialBackup(string backupName, string restorePath)
        {
            Console.WriteLine("🔄 Restauration différentielle en cours...");

            string fullBackupPath = FindLatestFullBackup(backupName);
            if (fullBackupPath == null)
            {
                Console.WriteLine("❌ Aucune sauvegarde complète trouvée !");
                return;
            }

            CopyFilesRecursively(fullBackupPath, restorePath);

            string[] differentialBackups = Directory.GetDirectories(_backupDirectory, $"{backupName}_diff_*");

            foreach (var diffBackup in differentialBackups.OrderBy(Directory.GetCreationTime))
            {
                CopyFilesRecursively(diffBackup, restorePath);
            }

            Console.WriteLine("✅ Restauration différentielle terminée !");
        }

        private string? FindLatestFullBackup(string backupName)
        {
            var fullBackups = Directory.GetDirectories(_backupDirectory, $"{backupName}_full_*")
                                       .OrderByDescending(Directory.GetCreationTime)
                                       .ToList();

            return fullBackups.FirstOrDefault();
        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                string destFilePath = filePath.Replace(sourcePath, targetPath);
                File.Copy(filePath, destFilePath, true);
            }
        }
    }
}
