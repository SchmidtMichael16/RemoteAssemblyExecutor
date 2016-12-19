
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace RemoteAssemblyExecutor
{
    public class Client
    {


        private TcpClient tcpClient;

        private ConnectionManager connectionManager;

        public Client(int id, TcpClient tcpClient)
        {
            this.Id = id;
            this.tcpClient = tcpClient;
            this.connectionManager = new ConnectionManager(tcpClient.GetStream());
            this.LogList = new ObservableCollection<LogEntry>();
        }

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

    }
}
