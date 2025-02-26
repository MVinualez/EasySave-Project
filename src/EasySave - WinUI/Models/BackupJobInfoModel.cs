using System.Text.Json.Serialization;

namespace EasySave___WinUI.Models {
    public class BackupJobInfoModel {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Source")]
        public string Source { get; set; }

        [JsonPropertyName("Destination")]
        public string Destination { get; set; }

        public BackupJobInfoModel(string name, string source, string destination) { 
            Name = name; 
            Source = source; 
            Destination = destination;
        }
    }
}