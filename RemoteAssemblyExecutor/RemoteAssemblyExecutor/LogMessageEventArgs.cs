//-----------------------------------------------------------------------
// <copyright file="LogMessageEventArgs.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class LogMessageEventArgs.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;

    /// <summary>
    /// Represent the class LogMessageEventArgs.
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageEventArgs"/> class.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        public LogMessageEventArgs(LogEntry logEntry)
        {
            this.LogEntry = logEntry;
        }

        /// <summary>
        /// Gets or sets the log entry.
        /// </summary>
        /// <value>The log entry.</value>
        public LogEntry LogEntry { get; set; }
    }
}

