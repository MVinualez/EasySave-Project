using System.Diagnostics;


namespace EasySave___WinUI.Models {
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
