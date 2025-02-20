using Newtonsoft.Json;

namespace EasySave___WinUI.Models {
    internal class StateModel {
        public string JobName { get; set; }
        public int Timestamp { get; set; }
        public string State { get; set; }
        public int NumberOfFilesRemaining { get; set; }
        public int TotalFilesSize { get; set; }
        public StateProgressModel Progress { get; set; }

        public StateModel(string jobName, int timestamp, string state, int numberOfFilesRemaining, int totalFilesSize, StateProgressModel progress) {
            JobName = jobName;
            Timestamp = timestamp;
            State = state;
            NumberOfFilesRemaining = numberOfFilesRemaining;
            TotalFilesSize = totalFilesSize;
            Progress = progress;
        }

        public void SaveToFile(string filePath) {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}