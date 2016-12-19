//-----------------------------------------------------------------------
// <copyright file="NetworkPacketEventArgs.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class NetworkPacketEventArgs.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class NetworkPacketEventArgs.
    /// </summary>
    public class NetworkPacketEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPacketEventArgs"/> class.
        /// </summary>
        /// <param name="packet">The network packet.</param>
        public NetworkPacketEventArgs(NetworkPacket packet)
        {
            this.Packet = packet;
        }

        /// <summary>
        /// Gets or sets the network packet.
        /// </summary>
        /// <value>The network packet.</value>
        public NetworkPacket Packet { get; set; }

        /// <summary>
        /// Gets or sets the info text.
        /// </summary>
        /// <value>The info text packet.</value>
        public string Infotext { get; set; }
    }
}
