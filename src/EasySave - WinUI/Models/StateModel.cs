using Newtonsoft.Json;
using System.IO;

namespace easysave_project.Models {
    internal class StateModel(
        string jobName,
        int timestamp,
        string state,
        int numberOfFilesRemaining,
        int totalFilesSize,
        StateProgressModel progress
    ) {
        public string JobName { get; set; } = jobName;
        public int Timestamp { get; set; } = timestamp;
        public string State { get; set; } = state;
        public int NumberOfFilesRemaining { get; set; } = numberOfFilesRemaining;
        public int TotalFilesSize { get; set; } = totalFilesSize;
        public StateProgressModel Progress { get; set; } = progress;

        public void SaveToFile(string filePath) {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}