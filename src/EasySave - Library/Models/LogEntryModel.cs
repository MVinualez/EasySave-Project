﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveLibrary.Models
{
    public class LogEntryModel
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }

        public double EncryptionTime { get; set; }
        public LogEntryModel(string name, string fileSource, string fileTarget, long fileSize, double fileTransferTime, double encryptionTime)
        {
            Name = name;
            FileSource = fileSource;
            FileTarget = fileTarget;
            FileSize = fileSize;
            FileTransferTime = fileTransferTime;
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            EncryptionTime = encryptionTime;
        }
    }
}