
namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Server
    {
        private int port;

        private int nextClientId;

        private TcpListener tcpListener;

        private Thread connectionListener;

        private List<Client> clientList;

        /// <summary>
        /// The synchronization context of the user interface.
        /// </summary>
        private SynchronizationContext uiContext;

        public Server(int port, SynchronizationContext uiContext)
        {
            this.Port = port;
            this.uiContext = uiContext;
            this.connectionListener = null;
            this.clientList = new List<Client>();
            this.LogList = new ObservableCollection<LogEntry>();
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port of the controller. </value>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(nameof(this.Port), "The port must be > 0.");
                }
                else if (value > 65535)
                {
                    throw new ArgumentException(nameof(this.Port), "The port must be < 65536.");
                }
                else
                {
                    this.port = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the log list.
        /// </summary>
        /// <value>The log list of the controller. </value>
        public ObservableCollection<LogEntry> LogList { get; set; }

        public void ConnectionListenerStart()
        {
            if (this.connectionListener == null || this.connectionListener.ThreadState != ThreadState.Running)
            {
                this.connectionListener = new Thread(new ThreadStart(ConnectionListenerWorker));
                this.connectionListener.IsBackground = true;
                this.connectionListener.Start();
            }
        }

        public void ConnectionListenerWorker()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
            tcpListener.Start();
            this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Server started on port {this.Port}")), null);

            while (true)
            {
                TcpClient tmpClient = tcpListener.AcceptTcpClient();
                this.clientList.Add(new Client(this.GetNextClientId(), tmpClient));
                this.clientList[this.clientList.Count - 1].ConnectionManager.StartListening();
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(this.nextClientId, DateTime.Now, LogMessageType.Info, $"Client-{this.nextClientId} connected! IpAdress:{((IPEndPoint)tmpClient.Client.RemoteEndPoint).Address.ToString()}")), null);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Generates the next client id.
        /// </summary>
        /// <returns>The next client id.</returns>
        private int GetNextClientId()
        {
            this.nextClientId++;
            return this.nextClientId;
        }
    }
}
