﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using EasySaveLibrary.Models;

namespace EasySaveLibrary.Controllers
{
    public class LogController
    {
        private readonly string logDirectory = "Logs"; // Log location, depending on where the .exe is executed

        public LogController()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }   
        }
        // LogController method of the LogController class, which uses the Exists method from the System.IO.Directory class 
        // and takes the logDirectory variable as a parameter
        // Checks if the Logs folder exists, and creates it if not.
        public void SaveLog(LogEntry log) // Creation of the SaveLog method, which calls the LogEntry method from /Models/LogEntry
        {
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.json"; 
            string logFilePath = Path.Combine(logDirectory, logFileName);

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
