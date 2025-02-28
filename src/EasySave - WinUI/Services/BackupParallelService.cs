using EasySave___WinUI.Models;
using System.Reflection;
using System.Text.Json;

namespace EasySave___WinUI.Services {
    class BackupParallelService {
        private readonly string FilePath;

        public BackupParallelService() {
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path ?? Directory.GetCurrentDirectory();

            string dirName = "parallelBackup";
            string fileName = "backups.json";
            string fullPath = Path.Combine(path, dirName, fileName);

            if (!Directory.Exists(Path.Combine(path, dirName)))
                Directory.CreateDirectory(Path.Combine(path, dirName));

            FilePath = fullPath;
        }

        public async Task<List<BackupJobInfoModel>> LoadBackupsAsync() {
            if (!File.Exists(FilePath))
                return new List<BackupJobInfoModel>();

            string json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<BackupJobInfoModel>>(json) ?? new List<BackupJobInfoModel>();
        }

        public async Task SaveBackupsAsync(List<BackupJobInfoModel> backups) {
            string json = JsonSerializer.Serialize(backups, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }

        public async Task AddBackupAsync(BackupJobInfoModel newBackup) {
            var backups = await LoadBackupsAsync();
            backups.Add(newBackup);
            await SaveBackupsAsync(backups);
        }
    }
}
