namespace EasySave___WinUI.Models {
    internal class StateProgressModel {

        private int _filesRemaining;
        private int _fileSizeRemaining;
        private string _sourceFilePathInProgress = string.Empty;
        private string _targetFilePathInProgress = string.Empty;

        public int FilesRemaining { get; set; }
        public int FilesSizeRemaining { get; set; }
        public string SourceFilePathInProgress { get; set; }
        public string TargetFilePathInProgress { get; set; }

        public StateProgressModel(
            int filesRemaining,
            int fileSizeRemaining,
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