using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using EasySaveLibrary.Models;
using System.Reflection;
using System.Xml.Serialization;
using Formatting = Newtonsoft.Json.Formatting;
using System.Diagnostics;

namespace EasySaveLibrary.Services
{
    public class LogService
    {
        private static LogService? _instance;
        private string? path;
        private readonly string logDirectory = "logs"; // Log location, depending on where the .exe is executed
        public string fullPath { get; set; }
        public string LogFormat { get; set; } = "JSON"; // Default format

        private LogService()
        {
            this.path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            this.path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            this.fullPath = Path.Combine(path, logDirectory);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public static LogService GetLogServiceInstance()
        {
            _instance ??= new LogService();
            return _instance;
        }

        // LogController method of the LogController class, which uses the Exists method from the System.IO.Directory class 
        // and takes the logDirectory variable as a parameter
        // Checks if the Logs folder exists, and creates it if not.
        public void SaveLog(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime)
        {
            LogEntryModel log = new LogEntryModel(name, fileSource, fileTarget, fileSize, fileTransferTime);
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.{(LogFormat == "JSON" ? "json" : "xml")}";
            string logFilePath = Path.Combine(fullPath, logFileName);

            Directory.CreateDirectory(fullPath);

            List<LogEntryModel> logEntries = LoadExistingLogs(logFilePath);

            logEntries.Add(log);

            SaveLogs(logFilePath, logEntries);
        }

        private List<LogEntryModel> LoadExistingLogs(string logFilePath)
        {
            if (!File.Exists(logFilePath))
            {
                Debug.WriteLine($"Log file not found: {logFilePath}");
                return new List<LogEntryModel>();
            }

            string existingLog = File.ReadAllText(logFilePath);
            Debug.WriteLine($"Log file loaded: {logFilePath}");

            try
            {
                return LogFormat == "JSON"
                    ? JsonConvert.DeserializeObject<List<LogEntryModel>>(existingLog) ?? new List<LogEntryModel>()
                    : DeserializeXml<List<LogEntryModel>>(existingLog) ?? new List<LogEntryModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading log file: {ex.Message}");
                return new List<LogEntryModel>();
            }
        }


        private void SaveLogs(string logFilePath, List<LogEntryModel> logEntries)
        {
            if (LogFormat == "JSON")
            {
                File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logEntries, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                SaveXmlLog(logFilePath, logEntries);
            }
        }


        private void SaveXmlLog(string logFilePath, List<LogEntryModel> logEntries)
        {
            try
            {
                using (var writer = new StreamWriter(logFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryModel>));
                    serializer.Serialize(writer, logEntries);
                }
                Debug.WriteLine($"XML log successfully written: {logFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing XML log: {ex.Message}");
            }
        }


        private T? DeserializeXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(xml);
            return (T?)serializer.Deserialize(reader);
        }

    }
}
