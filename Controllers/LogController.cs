using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using easysave_project.Models;

namespace easysave_project.Controllers
{
    public class LogController
    {
        private readonly string logDirectory = "Logs"; // emplacement des logs, en fonction de ou est éxecuté le .exe

        public LogController()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }
        // Méthode LogController de la classe LogController qui utilise la méthode Exists de la classe System.IO.Directory et prends en paramètre la variable logDirectory
        // Permet de vérifier si le dossier Logs existe et sinon le créer.
        public void SaveLog(LogEntry log) // Création de la méthode Savelog qui appelle la méthode LogEntry de /Models/logEntry 
        {
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.json"; 
            string logFilePath = Path.Combine(logDirectory, logFileName);

            List<LogEntry> logEntries = new List<LogEntry>(); 

            // Charger l'existant s'il y a déjà un log
            if (File.Exists(logFilePath))
            {
                string existingJson = File.ReadAllText(logFilePath);
                logEntries = JsonConvert.DeserializeObject<List<LogEntry>>(existingJson) ?? new List<LogEntry>();
            }

            // Ajouter la nouvelle entrée et sauvegarder
            logEntries.Add(log);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logEntries, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
