using easysave_project.Models;
using easysave_project.Services;

namespace easysave_project.Controllers
{
    internal class BackupJobController
    {
        private readonly BackupService _backupService;

        public BackupJobController(BackupService backupService)
        {
            _backupService = backupService;
        }

        public void StartBackup(string name, string source, string destination, bool isFullBackup)
        {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunBackup(job);
        }

        public void StartDiffBackup(string name, string source, string destination, bool isFullBackup) {
            var job = new BackupJob(name, source, destination, isFullBackup);
            _backupService.RunDifferentialBackup(job);

        }
    }
}
