using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteConsole {
    public class BackupSocketClient {
        private readonly string _serverIp;
        private readonly int _port;

        public BackupSocketClient(string serverIp, int port = 5000) {
            _serverIp = serverIp;
            _port = port;
        }

        public async Task SendCommand(string command, string jobName) {
            try {
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(_serverIp, _port);
                NetworkStream stream = client.GetStream();
                string fullCommand = $"{command} {jobName}";
                byte[] data = Encoding.UTF8.GetBytes(fullCommand);
                await stream.WriteAsync(data, 0, data.Length);

                byte[] responseBuffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                Console.WriteLine($"\ud83d\udce8 Réponse du serveur : {response}");
            } catch (Exception ex) {
                Console.WriteLine($"\u274c Erreur client : {ex.Message}");
            }
        }
    }
}