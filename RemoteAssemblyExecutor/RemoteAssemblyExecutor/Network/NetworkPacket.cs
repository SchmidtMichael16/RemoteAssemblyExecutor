//-----------------------------------------------------------------------
// <copyright file="NetworkPacket.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class NetworkPacket.</summary>
//-----------------------------------------------------------------------
namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;

    /// <summary>
    /// Represent the class NetworkPacket.
    /// </summary>
    [Serializable]
    public class NetworkPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPacket"/> class.
        /// </summary>
        public NetworkPacket()
        {
        }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>The client id of the packet. </value>
        public int ClientId { get; set; }

        /// <summary>
        /// Gets or sets the app domain id.
        /// </summary>
        /// <value>The app domain id of the packet. </value>
        public int AppDomainId { get; set; }

        /// <summary>
        /// Gets or sets the new priority.
        /// </summary>
        /// <value>The new priority of the packet. </value>
        public int NewPrio { get; set; }

        /// <summary>
        /// Gets or sets the info text.
        /// </summary>
        /// <value>The info text of the packet. </value>
        public string InfoMessage { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>The method name.</value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the packet type.
        /// </summary>
        /// <value>The packet type of the packet. </value>
        public PacketType PacketType { get; set; }

        /// <summary>
        /// Gets or sets the sender address.
        /// </summary>
        /// <value>The sender address of the packet. </value>
        public IPAddress SenderAdress { get; set; }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>The buffer of the packet. </value>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets or sets the assembly list.
        /// </summary>
        /// <value>The assembly list of the packet. </value>
        public List<AssemblyEntry> AssemblyList { get; set; }

        /// <summary>
        /// Gets or sets the assembly member.
        /// </summary>
        /// <value>The assembly member of the packet. </value>
        public AssemblyMember Member { get; set; }

        /// <summary>
        /// Gets or sets the log entry.
        /// </summary>
        /// <value>The log entry. </value>
        public LogEntry LogEntry { get; set; }

        /// <summary>
        /// Gets or sets the result entry.
        /// </summary>
        /// <value>The result entry. </value>
        public ResultEntry ResultEntry { get; set; }
    }
}
