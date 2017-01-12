//-----------------------------------------------------------------------
// <copyright file="Server.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class Server.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.Remoting.Contexts;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class Server.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The port of the server.
        /// </summary>
        private int port;

        /// <summary>
        /// The next id.
        /// </summary>
        private int nextClientId;

        /// <summary>
        /// The TCP listener.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// The connection listener.
        /// </summary>
        private Thread connectionListener;

        /// <summary>
        /// The client list.
        /// </summary>
        private List<Client> clientList;

        /// <summary>
        /// The assembly list.
        /// </summary>
        private List<AssemblyEntry> assemblyList;

        /// <summary>
        /// The app domain id.
        /// </summary>
        private int appDomainId;

        /// <summary>
        /// The synchronization context of the user interface.
        /// </summary>
        private SynchronizationContext uiContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="port">The port of the server.</param>
        /// <param name="uiContext">The context of the user interface.</param>
        public Server(int port, SynchronizationContext uiContext)
        {
            this.Port = port;
            this.uiContext = uiContext;
            this.connectionListener = null;
            this.clientList = new List<Client>();
            this.LogList = new ObservableCollection<LogEntry>();
            this.RunList = new ObservableCollection<RunEntry>();
            this.assemblyList = new List<AssemblyEntry>();
            this.appDomainId = 0;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port of the server. </value>
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
        /// <value>The log list of the server. </value>
        public ObservableCollection<LogEntry> LogList { get; set; }

        /// <summary>
        /// Gets or sets the run list.
        /// </summary>
        /// <value>The run list of the server. </value>
        public ObservableCollection<RunEntry> RunList { get; set; }

        /// <summary>
        /// Starts the thread connection listener.
        /// </summary>
        public void ConnectionListenerStart()
        {
            if (this.connectionListener == null || this.connectionListener.ThreadState != System.Threading.ThreadState.Running)
            {
                this.connectionListener = new Thread(new ThreadStart(this.ConnectionListenerWorker));
                this.connectionListener.IsBackground = true;
                this.connectionListener.Start();
            }
        }

        /// <summary>
        /// The worker of the listener thread.
        /// </summary>
        public void ConnectionListenerWorker()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
            this.tcpListener.Start();
            this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Server started on port {this.Port}")), null);

            while (true)
            {
                TcpClient tmpClient = this.tcpListener.AcceptTcpClient();
                this.clientList.Add(new Client(this.GetNextClientId(), tmpClient, this.uiContext));
                this.clientList[this.clientList.Count - 1].ConnectionManager.StartListening();
                this.clientList[this.clientList.Count - 1].ConnectionManager.OnNewLogEntry += this.ConnectionManager_OnNewLogEntry;
                this.clientList[this.clientList.Count - 1].ConnectionManager.OnPacketReceived += this.ConnectionManager_OnPacketReceived;
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(this.nextClientId, DateTime.Now, LogMessageType.Info, $"Client-{this.clientList[this.clientList.Count - 1].Id} connected! IpAdress:{((IPEndPoint)tmpClient.Client.RemoteEndPoint).Address.ToString()}")), null);
                this.clientList[this.clientList.Count - 1].ConnectionManager.SendAssemblieList(this.assemblyList, $"Client{this.clientList[this.clientList.Count - 1].Id}");
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Delete all files from the previous run.
        /// </summary>
        public void CleanUpWorkingDirectory()
        {
            try
            {
                string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.exe");

                // Delete all unnecessary exe files.
                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetFileName(files[i]) != "RAE_Server.exe" && Path.GetFileName(files[i]) != "RAE_Server.vshost.exe" && Path.GetFileName(files[i]) != "RemoteAssemblyExecutor.exe")
                    {
                        File.Delete(files[i]);
                    }
                }

                files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");

                // Delete all unnecessary dll files.
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }

                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Working directory cleaned up!")), null);
            }
            catch (Exception ex)
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during cleaning up working directory! - {ex.Message}")), null);
            }
        }

        /// <summary>
        /// The callback method of the connection manager on packet received event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ConnectionManager_OnPacketReceived(object sender, NetworkPacketEventArgs e)
        {
            this.uiContext.Send(x => this.LogList.Add(new LogEntry(e.Packet.ClientId, DateTime.Now, LogMessageType.Info, $"Packet received! {e.Packet.PacketType}")), null);

            if (e.Packet.PacketType == PacketType.Assemblie)
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(e.Packet.ClientId, DateTime.Now, LogMessageType.Info, $"Assembly '{e.Packet.InfoMessage}' loaded!")), null);

                if (!this.AssemblyExist(e.Packet.InfoMessage))
                {
                    this.SaveFile(e.Packet.InfoMessage, e.Packet.Buffer);
                    this.LoadAssemblyInNewAppDomain(e.Packet.InfoMessage, e.Packet.ClientId);
                }
                else
                {
                    this.uiContext.Send(x => this.LogList.Add(new LogEntry(e.Packet.ClientId, DateTime.Now, LogMessageType.Error, $"Assembly '{e.Packet.InfoMessage}' already exists!")), null);
                    this.clientList[this.GetClientIndexWithId(e.Packet.ClientId)].ConnectionManager.SendLogEntry(new LogEntry(e.Packet.ClientId, DateTime.Now, LogMessageType.Error, $"Assembly '{e.Packet.InfoMessage}' already exists!"), $"Client{e.Packet.ClientId}");
                }
            }
            else if (e.Packet.PacketType == PacketType.DeleteAssembly)
            {
                this.UnloadAssembly(e.Packet.InfoMessage);
            }
            else if (e.Packet.PacketType == PacketType.StartMethod)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartMethod), e.Packet);
            }
        }

        /// <summary>
        /// The callback method of the connection manager on new log entry event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ConnectionManager_OnNewLogEntry(object sender, LogMessageEventArgs e)
        {
            this.uiContext.Send(x => this.LogList.Add(e.LogEntry), null);
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

        /// <summary>
        /// Gets all referenced assemblies.
        /// </summary>
        /// <param name="asm">The current assembly.</param>
        /// <returns>The referenced assemblies.</returns>
        private List<Dependency> GetAllReferencedAssemblies(Assembly asm)
        {
            List<Dependency> retList = new List<Dependency>();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] allAssems = currentDomain.GetAssemblies();
            AssemblyName[] referencedAssemblies = asm.GetReferencedAssemblies();

            foreach (AssemblyName ass in referencedAssemblies)
            {
                retList.Add(new Dependency(ass.FullName, false));
            }

            return retList;
        }

        /// <summary>
        /// Refreshes the dependencies of the specified list.
        /// </summary>
        /// <param name="dependcyList">The dependency list.</param>
        private void RefreshDependacies(List<Dependency> dependcyList)
        {
            if (dependcyList == null)
            {
                return;
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] allAssems = currentDomain.GetAssemblies();

            for (int i = 0; i < dependcyList.Count; i++)
            {
                bool loaded = false;
                for (int j = 0; j < allAssems.Length; j++)
                {
                    if (dependcyList[i].Name == allAssems[j].FullName)
                    {
                        loaded = true;
                        break;
                    }
                }

                dependcyList[i].Available = loaded;
            }
        }

        /// <summary>
        /// Refreshes the members of the specified assembly entry.
        /// </summary>
        /// <param name="assemblyEntry">The assembly entry.</param>
        private void RefreshMembers(AssemblyEntry assemblyEntry)
        {
            assemblyEntry.Members.Clear();

            // Check if all domains were loaded
            if (assemblyEntry.AllDependenciesAvailable)
            {
                Type[] types = assemblyEntry.Assembly.GetTypes();

                foreach (Type type in types)
                {
                    ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    foreach (ConstructorInfo constructor in constructors)
                    {
                        ParameterInfo[] parameters = constructor.GetParameters();
                        List<ParameterEntry> paramList = new List<ParameterEntry>();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            paramList.Add(new ParameterEntry(parameters[i].Name, parameters[i].ParameterType.ToString()));
                        }

                        assemblyEntry.Members.Add(new AssemblyMember(constructor.DeclaringType.Name, constructor.DeclaringType.FullName, constructor.IsStatic, MemberType.Constructor, paramList));
                    }

                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
                    foreach (MethodInfo method in methods)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        List<ParameterEntry> paramList = new List<ParameterEntry>();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            paramList.Add(new ParameterEntry(parameters[i].Name, parameters[i].ParameterType.ToString()));
                        }

                        assemblyEntry.Members.Add(new AssemblyMember(method.Name, method.DeclaringType.FullName, method.IsStatic, MemberType.Method, paramList));
                    }
                }
            }
            else
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Warning, $"Assembly {assemblyEntry.FileName} --> Not all referenced assemblies available!")), null);
            }
        }

        /// <summary>
        /// Refreshes all dependencies and members.
        /// </summary>
        private void RefreshAllDependaciesAndMembers()
        {
            for (int i = 0; i < this.assemblyList.Count; i++)
            {
                this.RefreshDependacies(this.assemblyList[i].Dependencies);
                this.RefreshMembers(this.assemblyList[i]);
            }
        }

        /// <summary>
        /// Stores a assembly.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="buffer">The byte array to store.</param>
        private void SaveFile(string filename, byte[] buffer)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + filename))
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during storeing assemblie! - The file {filename} already exist!")), null);
                return;
            }

            try
            {
                using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\" + filename, FileMode.CreateNew))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(buffer);
                }
            }
            catch (Exception ex)
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during storeing assemblie! - {ex.Message}")), null);
            }
        }

        /// <summary>
        /// Loads an assembly in a new app domain.
        /// </summary>
        /// <param name="filename">The filename of the assembly.</param>
        /// <param name="clientId">The client id.</param>
        private void LoadAssemblyInNewAppDomain(string filename, int clientId)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\" + filename))
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during storeing assembly! - The file {filename} already exist!")), null);
                return;
            }

            try
            {
                AppDomain newDomain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(filename));
                newDomain.Load(Path.GetFileNameWithoutExtension(filename));
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Assembly {filename} --> Loaded!")), null);
                Assembly[] assemblies = newDomain.GetAssemblies();

                foreach (Assembly asm in assemblies)
                {
                    if (asm.ManifestModule.Name == filename)
                    {
                        AssemblyEntry newAssembly = new AssemblyEntry(asm.FullName, filename, asm, null, null);
                        newAssembly.Dependencies = this.GetAllReferencedAssemblies(asm);
                        this.RefreshDependacies(newAssembly.Dependencies);
                        newAssembly.AppDomain = newDomain;
                        newAssembly.Id = this.GetNextAppDomainId();

                        this.assemblyList.Add(newAssembly);
                        this.RefreshAllDependaciesAndMembers();
                        this.SendAssembliesToAllClients();
                    }
                }
            }
            catch (Exception ex)
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during loading assembly! - {ex.Message}")), null);
                this.clientList[this.GetClientIndexWithId(clientId)].ConnectionManager.SendLogEntry(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during loading assembly! - {ex.Message}"), $"Client{clientId}");
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    for (int i = 0; i < loaderExceptions.Length; i++)
                    {
                        this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during loading assemblie! - {loaderExceptions[i].ToString()}")), null);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the specified assembly.
        /// </summary>
        /// <param name="fullname">The name of the assembly.</param>
        private void UnloadAssembly(string fullname)
        {
            try
            {
                for (int i = 0; i < this.assemblyList.Count; i++)
                {
                    if (this.assemblyList[i].Name == fullname)
                    {
                        AppDomain.Unload(this.assemblyList[i].AppDomain);
                        this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Assembly {this.assemblyList[i].FileName} unloaded!")), null);
                        this.assemblyList.RemoveAt(i);
                        this.RefreshAllDependaciesAndMembers();
                        this.SendAssembliesToAllClients();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during unloading assembly! - {ex.Message}")), null);
            }
        }

        /// <summary>
        /// Starts a method.
        /// </summary>
        /// <param name="stateInfo">The network packet containing the assembly member.</param>
        private void StartMethod(object stateInfo)
        {
            NetworkPacket packet = (NetworkPacket)stateInfo;

            AssemblyMember member = packet.Member;
            int id = packet.AppDomainId;
            int clientId = packet.ClientId;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                DateTime startDateTime = DateTime.Now;
                Type[] types = new Type[member.ParamList.Count];
                object[] parameters = new object[member.ParamList.Count];
                for (int i = 0; i < member.ParamList.Count; i++)
                {
                    var type = Type.GetType(member.ParamList[i].Type);
                    parameters[i] = Convert.ChangeType(member.ParamList[i].Value, type);
                    types[i] = type;
                }

                if (member.Type == MemberType.Method)
                {
                    AppDomain appDomain = this.GetAppDomainWithId(id);
                    Assembly asm = this.GetAssemblyWithId(id);
                    var type = asm.GetType(member.DeclairingFullname);
                    var instance = Activator.CreateInstance(type);
                    var method = type.GetMethod(member.Name, types);
                    this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Start method {member.Name}...")), null);

                    stopwatch.Start();
                    var returnValue = method.Invoke(instance, parameters);
                    stopwatch.Stop();

                    this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Method {member.Name} finished!")), null);
                    if (returnValue == null)
                    {
                        this.uiContext.Send(x => this.RunList.Add(new RunEntry(startDateTime, stopwatch.ElapsedMilliseconds, asm.FullName, member.Name, parameters, string.Empty)), null);
                        this.clientList[this.GetClientIndexWithId(clientId)].ConnectionManager.SendResultEntry(new ResultEntry(string.Empty, method.ReturnParameter.ParameterType.FullName), $"Client{clientId}");
                    }
                    else
                    {
                        this.uiContext.Send(x => this.RunList.Add(new RunEntry(startDateTime, stopwatch.ElapsedMilliseconds, asm.FullName, member.Name, parameters, returnValue.ToString())), null);
                        this.clientList[this.GetClientIndexWithId(clientId)].ConnectionManager.SendResultEntry(new ResultEntry(returnValue.ToString(), method.ReturnParameter.ParameterType.FullName), $"Client{clientId}");
                    }
                }
                else if (member.Type == MemberType.Constructor)
                {
                    Assembly asm = this.GetAssemblyWithId(id);
                    var type = asm.GetType(member.DeclairingFullname);
                    stopwatch.Start();
                    var instance = Activator.CreateInstance(type, parameters);
                    stopwatch.Stop();

                    this.uiContext.Send(x => this.LogList.Add(new LogEntry(DateTime.Now, LogMessageType.Info, $"Constructor {member.Name} finished!")), null);

                    this.uiContext.Send(x => this.RunList.Add(new RunEntry(startDateTime, stopwatch.ElapsedMilliseconds, asm.FullName, member.Name, parameters, instance.ToString())), null);
                    this.clientList[this.GetClientIndexWithId(clientId)].ConnectionManager.SendResultEntry(new ResultEntry(instance.ToString(), instance.ToString()), $"Client{clientId}");
                }
            }
            catch (Exception ex)
            {
                LogEntry newLogEntry = new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during execute method/constructor {member.Name}! - {ex.Message}");
                this.uiContext.Send(x => this.LogList.Add(newLogEntry), null);
                this.clientList[this.GetClientIndexWithId(clientId)].ConnectionManager.SendLogEntry(newLogEntry, $"Client{clientId}");
            }
        }

        /// <summary>
        /// Gets the app domain with the specified id.
        /// </summary>
        /// <param name="id">The id of the app domain.</param>
        /// <returns>The app domain.</returns>
        private AppDomain GetAppDomainWithId(int id)
        {
            for (int i = 0; i < this.assemblyList.Count; i++)
            {
                if (this.assemblyList[i].Id == id)
                {
                    return this.assemblyList[i].AppDomain;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the assembly with the specified id.
        /// </summary>
        /// <param name="id">The id of the assembly.</param>
        /// <returns>The assembly.</returns>
        private Assembly GetAssemblyWithId(int id)
        {
            for (int i = 0; i < this.assemblyList.Count; i++)
            {
                if (this.assemblyList[i].Id == id)
                {
                    return this.assemblyList[i].Assembly;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the client with the specified id.
        /// </summary>
        /// <param name="id">The id of the client.</param>
        /// <returns>The index of the client.</returns>
        private int GetClientIndexWithId(int id)
        {
            for (int i = 0; i < this.clientList.Count; i++)
            {
                if (this.clientList[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sends the assembly list to all clients.
        /// </summary>
        private void SendAssembliesToAllClients()
        {
            for (int i = 0; i < this.clientList.Count; i++)
            {
                this.clientList[i].ConnectionManager.SendAssemblieList(this.assemblyList, "Client" + this.clientList[i].Id);
            }
        }

        /// <summary>
        /// Gets the next domain app id.
        /// </summary>
        /// <returns>The next domain app id.</returns>
        private int GetNextAppDomainId()
        {
            this.appDomainId++;
            return this.appDomainId;
        }

        /// <summary>
        /// Checks if the specified assembly already exist.
        /// </summary>
        /// <param name="filenmae">The name of the assembly.</param>
        /// <returns>If the specified assembly already exist.</returns>
        private bool AssemblyExist(string filenmae)
        {
            for (int i = 0; i < this.assemblyList.Count; i++)
            {
                if (this.assemblyList[i].FileName == filenmae)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
