﻿//-----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class ConnectionManager.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;

    /// <summary>
    /// Represent the class ConnectionManager.
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// The listener thread.
        /// </summary>
        private Thread listenerThread;

        /// <summary>
        /// The network stream.
        /// </summary>
        private NetworkStream networkStream;

        /// <summary>
        /// If the thread should listen on the stream.
        /// </summary>
        private bool listen;

        /// <summary>
        /// The thread lock for sending packets.
        /// </summary>
        private object sendLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        /// <param name="networkStream">The network stream.</param>
        public ConnectionManager(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
            this.sendLock = new object();
        }

        /// <summary>
        /// Gets fired if a packet was received.
        /// </summary>
        public event EventHandler<NetworkPacketEventArgs> OnPacketReceived;

        /// <summary>
        /// Gets fired if a new log entry has to be written.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> OnNewLogEntry;

        /// <summary>
        /// Starts listening.
        /// </summary>
        public void StartListening()
        {
            this.listenerThread = new Thread(new ThreadStart(this.WorkerListening));
            this.listenerThread.IsBackground = true;
            this.listen = true;
            this.listenerThread.Start();
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public void StopListening()
        {
            this.listen = false;
            try
            {
                if (this.listenerThread != null || this.listenerThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    this.listenerThread.Abort();
                }
            }
            catch (Exception ex)
            {
                this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error while trying to abort the process monitor thread. --> {ex.Message}")));
            }
        }

        /// <summary>
        /// The worker method of the listening thread.
        /// </summary>
        public void WorkerListening()
        {
            while (this.listen)
            {
                try
                {
                    if (this.networkStream != null && this.networkStream.DataAvailable)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream;
                        byte[] packetLengthBuffer = new byte[4];
                        byte[] bytesBuffer = new byte[8192];
                        int readBytes = 0;
                        int packetLegth = 0;

                        readBytes = this.networkStream.Read(packetLengthBuffer, 0, packetLengthBuffer.Length);
                        packetLegth = BitConverter.ToInt32(packetLengthBuffer, 0);

                        bytesBuffer = new byte[packetLegth];
                        readBytes = this.networkStream.Read(bytesBuffer, 0, bytesBuffer.Length);

                        stream = new MemoryStream(bytesBuffer);
                        try
                        {
                            NetworkPacket receivedPacket;
                            receivedPacket = (NetworkPacket)formatter.Deserialize(stream);

                            if (receivedPacket.PacketType == PacketType.ServerCommand)
                            {
                            }
                            else
                            {
                                this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Info, $"{receivedPacket.PacketType} Packet with length {packetLegth} received!")));
                            }

                            this.FireOnPacketReceived(new NetworkPacketEventArgs(receivedPacket));
                        }
                        catch (Exception ex)
                        {
                            this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during receiving network packet! - {ex.Message}")));
                        }
                    }
                    else
                    {
                        this.SendAliveMessage();
                    }
                }
                catch (Exception)
                {
                    this.FireOnPacketReceived(new NetworkPacketEventArgs(new NetworkPacket() { PacketType = PacketType.ServerCommand }));
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Sends close connection.
        /// </summary>
        /// <param name="recepient">The recipient.</param>
        public void SendCloseConnection(string recepient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.ServerCommand };
            this.SendPacket(tmpPacket, recepient, "SendCloseConnection");
            this.StopListening();
        }

        /// <summary>
        /// Sends an alive message.
        /// </summary>
        public void SendAliveMessage()
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.AliveMessage };
            ////this.SendPacket(tmpPacket, string.Empty, "AliveMessage");
        }

        /// <summary>
        /// Sends a info message to the client.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendInfoMessage(string message, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.InfoMessage, InfoMessage = message };
            this.SendPacket(tmpPacket, recipient, "InfoMessage");
        }

        /// <summary>
        /// Sends start method to the server.
        /// </summary>
        /// <param name="member">The assembly member.</param>
        /// <param name="appDomainId">The domain app.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendStartMethod(AssemblyMember member, int appDomainId, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.StartMethod, Member = member, AppDomainId = appDomainId };
            this.SendPacket(tmpPacket, recipient, "StartMehtod");
        }

        /// <summary>
        /// Sends a log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendLogEntry(LogEntry logEntry, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.LogEntry, LogEntry = logEntry };
            this.SendPacket(tmpPacket, recipient, "SendLogEntry");
        }

        /// <summary>
        /// Sends a result entry.
        /// </summary>
        /// <param name="resultEntry">The result entry.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendResultEntry(ResultEntry resultEntry, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.ResultEntry, ResultEntry = resultEntry };
            this.SendPacket(tmpPacket, recipient, "SendResultEntry");
        }

        /// <summary>
        /// Sends an assembly.
        /// </summary>
        /// <param name="buffer">The assembly as byte array.</param>
        /// <param name="filename">The filename of the assembly.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendAssemblie(byte[] buffer, string filename, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.Assemblie, Buffer = buffer, InfoMessage = filename };
            this.SendPacket(tmpPacket, recipient, "SendAssemblie");
        }

        /// <summary>
        /// Sends the assembly list.
        /// </summary>
        /// <param name="assemblyList">The assembly list.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendAssemblieList(List<AssemblyEntry> assemblyList, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.AssemblieList, AssemblyList = assemblyList };
            this.SendPacket(tmpPacket, recipient, "SendAssemblieList");
        }

        /// <summary>
        /// Sends delete assembly.
        /// </summary>
        /// <param name="fullname">The full name of the assembly.</param>
        /// <param name="recipient">The recipient.</param>
        public void SendDeleteAssembly(string fullname, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.DeleteAssembly, InfoMessage = fullname };
            this.SendPacket(tmpPacket, recipient, "SendDeleteAssembly");
        }

        /// <summary>
        /// Close the network stream.
        /// </summary>
        public void CloseConnection()
        {
            this.networkStream.Close();
        }

        /// <summary>
        /// Sends a network packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="recipient">The recipient info.</param>
        /// <param name="requestInfo">The request info.</param>
        private void SendPacket(NetworkPacket packet, string recipient, string requestInfo)
        {
            lock (this.sendLock)
            {
                try
                {
                    byte[] byteBuffer;
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, packet);
                        byteBuffer = stream.ToArray();
                    }

                    // Send length of the network packet.
                    this.networkStream.Write(BitConverter.GetBytes((int)byteBuffer.Length), 0, 4);
                    if (packet.PacketType != PacketType.AliveMessage)
                    {
                        this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Info, $"Send length of the {requestInfo} packet  --> {(Int32)byteBuffer.Length} to {recipient}!")));
                    }

                    // Send network packet.
                    this.networkStream.Write(byteBuffer, 0, byteBuffer.Length);

                    if (packet.PacketType != PacketType.AliveMessage)
                    {
                        this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Info, $"Send data of {requestInfo} to  {recipient}!")));
                    }
                }
                catch (Exception ex)
                {
                    this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Error, $"Error during send {requestInfo} to  {recipient}! " + ex.Message)));
                }
            }
        }

        /// <summary>
        /// Gets fired if a packet was received.
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
        /// Gets fired if a new log entry has to be written.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void FireOnNewLogEntry(LogMessageEventArgs e)
        {
            if (this.OnNewLogEntry != null)
            {
                this.OnNewLogEntry(this, e);
            }
        }
    }
}
