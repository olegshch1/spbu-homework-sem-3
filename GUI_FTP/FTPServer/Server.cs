using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{
    /// <summary>
    /// FTP server
    /// </summary>
    public class Server
    {
        private readonly TcpListener listener;

        private readonly CancellationTokenSource cancellationToken;

        public Server(int port)
        {
            cancellationToken = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Starting server
        /// </summary>
        public async Task Start()
        {
            try
            {
                listener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    ClientService(client);
                }
            }
            finally
            {
                Stop();
            }
        }

        private void ClientService(TcpClient client)
        {
            Task.Run(async () =>
            {
                using (var writer = new StreamWriter(client.GetStream()) { AutoFlush = true })
                using (var reader = new StreamReader(client.GetStream()))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var request = (await reader.ReadLineAsync()).Split(' ');

                        if (request[0] == "1")
                        {
                            await ListResponse(request[1], writer);
                        }
                        else if (request[0] == "2")
                        {
                            await GetResponse(request[1], writer);
                        }
                    }
                }
            });
        }

        private async Task ListResponse(string path, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);

            var response = $"{files.Length + directories.Length} ";

            foreach (var directory in directories)
            {
                var directoryName = directory.Replace(path, "");
                response += $".{directoryName} true ";
            }

            foreach (var file in files)
            {
                var fileName = file.Replace(path, "");
                response += $".{fileName} false ";
            }

            await writer.WriteLineAsync(response);
        }

        private async Task GetResponse(string path, StreamWriter writer)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var content = File.ReadAllText(path);
            var size = content.Length;
            await writer.WriteLineAsync($"{size} {content}");
        }

        /// <summary>
        /// Stopping server.
        /// </summary>
        public void Stop()
        {
            cancellationToken.Cancel();
            listener?.Stop();
        }
    }
}
