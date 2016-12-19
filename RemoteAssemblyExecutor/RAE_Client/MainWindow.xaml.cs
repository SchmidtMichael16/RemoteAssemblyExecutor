using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RemoteAssemblyExecutor;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace RAE_Client
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client client;
        private ObservableCollection<LogEntry> tmpLogList = new ObservableCollection<LogEntry>();
        private ListView listViewLog;

        public MainWindow()
        {
            InitializeComponent();
            this.client = null;

            listViewLog = this.ListViewLog.FindName("LogList") as ListView;
            if (listViewLog != null)
            {
                listViewLog.ItemsSource = tmpLogList;
            }
        }

        private void MenuItem_Connect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Connect();
            dialog.ShowDialog();
            TcpClient tmpClient = new TcpClient();
            try
            {
                tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Try to connect to server! IpAdress:{dialog.IPAddress.ToString()} Port:{dialog.Port}"));
                tmpClient.Connect(dialog.IPAddress, dialog.Port);
                tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Connected to server!"));
                this.client = new Client(0, tmpClient);
                this.client.ConnectionManager.StartListening();
                this.client.ConnectionManager.SendInfoMessage("Hello :-)", "Server");
                foreach (LogEntry entry in this.tmpLogList)
                {
                    this.client.LogList.Add(entry);
                }

                if (listViewLog != null)
                {
                    listViewLog.ItemsSource = this.client.LogList;
                }
            }
            catch (Exception ex)
            {
                tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during connecting to server! IpAdress:{dialog.IPAddress} Port:{dialog.Port}"));
                tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, ex.Message));
                MessageBox.Show("Cannot connect to the server! \n Look at the 'Log-Tab' for more information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Upload_Assemblie_Click(object sender, RoutedEventArgs e)
        {
            if (this.client != null)
            {
                string path = string.Empty;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Assemblie files(*.dll)|*.dll";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.FileName;
                }
            }
            else
            {
                MessageBox.Show("First connect to a server!");
            }
        }
    }
}
