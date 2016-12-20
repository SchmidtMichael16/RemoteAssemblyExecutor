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
        /// Gets or sets the process id.
        /// </summary>
        /// <value>The process id of the packet. </value>
        public int ProcessId { get; set; }

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
        /// Gets or sets the name of the mehtod.
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
    }
}

