//-----------------------------------------------------------------------
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
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        /// <param name="networkStream">The network stream.</param>
        public ConnectionManager(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
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
                    this.FireOnPacketReceived(new NetworkPacketEventArgs(new NetworkPacket() { PacketType = PacketType.ServerCommand}));
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
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.ServerCommand};
            this.SendPacket(tmpPacket, recepient, "SendCloseConnection");
            this.StopListening();
        }

        /// <summary>
        /// Sends an alive message.
        /// </summary>
        public void SendAliveMessage()
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.AliveMessage };
            //this.SendPacket(tmpPacket, string.Empty, "AliveMessage");
        }

        public void SendInfoMessage(string message, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.InfoMessage, InfoMessage = message };
            this.SendPacket(tmpPacket, recipient, "InfoMessage");
        }

        public void SendStartMethod(string methodName, string recipient)
        {
            NetworkPacket tmpPacket = new NetworkPacket() { PacketType = PacketType.StartMethod, MethodName = methodName };
            this.SendPacket(tmpPacket, recipient, "StartMehtod");
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
            try
            {
                byte[] byteBuffer;
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, packet);
                    //stream.Seek(0, SeekOrigin.Begin);
                    //NetworkPacket tmpPacket = (NetworkPacket)formatter.Deserialize(stream);
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
                this.FireOnNewLogEntry(new LogMessageEventArgs(new LogEntry(DateTime.Now, LogMessageType.Info, $"Error during send {requestInfo} to  {recipient}! " + ex.Message)));
                //this.FireOnPacketReceived(new NetworkPacketEventArgs(new NetworkPacket() { PacketType = PacketType.ServerCommand, CommandType = CommandType.CloseConnection }));
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

