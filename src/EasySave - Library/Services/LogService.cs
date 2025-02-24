﻿using System;
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
        public void SaveLog(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime) // Creation of the SaveLog method, which calls the LogEntry method from /Models/LogEntry
        {
            LogEntryModel log = new LogEntryModel(name, fileSource, fileTarget, fileSize, fileTransferTime);
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.{(LogFormat == "JSON" ? "json" : "xml")}";
            string logFilePath = Path.Combine(fullPath, logFileName);

            List<LogEntryModel> logEntries = new List<LogEntryModel>();

            // Load existing log if there is already one
            if (File.Exists(logFilePath))
            {
                /*  string existingJson = File.ReadAllText(logFilePath);
                  logEntries = JsonConvert.DeserializeObject<List<LogEntryModel>>(existingJson) ?? new List<LogEntryModel>();
                */
                string existingLog = File.ReadAllText(logFilePath);
                logEntries = LogFormat == "JSON"
                    ? JsonConvert.DeserializeObject<List<LogEntryModel>>(existingLog) ?? new List<LogEntryModel>()
                    : DeserializeXml<List<LogEntryModel>>(existingLog) ?? new List<LogEntryModel>();
            }

            logEntries.Add(log);
            if (LogFormat == "JSON")
            {
                File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logEntries, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                File.WriteAllText(logFilePath, SerializeXml(logEntries));
            }
        }

        private string SerializeXml<T>(T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, data);
            return writer.ToString();
        }

        private T? DeserializeXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(xml);
            return (T?)serializer.Deserialize(reader);
        }

    }
}
