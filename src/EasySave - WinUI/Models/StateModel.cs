using Newtonsoft.Json;

namespace EasySave___WinUI.Models {
    internal class StateModel {
        private string _jobName = string.Empty;
        private int _timestamp;
        private string _state = string.Empty;
        private int _numberOfFilesRemaining;
        private int _totalFilesSize;
        private StateProgressModel _progress;


        // ------ Getters / Setters ------
        public string JobName {
            get { return _jobName; }
            set { _jobName = value; }
        }

        public int Timestamp {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public string State {
            get { return _state; }
            set { _state = value; }
        }

        public int NumberOfFilesRemaining {
            get { return _numberOfFilesRemaining; }
            set { _numberOfFilesRemaining = value; }
        }

        public int TotalFilesSize {
            get { return _totalFilesSize; }
            set { _totalFilesSize = value; }
        }

        public StateProgressModel Progress {
            get { return _progress; }
            set { _progress = value; }
        }
        // -------------------------------

        public StateModel(string jobName, int timestamp, string state, int numberOfFilesRemaining, int totalFilesSize, StateProgressModel progress) {
            JobName = jobName;
            Timestamp = timestamp;
            State = state;
            NumberOfFilesRemaining = numberOfFilesRemaining;
            TotalFilesSize = totalFilesSize;
            Progress = progress;

            _progress = progress ?? throw new ArgumentNullException(nameof(progress), "Progress cannot be null");
        }

        public void SaveToFile(string filePath) {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}