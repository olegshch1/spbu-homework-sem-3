using FTPClient;
using GUI_FTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace GUIForFTP
{
    /// <summary>
    /// View model class
    /// </summary>
    public class AppViewModel : INotifyPropertyChanged
    {
        private Client client;
        private string hostname;
        private int port;
        private string serverPath;
        private string clientPath;
        private readonly string serverRoot;
        private List<(string, bool)> serverFolderList = new List<(string, bool)>();
        private List<(string, bool)> clientFolderList = new List<(string, bool)>();
        private string selectedItem;

        /// <summary>
        /// Shows if component property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Files and folders in the current folder on server
        /// </summary>
        public ObservableCollection<string> DisplayedServerFolderList { get; private set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Files and folders in the current folder on client
        /// </summary>
        public ObservableCollection<string> DisplayedClientFolderList { get; private set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Downloading-state files
        /// </summary>
        public ObservableCollection<string> InProcessList { get; private set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Downloaded files
        /// </summary>
        public ObservableCollection<string> DownloadedList { get; private set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Selected item in server folder list
        /// </summary>
        public string SelectedItem
        {
            get
                => selectedItem;
            set
            {
                if (value != null)
                {
                    if (IsDirectory(value))
                    {
                        selectedItem = null;
                        return;
                    }
                }
                selectedItem = value;
            }
        }

        /// <summary>
        /// Path to current folder on server
        /// </summary>
        public string ServerPath
        {
            get
                => serverPath.Substring(8);
            set
            {
                serverPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Path to current folder on client
        /// </summary>
        public string ClientPath
        {
            get
                => clientPath;
            set
            {
                clientPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Server hostname
        /// </summary>
        public string Hostname
        {
            get
                => hostname;
            set
            {
                if (IsConnected)
                {
                    return;
                }

                hostname = value;
            }
        }

        /// <summary>
        /// Port for connection
        /// </summary>
        public string Port
        {
            get
            {
                if (port != -1)
                {
                    return port.ToString();
                }
                return "";
            }
            set
            {
                if (IsConnected)
                {
                    return;
                }

                if (!int.TryParse(value, out port))
                {
                    port = -1;
                }
            }
        }

        /// <summary>
        /// Shows if the client is connected
        /// </summary>
        public bool IsConnected { get; private set; }

        public AppViewModel(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
            IsConnected = false;
            serverRoot = "../../..";
            serverPath = serverRoot;
            clientPath = "Choose folder";
        }

        /// <summary>
        /// Command for connecting
        /// </summary>
        public ICommand ConnectCommand
        {
            get
                => new Command(async obj =>
                {
                    await Task.Run(() => Connect());
                    if (IsConnected)
                    {
                        await UpdateServerList(serverRoot);
                    }
                }, obj => hostname != "" && port != -1);
        }

        /// <summary>
        /// Command for showing the previous folder on server
        /// </summary>
        public ICommand GoServerFolderUpCommand
        {
            get
                => new Command(async obj => await GoServerFolderUp(), obj => serverPath != serverRoot);
        }

        /// <summary>
        /// Command for showing the previous folder on client
        /// </summary>
        public ICommand GoClientFolderUpCommand
        {
            get
                => new Command(obj => GoClientFolderUp(), obj => clientPath != "Choose folder" && clientPath.Length > 3);
        }

        /// <summary>
        /// Command for showing a hint for user to understand the interface of the application
        /// </summary>
        public ICommand HelpCommand
        {
            get
                => new Command(obj =>
                {
                    System.Windows.MessageBox.Show("Enter server's hostname and port and click 'Connect' button to connect to the server.\n" +
                        "Click 'Choose folder' button to choose a client's file system folder for downloading files.\n" +
                        "Double click on folder to open it; click on '..' button to go back to the previous folder.\n" +
                        "Choose file in the current server folder and click 'Download' button or double click on the file to download it into the client's downloads folder.\n" +
                        "Click 'Download all' button to download all files from the current server folder into the client's downloads folder.\n",
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                });
        }

        /// <summary>
        /// Command for letting user choose folder 
        /// </summary>
        public ICommand ChooseClientFolderCommand
        {
            get
                => new Command(obj =>
                {
                    var folderBrowser = new FolderBrowserDialog();
                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        ClientPath = folderBrowser.SelectedPath.Replace('\\', '/');
                        ChooseClientFolder(clientPath);
                    }
                });
        }

        /// <summary>
        /// Command for downloading 
        /// </summary>
        public ICommand DownloadCommand
        {
            get
                => new Command(async obj => await DownloadFile(selectedItem), obj => serverFolderList.Count != 0 && selectedItem != null);
        }

        /// <summary>
        /// Command for downloading all files 
        /// </summary>
        public ICommand DownloadAllCommand
        {
            get
                => new Command(async obj =>
                {
                    if (clientPath == "Choose folder")
                    {
                        HandleError("Choose downloads folder to download files into.");
                        return;
                    }

                    foreach (var item in serverFolderList)
                    {
                        if (!item.Item2)
                        {
                            var name = item.Item1.Substring(item.Item1.LastIndexOf('\\') + 1);
                            await DownloadFile(name);
                        }
                    }
                });
        }

        /// <summary>
        /// Notifies that the component property is changed
        /// </summary>
        public void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>
        /// Connecting
        /// </summary>
        public void Connect()
        {
            try
            {
                client = new Client(hostname, port);
                client.Connect();
                IsConnected = true;
            }
            catch (Exception exception)
            {
                HandleError(exception.Message);
                IsConnected = false;
            }
        }

        /// <summary>
        /// Shows the previous folder on server
        /// </summary>
        public async Task GoServerFolderUp() => await UpdateServerList(serverPath.Substring(0, serverPath.LastIndexOf('/')));

        /// <summary>
        /// Shows the previous folder on client
        /// </summary>
        public void GoClientFolderUp()
        {
            var newPath = clientPath.Substring(0, clientPath.LastIndexOf('/'));

            if (newPath.Length < 3 && !newPath.EndsWith("/"))
            {
                newPath += "/";
            }

            UpdateClientList(newPath);
        }

        /// <summary>
        /// Chooses folder in client's file system to download files
        /// </summary>
        public void ChooseClientFolder(string path) => UpdateClientList(path);

        /// <summary>
        /// Opens and shows the contents 
        /// </summary>
        /// <param name="name">Name of the folder to open.</param>
        public void OpenClientFolder(string name)
        {
            foreach (var item in clientFolderList)
            {
                if (item.Item1.EndsWith(name))
                {
                    if (item.Item2)
                    {
                        UpdateClientList(clientPath + $"/{name}");
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Opens the folder or downloads file on server
        /// </summary>
        /// <param name="name">Name of the item.</param>
        public async Task OpenOrDownloadServerItem(string name)
        {
            if (IsDirectory(name))
            {
                await UpdateServerList(serverPath + $"/{name}");
            }
            else
            {
                await DownloadFile(name);
            }
        }

        private async Task DownloadFile(string name)
        {
            try
            {
                if (clientPath == "Choose folder")
                {
                    HandleError("Choose downloads folder to download files into.");
                    return;
                }

                var path = serverPath + "/" + name;

                InProcessList.Add(name);

                await client.Get(path, clientPath);

                InProcessList.Remove(name);
                DownloadedList.Add(name);
                UpdateClientList(clientPath);
            }
            catch (Exception exception)
            {
                InProcessList.Remove(name);
                HandleError(exception.Message);
            }
        }

        private void UpdateClientList(string path)
        {
            clientFolderList.Clear();

            try
            {
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                foreach (var directory in directories)
                {
                    clientFolderList.Add((directory.Substring(directory.LastIndexOf('/') + 1), true));
                }

                foreach (var file in files)
                {
                    clientFolderList.Add((file.Substring(file.LastIndexOf('/') + 1), false));
                }

                UpdateDisplayedClientFolderList();
                ClientPath = path;
            }
            catch (Exception exception)
            {
                HandleError(exception.Message);
            }
        }

        private void UpdateDisplayedClientFolderList()
        {
            DisplayedClientFolderList.Clear();

            foreach (var item in clientFolderList)
            {
                var name = item.Item1;
                DisplayedClientFolderList.Add(name.Substring(name.LastIndexOf('\\') + 1));
            }
        }

        private async Task UpdateServerList(string path)
        {
            serverFolderList.Clear();
            serverFolderList = await client.List(path);
            UpdateDisplayedServerFolderList();
            ServerPath = path;
        }

        private void UpdateDisplayedServerFolderList()
        {
            DisplayedServerFolderList.Clear();
            foreach (var item in serverFolderList)
            {
                var name = item.Item1;
                DisplayedServerFolderList.Add(name.Substring(name.LastIndexOf('\\') + 1));
            }
        }

        private bool IsDirectory(string name)
        {
            foreach (var item in serverFolderList)
            {
                if (item.Item1.EndsWith(name))
                {
                    return item.Item2;
                }
            }
            return false;
        }

        private void HandleError(string message)
        {
            System.Windows.MessageBox.Show("An error just occurred: " + message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
