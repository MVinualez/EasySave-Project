using EasySaveLibrary.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveLibrary.ViewModel
{
    internal class LogEntryViewModel
    {
        private static LogEntryViewModel _logEntryViewModel;

        private LogEntryViewModel()
        {

        }

        public static LogEntryViewModel getInstanceLogEntryModel()
        {
            if (_logEntryViewModel == null)
            {
                _logEntryViewModel = new LogEntryViewModel();
            }
            return _logEntryViewModel;
        }

        public void WriteLog(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime)
        {
            LogService logService = LogService.GetInstanceLogController();
            logService.SaveLog(name, fileSource, fileTarget, fileSize, fileTransferTime);
        }

    }
}
