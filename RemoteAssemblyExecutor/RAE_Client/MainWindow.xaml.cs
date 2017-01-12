//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class MainWindow.</summary>
//-----------------------------------------------------------------------

namespace RAE_Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;   
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using RemoteAssemblyExecutor;

    /// <summary>
    /// Represent the class MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The client of program.
        /// </summary>
        private Client client;

        /// <summary>
        /// The temporary log list.
        /// </summary>
        private ObservableCollection<LogEntry> tmpLogList = new ObservableCollection<LogEntry>();

        /// <summary>
        /// The list view log.
        /// </summary>
        private ListView listViewLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.client = null;

            this.listViewLog = this.ListViewLog.FindName("LogList") as ListView;
            if (this.listViewLog != null)
            {
                this.listViewLog.ItemsSource = this.tmpLogList;
            }
        }

        /// <summary>
        /// The callback method for connect click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void MenuItem_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (this.client != null)
            {
                MessageBox.Show("Client is already connected to a server!");
            }
            else
            {
                var dialog = new Connect();
                dialog.ShowDialog();
                TcpClient tmpClient = new TcpClient();
                try
                {
                    this.tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Try to connect to server! IpAdress:{dialog.IPAddress.ToString()} Port:{dialog.Port}"));
                    tmpClient.Connect(dialog.IPAddress, dialog.Port);
                    this.tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Connected to server!"));
                    this.client = new Client(0, tmpClient, SynchronizationContext.Current);
                    this.client.ConnectionManager.StartListening();
                    this.client.LogList = new ObservableCollection<LogEntry>(this.tmpLogList);
                    this.OverviewList.ItemsSource = this.client.AssemblyList;
                    this.ResultList.ItemsSource = this.client.ResultList;

                    foreach (LogEntry entry in this.tmpLogList)
                    {
                        this.client.LogList.Add(entry);
                    }

                    if (this.listViewLog != null)
                    {
                        this.listViewLog.ItemsSource = this.client.LogList;
                    }
                }
                catch (Exception ex)
                {
                    this.tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during connecting to server! IpAdress:{dialog.IPAddress} Port:{dialog.Port}"));
                    this.tmpLogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, ex.Message));
                    MessageBox.Show("Cannot connect to the server! \n Look at the 'Log-Tab' for more information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// The callback method for connect click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void MenuItem_Upload_Assemblie_Click(object sender, RoutedEventArgs e)
        {
            if (this.client != null)
            {
                string path = string.Empty;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "DLL files(*.dll)|*.dll|EXE files (*.exe)|*.exe";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.FileName;

                    try
                    {
                        this.client.UploadAssemblie(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception occured during uploading file! \n" + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("First connect to a server!");
            }
        }

        /// <summary>
        /// The callback method for run.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            int runIndex = this.GetSelectedAssemblieIndex(sender);

            if (runIndex >= 0)
            {
                if (this.client.AssemblyList[runIndex].AllDependenciesAvailable)
                {
                    var memberWindow = new MemberWindow(this.client.AssemblyList[runIndex].Members);
                    ListView listViewMember = memberWindow.FindName("MemberList") as ListView;

                    if (listViewMember != null)
                    {
                        if (!this.client.AssemblyList[runIndex].AllDependenciesAvailable)
                        {
                            MessageBox.Show("Not all referneced assemblies available!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            memberWindow.ShowDialog();
                            if (memberWindow.Start)
                            {
                                AssemblyMember tmp = this.client.AssemblyList[runIndex].Members[memberWindow.MemberIndex];
                                this.client.ConnectionManager.SendStartMethod(this.client.AssemblyList[runIndex].Members[memberWindow.MemberIndex], this.client.AssemblyList[runIndex].Id, "Server");
                            }

                            for (int i = 0; i < this.client.AssemblyList[runIndex].Members.Count; i++)
                            {
                                for (int j = 0; j < this.client.AssemblyList[runIndex].Members[i].ParamList.Count; j++)
                                {
                                    this.client.AssemblyList[runIndex].Members[i].ParamList[j].Value = string.Empty;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not all referenced assemblies available!");
                }
            }
        }

        /// <summary>
        /// The callback method for check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Check_Click(object sender, RoutedEventArgs e)
        {
            var checkWindow = new CheckWindow();
            ListView listViewDependancies = checkWindow.FindName("DpendencyList") as ListView;

            if (listViewDependancies != null)
            {
                Button button = (Button)sender;
                AssemblyEntry selectedItem = (AssemblyEntry)button.DataContext;

                listViewDependancies.ItemsSource = this.client.AssemblyList[this.GetSelectedAssemblieIndex(sender)].Dependencies;
                checkWindow.ShowDialog();
            }
        }

        /// <summary>
        /// The callback method for delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int deleteIndex = this.GetSelectedAssemblieIndex(sender);

            if (deleteIndex >= 0)
            {
                if (MessageBox.Show("Do you really want to delete this Assembly?", "Delete Assembly", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.client.ConnectionManager.SendDeleteAssembly(this.client.AssemblyList[deleteIndex].Name, "Server");
                }
            }
        }

        /// <summary>
        /// Gets the selected assembly of assembly list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns>The selected index.</returns>
        private int GetSelectedAssemblieIndex(object sender)
        {
            Button button = (Button)sender;
            AssemblyEntry selectedItem = (AssemblyEntry)button.DataContext;

            for (int i = 0; i < this.client.AssemblyList.Count; i++)
            {
                if (selectedItem.Name == this.client.AssemblyList[i].Name)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
