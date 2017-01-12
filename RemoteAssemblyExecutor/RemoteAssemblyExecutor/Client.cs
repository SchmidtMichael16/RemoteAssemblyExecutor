//-----------------------------------------------------------------------
// <copyright file="Client.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class Client.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Represent the class Client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The TCP client.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// The connection manager.
        /// </summary>
        private ConnectionManager connectionManager;

        /// <summary>
        /// The synchronization context of the user interface.
        /// </summary>
        private SynchronizationContext uiContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="tcpClient">The TCP client.</param>
        /// <param name="uiContext">The user interface context.</param>
        public Client(int id, TcpClient tcpClient, SynchronizationContext uiContext)
        {
            this.Id = id;
            this.tcpClient = tcpClient;
            this.uiContext = uiContext;
            this.connectionManager = new ConnectionManager(tcpClient.GetStream());
            this.connectionManager.OnNewLogEntry += this.ConnectionManager_OnNewLogEntry;
            this.connectionManager.OnPacketReceived += this.ConnectionManager_OnPacketReceived;
            this.LogList = new ObservableCollection<LogEntry>();
            this.AssemblyList = new ObservableCollection<AssemblyEntry>();
            this.ResultList = new ObservableCollection<ResultEntry>();
        }

        /// <summary>
        /// Gets fired if a packet was received.
        /// </summary>
        public event EventHandler<NetworkPacketEventArgs> OnPacketReceived;

        /// <summary>
        /// Gets fired if a new log entry is to write.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnNewLogEntry;

        /// <summary>
        /// Gets or sets the id of the client.
        /// </summary>
        /// <value>The id of the client.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the log list.
        /// </summary>
        /// <value>The log list of the client.</value>
        public ObservableCollection<LogEntry> LogList { get; set; }

        /// <summary>
        /// Gets the connection manager of the client.
        /// </summary>
        /// <value>The connection manager of the client.</value>
        public ConnectionManager ConnectionManager
        {
            get
            {
                return this.connectionManager;
            }
        }

        /// <summary>
        ///  Gets or sets the assembly list.
        /// </summary>
        /// <value>The assembly list of the client.</value>
        public ObservableCollection<AssemblyEntry> AssemblyList
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the result list.
        /// </summary>
        /// <value>The assembly list of the client.</value>
        public ObservableCollection<ResultEntry> ResultList
        {
            get;
            set;
        }

        /// <summary>
        /// Upload an assembly on the server.
        /// </summary>
        /// <param name="path">The path of the assembly file.</param>
        public void UploadAssemblie(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found!", path);
            }
            else
            {
                byte[] buffer = File.ReadAllBytes(path);

                this.connectionManager.SendAssemblie(buffer, Path.GetFileName(path), "Server");
            }
        }

        /// <summary>
        /// Gets fired if a new packet received.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void FireOnPacketReceived(NetworkPacketEventArgs e)
        {
            if (this.OnPacketReceived != null)
            {
                this.OnPacketReceived(this, e);
            }
        }

        /// <summary>
        /// Gets fired if a new log entry is to write.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void FireOnNewLogEntry(LogMessageEventArgs e)
        {
            if (this.OnNewLogEntry != null)
            {
                this.OnNewLogEntry(this, e);
            }
        }

        /// <summary>
        /// The callback method of the packet received event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ConnectionManager_OnPacketReceived(object sender, NetworkPacketEventArgs e)
        {
            e.Packet.ClientId = this.Id;
            this.uiContext.Send(x => this.LogList.Add(new LogEntry(e.Packet.ClientId, DateTime.Now, LogMessageType.Info, $"Packet received! {e.Packet.PacketType}")), null);

            if (e.Packet.PacketType == PacketType.AssemblieList)
            {
                this.uiContext.Send(x => this.AssemblyList.Clear(), null);

                for (int i = 0; i < e.Packet.AssemblyList.Count; i++)
                {
                    this.uiContext.Send(x => this.AssemblyList.Add(e.Packet.AssemblyList[i]), null);
                }
            }
            else if (e.Packet.PacketType == PacketType.LogEntry)
            {
                this.uiContext.Send(x => this.LogList.Add(e.Packet.LogEntry), null);
            }
            else if (e.Packet.PacketType == PacketType.ResultEntry)
            {
                this.uiContext.Send(x => this.ResultList.Add(e.Packet.ResultEntry), null);
            }
        }

        /// <summary>
        /// The callback method of the new log entry event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ConnectionManager_OnNewLogEntry(object sender, LogMessageEventArgs e)
        {
            e.LogEntry.ClientId = this.Id;
            this.uiContext.Send(x => this.LogList.Add(e.LogEntry), null);
        }
    }
}
