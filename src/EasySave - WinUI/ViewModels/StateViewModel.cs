using EasySave___WinUI.Services;
using Microsoft.UI.Xaml;

namespace EasySave___WinUI.ViewModels {
    internal class StateViewModel {
        private static StateViewModel? _instance;
        private StateService _stateService;

        private StateViewModel(XamlRoot xamlRoot) {
            _stateService = StateService.GetStateServiceInstance(xamlRoot); 
        }

        public static StateViewModel GetStateViewModelInstance(XamlRoot xamlRoot) {
            _instance ??= new StateViewModel(xamlRoot);
            return _instance;
        }

        /// <summary>
        /// Initializes and registers a new backup job in the real-time state tracking.
        /// </summary>
        /// <param name="jobName"></param>
        public void RegisterJobState(string jobName) {
            _stateService.StartJob(jobName);
        }

        /// <summary>
        /// Registers a file in the real-time state tracking before processing.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="fileSize"></param>
        public void TrackFileInState(string jobName, string sourceFilePath, string targetFilePath, long fileSize) {
            _stateService.AddFileToState(jobName, sourceFilePath, targetFilePath, fileSize);
        }

        /// <summary>
        /// Updates the state to confirm that a file has been successfully processed.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="fileSize"></param>
        public void MarkFileAsProcessed(string jobName, string sourceFilePath, long fileSize) {
            _stateService.UpdateFileTransfer(jobName, sourceFilePath, fileSize);
        }

        /// <summary>
        /// Marks a backup job as completed if all files have been processed.
        /// </summary>
        /// <param name="jobName"></param>
        public void CompleteJobState(string jobName) {
            _stateService.CompleteJob(jobName);
        }
    }
}
