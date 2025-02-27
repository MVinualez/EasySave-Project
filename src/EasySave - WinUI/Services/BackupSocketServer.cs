using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Services {
    public class BackupSocketServer {
        private TcpListener? _server;
        private bool _isRunning = false;
        private readonly Dictionary<string, BackupService> _backupServices;

        public BackupSocketServer() {
            _backupServices = new Dictionary<string, BackupService>();
        }

        public void RegisterBackupService(string jobName, BackupService backupService) {
            if (!_backupServices.ContainsKey(jobName)) {
                _backupServices[jobName] = backupService;
            }
        }

        public async Task StartServer(int port = 5000) {
            try {
                _server = new TcpListener(IPAddress.Any, port);
                _server.Start();
                _isRunning = true;

                while (_isRunning) {
                    TcpClient client = await _server.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
            } catch (Exception ex) {
                Console.WriteLine($"Erreur serveur : {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            string response = "INVALID_COMMAND"

            if (command.StartsWith("PAUSE ")) {
                string jobName = command.Substring(6);
                if (_backupServices.TryGetValue(jobName, out var backupService)) {
                    backupService.PauseBackup();
                    response = $"PAUSED {jobName}";
                } else {
                    response = $"JOB_NOT_FOUND {jobName}";
                }
            } else if (command.StartsWith("RESUME ")) {
                string jobName = command.Substring(7);
                if (_backupServices.TryGetValue(jobName, out var backupService)) {
                    backupService.ResumeBackup();
                    response = $"RESUMED {jobName}";
                } else {
                    response = $"JOB_NOT_FOUND {jobName}";
                }
            } else if (command.StartsWith("STOP ")) {
                string jobName = command.Substring(5);
                if (_backupServices.TryGetValue(jobName, out var backupService)) {
                    backupService.StopBackup();
                    response = $"STOPPED {jobName}";
                } else {
                    response = $"JOB_NOT_FOUND {jobName}";
                }
            }

            byte[] responseData = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseData, 0, responseData.Length);
            client.Close();
        }

        public void StopServer() {
            _isRunning = false;
            _server?.Stop();
        }
    }
}
