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

namespace EasySaveLibrary.Controllers
{
    public class LogController
    {
        private string? path;

        private readonly string logDirectory = "logs"; // Log location, depending on where the .exe is executed
        public string fullPath { get; set; }

        public LogController()
        {
            this.path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            this.path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            this.fullPath = Path.Combine(path, logDirectory);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }   
        }
        // LogController method of the LogController class, which uses the Exists method from the System.IO.Directory class 
        // and takes the logDirectory variable as a parameter
        // Checks if the Logs folder exists, and creates it if not.
        public void SaveLog(LogEntry log) // Creation of the SaveLog method, which calls the LogEntry method from /Models/LogEntry
        {
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.json"; 
            string logFilePath = Path.Combine(fullPath, logFileName);

            List<LogEntry> logEntries = new List<LogEntry>();

            // Load existing log if there is already one
            if (File.Exists(logFilePath))
            {
                string existingJson = File.ReadAllText(logFilePath);
                logEntries = JsonConvert.DeserializeObject<List<LogEntry>>(existingJson) ?? new List<LogEntry>();
            }

            // Add the new entry and save
            logEntries.Add(log);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logEntries, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
