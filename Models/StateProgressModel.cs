namespace easysave_project.Models {
    internal class StateProgressModel {
        public int FilesRemaining { get; set; }
        public int FileSizeRemaining { get; set; }
        public string SourceFilePathInProgress { get; set; }
        public string TargetFilePathInProgress { get; set; }

        public StateProgressModel(
            int filesRemaining,
            int fileSizeRemaining,
            string sourceFilePathInProgress,
            string targetFilePathInProgress
        ) {
            FilesRemaining = filesRemaining;
            FileSizeRemaining = fileSizeRemaining;
            SourceFilePathInProgress = sourceFilePathInProgress;
            TargetFilePathInProgress = targetFilePathInProgress;
        }
    }
}