namespace easysave_project.Models
{
    internal class BackupJob
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool IsFullBackup { get; set; } // true = full, false = differential

        public BackupJob(string name, string source, string destination, bool isFullBackup)
        {
            Name = name;
            Source = source;
            Destination = destination;
            IsFullBackup = isFullBackup;
        }
    }
}
