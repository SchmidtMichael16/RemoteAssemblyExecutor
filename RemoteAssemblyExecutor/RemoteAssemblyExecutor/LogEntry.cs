//-----------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class LogEntry.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class LogEntry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="date">The date and time.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageText">The message text.</param>
        public LogEntry(int clientId, DateTime date, LogMessageType messageType, string messageText)
        {
            this.ClientId = clientId;
            this.Date = date;
            this.MessageType = messageType;
            this.MessageText = messageText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="date">The date and time.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageText">The message text.</param>
        public LogEntry(DateTime date, LogMessageType messageType, string messageText)
        {
            this.ClientId = 0;
            this.Date = date;
            this.MessageType = messageType;
            this.MessageText = messageText;
        }

        /// <summary>
        /// Gets or sets a value indicating the id.
        /// </summary>
        /// <value>The id of the log entry.</value>
        public int ClientId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the date.
        /// </summary>
        /// <value>The date of the log entry.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the message type.
        /// </summary>
        /// <value>The message type of the log entry.</value>
        public LogMessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the message text.
        /// </summary>
        /// <value>The message text of the log entry.</value>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets a value indicating the date string.
        /// </summary>
        /// <value>The date string of the log entry.</value>
        public string DateString
        {
            get
            {
                return this.Date.ToShortDateString() + " " + this.Date.ToLongTimeString() + ":" + this.Date.Millisecond;
            }
        }
    }
}

