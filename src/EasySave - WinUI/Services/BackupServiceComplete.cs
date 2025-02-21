using EasySave___WinUI.CryptoSoft;
using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;

namespace EasySave___WinUI.Services {
    internal class BackupServiceComplete : BackupService {
        private static BackupServiceComplete? _instance;

        private BackupServiceComplete(XamlRoot xamlRoot) : base(xamlRoot) { }

        public static BackupServiceComplete GetBackupServiceCompleteInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupServiceComplete(xamlRoot);
            return _instance;
        }

        public override void RunBackup(string name, string source, string destination, bool isFullBackup) {
            BackupJob job = new BackupJob(name, source, destination, isFullBackup);

            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            string dirName = "Backup";
            string fullPathBackup = Path.Combine(path, dirName);
            if (!Directory.Exists(Path.Combine(path, dirName))) {
                Directory.CreateDirectory(Path.Combine(path, dirName));
            }

            Console.WriteLine($"Démarrage de la sauvegarde : {job.Name}");
            Console.WriteLine($"Source : {job.Source}");
            Console.WriteLine($"Destination : {job.Destination}");

            try {
                if (!Directory.Exists(job.Source)) {
                    Console.WriteLine("⚠️ Dossier source introuvable !");
                    return;
                }

                Directory.CreateDirectory(job.Destination);

                string[] files = Directory.GetFiles(job.Source);

                foreach (var file in files) {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);
                    string destFileBackup = Path.Combine(fullPathBackup, fileName);
                    CopyDirectoryReccursively(job.Name, job.Source, job.Destination, job.IsFullBackup);
                    CopyDirectoryReccursively(job.Name, job.Source, fullPathBackup, job.IsFullBackup);
                    //Encrypt_Recursively(destFile, key);  
                    File.Copy(file, destFileBackup, true);
                    Console.WriteLine($"✅ {fileName} copié !");
                    Console.WriteLine($"✅ {fileName} copié dans le Backup !");
                }

                var fileManager = new FileManager(job.Destination, [".docx", ".txt"], EncryptionKey);
                fileManager.Transform();
                Console.WriteLine("🎉 Sauvegarde terminée !");
            } catch (Exception ex) {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        public override void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup) {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                string targetSubDir = dir.Replace(source, target);
                Directory.CreateDirectory(targetSubDir);
            }

            foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                string destFile = file.Replace(source, target);
                File.Copy(file, destFile, true);
                Console.WriteLine($"✅ {file} → {destFile}");
            }
        }
    }
}
