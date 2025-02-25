namespace EasySave___WinUI.Models {
    internal class StateProgressModel {

        private int _filesRemaining;
        private long _fileSizeRemaining;
        private string _sourceFilePathInProgress = string.Empty;
        private string _targetFilePathInProgress = string.Empty;

        public int FilesRemaining { get; set; }
        public long FilesSizeRemaining { get; set; }
        public string SourceFilePathInProgress { get; set; }
        public string TargetFilePathInProgress { get; set; }

        public StateProgressModel(
            int filesRemaining,
            long fileSizeRemaining,
            string sourceFilePathInProgress,
            string targetFilePathInProgress
        ) {
            FilesRemaining = filesRemaining;
            FilesSizeRemaining = fileSizeRemaining;
            SourceFilePathInProgress = sourceFilePathInProgress;
            TargetFilePathInProgress = targetFilePathInProgress;
        }
    }
}