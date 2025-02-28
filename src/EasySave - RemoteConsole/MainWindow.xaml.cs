using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace RemoteConsole {
    public partial class MainWindow : Window {
        private readonly BackupSocketClient _socketClient;

        public MainWindow() {
            InitializeComponent();
            _socketClient = new BackupSocketClient("127.0.0.1");
            CheckConnection();
        }

        private async void CheckConnection() {
            bool isConnected = await TestServerConnection();
            UpdateStatus(isConnected);
        }

        private async Task<bool> TestServerConnection() {
            try {
                using TcpClient client = new TcpClient();
                var connectTask = client.ConnectAsync("127.0.0.1", 5000);
                if (await Task.WhenAny(connectTask, Task.Delay(2000)) != connectTask) {
                    return false;
                }
                return true;
            } catch {
                return false;
            }
        }

        private void UpdateStatus(bool isConnected) {
            if (isConnected) {
                StatusTextBlock.Text = "🟢 Connecté au serveur";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            } else {
                StatusTextBlock.Text = "❌ Déconnecté - Vérifiez le serveur";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private async void Reconnect_Click(object sender, RoutedEventArgs e) {
            StatusTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
            StatusTextBlock.Text = "🔄 Tentative de reconnexion...";
            bool isConnected = await TestServerConnection();
            UpdateStatus(isConnected);
        }

        private async void PauseBackup_Click(object sender, RoutedEventArgs e) {
            string jobName = JobNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(jobName)) {
                await _socketClient.SendCommand("PAUSE", jobName);
            }
        }

        private async void ResumeBackup_Click(object sender, RoutedEventArgs e) {
            string jobName = JobNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(jobName)) {
                await _socketClient.SendCommand("RESUME", jobName);
            }
        }

        private async void StopBackup_Click(object sender, RoutedEventArgs e) {
            string jobName = JobNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(jobName)) {
                await _socketClient.SendCommand("STOP", jobName);
            }
        }
    }
}
