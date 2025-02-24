using EasySaveLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveLibrary.ViewModels
{
    public class LogEntryViewModel
    {
        private static LogEntryViewModel? _instance;
        private LogService logService;


        private LogEntryViewModel()
        {
            logService = LogService.GetLogServiceInstance();
        }

        public static LogEntryViewModel GetLogEntryViewModelInstance()
        {
            _instance ??= new LogEntryViewModel();
            return _instance;
        }

        public void WriteLog(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime)
        {
            //LogService logService = LogService.GetLogServiceInstance();
            logService.SaveLog(name, fileSource, fileTarget, fileSize, fileTransferTime);
        }
        public void SetLogFormat(string format)
        {
            logService.SetLogFormat(format);
        }

    }
}
