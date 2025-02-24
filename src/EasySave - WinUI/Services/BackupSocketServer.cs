using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace EasySave___WinUI.Services {
    public class BackupSocketServer {
        private TcpListener? _server;
        private bool _isRunning = false;
        private BackupService? _backupService;

        public BackupSocketServer() {}

        public void SetBackupService(BackupService backupService) {
            _backupService = backupService;
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
            } catch {
            }
        }

        private async Task HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (_backupService != null) {
                if (command == "PAUSE") _backupService.PauseBackup();
                if (command == "RESUME") _backupService.ResumeBackup();
                if (command == "STOP") _backupService.StopBackup();
            }

            string response = "CONNECTED"; // Répond toujours "CONNECTED" pour vérifier la connexion
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
