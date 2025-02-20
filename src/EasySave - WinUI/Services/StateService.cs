using easysave_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace easysave_project.Services
{
    internal class StateService(string stateFilePath)
    {
        private string _stateFilePath = stateFilePath;

        public void GetCurrentStateFile() {
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            string dirName = "state";
            string fileName = "state.json";
            string fullPath = Path.Combine(path, dirName, fileName);
            _stateFilePath = fullPath;

            if (!Directory.Exists(Path.Combine(path, dirName)))
            {
                Directory.CreateDirectory(Path.Combine(path, dirName));
            }
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, "{}");
            }
        }

        private Dictionary<string, StateModel> LoadState() {
            if (File.Exists(_stateFilePath))
            {
                string json = File.ReadAllText(_stateFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, StateModel>>(json) ?? new();
                }
            }
            return new();
        }

        private void SaveState(Dictionary<string, StateModel> states) {
            string json = JsonConvert.SerializeObject(states, Formatting.Indented);
            File.WriteAllText(_stateFilePath, json);
            Console.WriteLine($"État mis à jour : {_stateFilePath}");
        }

        public void StartJob(string jobName)
        {
            Dictionary<string, StateModel> states = LoadState();

            if (!states.ContainsKey(jobName))
            {
                states[jobName] = new StateModel(
                    jobName: jobName,
                    timestamp: (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    state: "InProgress",
                    numberOfFilesRemaining: 0,
                    totalFilesSize: 0,
                    progress: new StateProgressModel(0, 0, "", "")
                );
                SaveState(states);
                Console.WriteLine($"Job {jobName} démarré.");
            }
            else
            {
                Console.WriteLine($"Le job {jobName} existe déjà.");
            }
        }

        public void AddFileToState(string jobName, string sourceFilePath, string targetFilePath, int fileSize) {
            Dictionary<string, StateModel> states = LoadState();

            if (!states.ContainsKey(jobName))
            {
                Console.WriteLine($"Le job {jobName} n'existe pas. Utilisez StartJob() d'abord.");
                return;
            }

            StateModel state = states[jobName];

            state.NumberOfFilesRemaining++;
            state.TotalFilesSize += fileSize;
            state.Progress = new StateProgressModel(
                filesRemaining: state.NumberOfFilesRemaining,
                fileSizeRemaining: state.TotalFilesSize,
                sourceFilePathInProgress: sourceFilePath,
                targetFilePathInProgress: targetFilePath
            );

            states[jobName] = state;
            SaveState(states);
            Console.WriteLine($"Ajout du fichier {sourceFilePath} au job {jobName}.");
        }

        public void UpdateFileTransfer(string jobName, string sourceFilePath, int fileSize) {
            Dictionary<string, StateModel> states = LoadState();

            if (!states.ContainsKey(jobName))
            {
                Console.WriteLine($"Le job {jobName} n'existe pas.");
                return;
            }

            StateModel state = states[jobName];

            if (state.NumberOfFilesRemaining > 0)
            {
                state.NumberOfFilesRemaining--;
                state.Timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                state.TotalFilesSize -= fileSize;
                state.Progress = new StateProgressModel(
                    filesRemaining: state.NumberOfFilesRemaining,
                    fileSizeRemaining: state.TotalFilesSize,
                    sourceFilePathInProgress: sourceFilePath,
                    targetFilePathInProgress: state.Progress.TargetFilePathInProgress
                );

                states[jobName] = state;
                SaveState(states);
                Console.WriteLine($"Transfert en cours : {sourceFilePath} ({fileSize} octets)");
            }
            else
            {
                Console.WriteLine($"Aucun fichier en attente pour {jobName}.");
            }
        }

        public void CompleteJob(string jobName) {
            Dictionary<string, StateModel> states = LoadState();

            if (!states.ContainsKey(jobName))
            {
                Console.WriteLine($"Le job {jobName} n'existe pas.");
                return;
            }

            StateModel state = states[jobName];

            if (state.NumberOfFilesRemaining == 0)
            {
                state.State = "Completed";
                states[jobName] = state;
                SaveState(states);
                Console.WriteLine($"Job {jobName} terminé avec succès !");
            }
            else
            {
                Console.WriteLine($"Le job {jobName} n'est pas encore terminé. Fichiers restants : {state.NumberOfFilesRemaining}");
            }
        }
    }
}
