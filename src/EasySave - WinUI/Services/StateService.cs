using EasySave___WinUI.Models;
using EasySave___WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using System.Reflection;
using Windows.ApplicationModel.Resources;

namespace EasySave___WinUI.Services {
    internal class StateService {
        private static StateService? _instance;
        private string _stateFilePath = string.Empty;
        private readonly XamlRoot _xamlRoot;
        private readonly NotificationViewModel _notificationViewModel = NotificationViewModel.GetNotificationViewModelInstance();
        private readonly ResourceLoader _resourceLoader;

        private StateService(XamlRoot xamlRoot) {
            _resourceLoader = new ResourceLoader();

            _xamlRoot = xamlRoot;
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            path = path != null && path.Length >= 1 ? path : Directory.GetCurrentDirectory();

            string dirName = "state";
            string fileName = "state.json";
            string fullPath = Path.Combine(path, dirName, fileName);
            _stateFilePath = fullPath;

            if (!Directory.Exists(Path.Combine(path, dirName))) {
                Directory.CreateDirectory(Path.Combine(path, dirName));
            }
            if (!File.Exists(fullPath)) {
                File.WriteAllText(fullPath, "{}");
            }
        }

        public static StateService GetStateServiceInstance(XamlRoot xamlRoot) {
            _instance ??= new StateService(xamlRoot);
            return _instance;
        }

        private Dictionary<string, StateModel> LoadState() {
            if (File.Exists(_stateFilePath)) {
                string json = File.ReadAllText(_stateFilePath);
                if (!string.IsNullOrWhiteSpace(json)) {
                    return JsonConvert.DeserializeObject<Dictionary<string, StateModel>>(json) ?? new();
                }
            }
            return new();
        }

        private void SaveState(Dictionary<string, StateModel> states) {
            string json = JsonConvert.SerializeObject(states, Formatting.Indented);
            File.WriteAllText(_stateFilePath, json);
        }

        public void StartJob(string jobName) {
            Dictionary<string, StateModel> states = LoadState();

            if (!states.ContainsKey(jobName)) {
                states[jobName] = new StateModel(
                    jobName: jobName,
                    timestamp: (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    state: "InProgress",
                    numberOfFilesRemaining: 0,
                    totalFilesSize: 0,
                    progress: new StateProgressModel(0, 0, "", "")
                );
                SaveState(states);
            } else {
                //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_TaskAlreadyExists"), string.Format(_resourceLoader.GetString("State_TaskAlreadyExistsContent"), jobName), string.Empty, "OK", _xamlRoot);
            }
        }

        public void AddFileToState(string jobName, string sourceFilePath, string targetFilePath, long fileSize) {
            Dictionary<string, StateModel> states = LoadState();

            //if (!states.ContainsKey(jobName)) {
            //    _notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_JobDoesntExists"), string.Format(_resourceLoader.GetString("State_JobDoesntExistsContent"), jobName), string.Empty, "OK", _xamlRoot);
            //    return;
            //}

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
            //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_FileAdded"), string.Format(_resourceLoader.GetString("State_FileAdded"), sourceFilePath, jobName), string.Empty, "OK", _xamlRoot);
        }

        public void UpdateFileTransfer(string jobName, string sourceFilePath, long fileSize) {
            Dictionary<string, StateModel> states = LoadState();

            //if (!states.ContainsKey(jobName)) {
            //    _notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_JobDoesntExists"), string.Format(_resourceLoader.GetString("State_JobDoesntExistsContent"), jobName), string.Empty, "OK", _xamlRoot);
            //    return;
            //}

            StateModel state = states[jobName];

            if (state.NumberOfFilesRemaining > 0) {
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
                //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_FileTransferInProgress"), string.Format(_resourceLoader.GetString("State_FileTransferInProgress"), sourceFilePath, fileSize), string.Empty, "OK", _xamlRoot);
            } else {
                //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_NoPendingFiles"), string.Format(_resourceLoader.GetString("State_NoPendingFiles"), jobName), string.Empty, "OK", _xamlRoot);
            }
        }

        public void CompleteJob(string jobName) {
            Dictionary<string, StateModel> states = LoadState();

            //if (!states.ContainsKey(jobName)) {
            //    _notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_JobDoesntExists"), string.Format(_resourceLoader.GetString("State_JobDoesntExistsContent"), jobName), string.Empty, "OK", _xamlRoot);
            //    return;
            //}

            StateModel state = states[jobName];

            if (state.NumberOfFilesRemaining == 0) {
                state.State = "Completed";
                states[jobName] = state;
                SaveState(states);
                //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_JobCompleted"), string.Format(_resourceLoader.GetString("State_JobCompleted"), jobName), string.Empty, "OK", _xamlRoot);
            } else {
                //_notificationViewModel?.ShowPopupDialog(_resourceLoader.GetString("State_JobNotFinished"), string.Format(_resourceLoader.GetString("State_JobNotFinished"), jobName, state.NumberOfFilesRemaining), string.Empty, "OK", _xamlRoot);
            }
        }
    }
}
