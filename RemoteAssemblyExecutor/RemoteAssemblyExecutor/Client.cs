
using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading;

namespace RemoteAssemblyExecutor
{
    public class Client
    {
        private TcpClient tcpClient;

        private ConnectionManager connectionManager;

        /// <summary>
        /// The synchronization context of the user interface.
        /// </summary>
        private SynchronizationContext uiContext;

        public Client(int id, TcpClient tcpClient,SynchronizationContext uiContext )
        {
            this.Id = id;
            this.tcpClient = tcpClient;
            this.uiContext = uiContext;
            this.connectionManager = new ConnectionManager(tcpClient.GetStream());
            this.connectionManager.OnNewLogEntry += ConnectionManager_OnNewLogEntry;
            this.connectionManager.OnPacketReceived += ConnectionManager_OnPacketReceived;
            this.LogList = new ObservableCollection<LogEntry>();
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
        /// <value>The log list of the controller. </value>
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
        }

        /// <summary>
        /// The callback method of the new log entry event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ConnectionManager_OnNewLogEntry(object sender, LogMessageEventArgs e)
        {
            e.LogEntry.ClientId = this.Id;
            this.uiContext.Send(x => this.LogList.Add(e.LogEntry),null);
        }
    }
}
