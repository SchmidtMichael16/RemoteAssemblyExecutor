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
        /// The packet Type ServerCommand.
        /// </summary>
        ServerCommand,

        /// <summary>
        /// The packet Type ControlCommand.
        /// </summary>
        ControlCommand,

        /// <summary>
        /// The packet Type Modules.
        /// </summary>
        Modules,

        /// <summary>
        /// The packet Type InfoMessage.
        /// </summary>
        InfoMessage,

        /// <summary>
        /// The packet Type InfoMessage.
        /// </summary>
        StartMethod,

        /// <summary>
        /// The packet Type AliveMessage.
        /// </summary>
        AliveMessage,

        /// <summary>
        /// The packet Type AliveMessage.
        /// </summary>
        Assemblie,

        /// <summary>
        /// The packet Type AssemblyList.
        /// </summary>
        AssemblieList,

        /// <summary>
        /// The packet Type DeleteAssembly.
        /// </summary>
        DeleteAssembly,

        /// <summary>
        /// The packet Type LogEntry.
        /// </summary>
        LogEntry,

        /// <summary>
        /// The packet Type ResultEntry.
        /// </summary>
        ResultEntry
    }
}
