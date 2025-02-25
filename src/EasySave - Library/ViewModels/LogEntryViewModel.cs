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
        private string _logFormat;


        private LogEntryViewModel()
        {
            logService = LogService.GetLogServiceInstance();
        }

        public static LogEntryViewModel GetLogEntryViewModelInstance()
        {
            _instance ??= new LogEntryViewModel();
            return _instance;
        }

        public void WriteLog(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime, double encryptionTime)
        {
            LogService logService = LogService.GetLogServiceInstance();
            logService.SaveLog(name, fileSource, fileTarget, fileSize, fileTransferTime, encryptionTime);
        }
        public string GetLogFormat()
        {
            return _logFormat;
        }

        public void SetLogFormat(string format)
        {
            if (format == "JSON" || format == "XML")    
            {
                _logFormat = format;
            }

            logService.LogFormat = _logFormat;
        }

    }
}
