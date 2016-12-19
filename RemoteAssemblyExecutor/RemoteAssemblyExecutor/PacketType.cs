//-----------------------------------------------------------------------
// <copyright file="PacketType.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class PacketType.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    /// <summary>
    /// Represent the class PacketType.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// The log packet Type ServerCommand.
        /// </summary>
        ServerCommand,

        /// <summary>
        /// The log packet Type ControlCommand.
        /// </summary>
        ControlCommand,

        /// <summary>
        /// The log packet Type Modules.
        /// </summary>
        Modules,

        /// <summary>
        /// The log packet Type InfoMessage.
        /// </summary>
        InfoMessage,

        /// <summary>
        /// The log packet Type AliveMessage.
        /// </summary>
        AliveMessage
    }
}
