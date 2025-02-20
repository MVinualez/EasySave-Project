using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EasySave___WinUI.Models
{
    internal class ProcessChecker
    {
        private static readonly string[] OfficeProcesses = { "WINWORD", "EXCEL", "POWERPNT", "OUTLOOK" };

        public bool IsOfficeAppRunning()
        {
            return Process.GetProcesses()
                .Any(p => OfficeProcesses.Contains(p.ProcessName.ToUpper()));
        }
    }
}
