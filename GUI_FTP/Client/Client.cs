using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace FTPClient
{
    /// <summary>
    /// Client class for FTP
    /// </summary>
    public class Client : IDisposable
    {
        private static TcpClient client;

        private readonly string hostname;

        private readonly int port;

        private StreamWriter writer;

        private StreamReader reader;

        /// <summary>
        /// Connection status
        /// </summary>
        public bool IsConnected { get; private set; }

        public Client(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        /// <summary>
        /// Connecting
        /// </summary>
        public void Connect()
        {
            try
            {
                client = new TcpClient(hostname, port);
                var stream = client.GetStream();
                writer = new StreamWriter(stream) { AutoFlush = true };
                reader = new StreamReader(stream);
                IsConnected = true;
            }
            catch (SocketException exception)
            {
                IsConnected = false;
                throw exception;
            }
        }

        /// <summary>
        /// Sends a request to list all files and directories
        /// </summary>
        /// <param name="path">Path to the directory on the server </param>
        /// <returns>info about every element on that path</returns>
        public async Task<List<(string, bool)>> List(string path)
        {
            var response = await MakeRequest(1, path);

            if (response == "-1")
            {
                throw new DirectoryNotFoundException();
            }
            else
            {
                return HandleListResponse(response);
            }
        }

        private List<(string, bool)> HandleListResponse(string response)
        {
            var splitResponse = response.Split(' ');

            var length = int.Parse(splitResponse[0]);

            var result = new List<(string, bool)>();
            for (var i = 1; i < length * 2; i += 2)
            {
                var name = splitResponse[i];
                var isDirectory = bool.Parse(splitResponse[i + 1]);
                result.Add((name, isDirectory));
            }

            return result;
        }

        /// <summary>
        /// Copying file from server to path 
        /// </summary>
        /// <param name="filePath">getting file from</param>
        /// <param name="destination">getting file to</param>
        public async Task Get(string filePath, string destination)
        {
            var response = await MakeRequest(2, filePath);
            var splitResponse = response.Split(' ');
            var content = response.Replace($"{splitResponse[0]} ", "");

            if (response == "-1")
            {
                throw new FileNotFoundException();
            }
            else
            {
                var fileName = filePath.Substring(filePath.LastIndexOf('/') + 1);
                using (var fileWriter = new StreamWriter(new FileStream(destination + "/" + fileName, FileMode.Create)))
                {
                    await fileWriter.WriteAsync(content);
                }
            }
        }

        private async Task<string> MakeRequest(int command, string path)
        {
            if (IsConnected)
            {
                await writer.WriteLineAsync(command + " " + path);
                return await reader.ReadLineAsync();
            }
            else
            {
                throw new InvalidOperationException("No connection with server");
            }
        }

        /// <summary>
        /// Stops the client.
        /// </summary>
        public void Stop()
        {
            writer?.Close();
            reader?.Close();
            client?.Close();
        }

        /// <summary>
        /// Disposes all
        /// </summary>
        public void Dispose()
        {
            writer?.Dispose();
            reader?.Dispose();
            client?.Dispose();
        }
    }
}
