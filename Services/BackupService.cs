using easysave_project.Models;
using System;
using System.IO;

namespace easysave_project.Services
{
    internal class BackupService
    {
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
    }
}
