using EasySave___WinUI.CryptoSoft;
using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;

namespace EasySave___WinUI.Services {
    internal class BackupServiceDifferential : BackupService {
        private static BackupServiceDifferential? _instance;

        private BackupServiceDifferential(XamlRoot xamlRoot) : base(xamlRoot) { }

        public static BackupServiceDifferential GetBackupServiceDifferentialInstance(XamlRoot xamlRoot) {
            _instance ??= new BackupServiceDifferential(xamlRoot);
            return _instance;
        }

        public override void RunBackup(string name, string source, string destination, bool isFullBackup) {
            BackupJob job = new BackupJob(name, source, destination, isFullBackup);

            Console.WriteLine($"🔄 Démarrage de la sauvegarde différentielle : {job.Name}");
            Console.WriteLine($"📂 Source : {job.Source}");
            Console.WriteLine($"💾 Destination : {job.Destination}");

            try {
                if (!Directory.Exists(job.Source)) {
                    Console.WriteLine("⚠️ Dossier source introuvable !");
                    return;
                }

                Directory.CreateDirectory(job.Destination);

                string[] files = Directory.GetFiles(job.Source);
                string[] filesDestination = Directory.GetFiles(job.Destination);

                HashSet<string> existingFiles = new HashSet<string>(filesDestination.Select(Path.GetFileName));

                int copiedFiles = 0;

                foreach (var file in files) {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(job.Destination, fileName);

                    if (!existingFiles.Contains(fileName) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile)) {
                        CopyDirectoryReccursively(job.Name, job.Source, job.Destination, job.IsFullBackup);
                        Console.WriteLine($"✅ {fileName} copié !");
                        copiedFiles++;
                    }
                }
                var fileManager = new FileManager(job.Destination, new List<string> { ".pdf", ".docx", ".txt" }, EncryptionKey);
                fileManager.Transform();
                if (copiedFiles == 0) {
                    Console.WriteLine("✨ Aucun nouveau fichier, rien à bouger.");
                } else {
                    Console.WriteLine($"🎉 Sauvegarde terminée ! {copiedFiles} fichiers copiés.");
                }
            } catch (Exception ex) {
                Console.WriteLine($"❌ Erreur : {ex.Message}");
            }
        }

        public override void CopyDirectoryReccursively(string name, string source, string target, bool isFullBackup) {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                string targetSubDir = dir.Replace(source, target);
                if (!Directory.Exists(targetSubDir)) {
                    Directory.CreateDirectory(targetSubDir);
                }
            }

            foreach (string file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                string destFile = file.Replace(source, target);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile)) {
                    File.Copy(file, destFile, true);
                    Console.WriteLine($"✅ {file} → {destFile}");
                }
            }
        }
    }
}
