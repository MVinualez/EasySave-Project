namespace easysave_project.Models {
    internal class StateProgressModel {

        private int _filesRemaining;
        private int _fileSizeRemaining;
        private string _sourceFilePathInProgress = string.Empty;
        private string _targetFilePathInProgress = string.Empty;


        // ------ Getters / Setters ------
        public int FilesRemaining {
            get { return _filesRemaining; }
            set { _filesRemaining = value; }
        }

        public int FileSizeRemaining {
            get { return _fileSizeRemaining; }
            set { _fileSizeRemaining = value; }
        }

        public string SourceFilePathInProgress {
            get { return _sourceFilePathInProgress; }
            set { _sourceFilePathInProgress = value; }
        }

        public string TargetFilePathInProgress {
            get { return _targetFilePathInProgress; }
            set { _targetFilePathInProgress = value; }
        }
        // -------------------------------

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