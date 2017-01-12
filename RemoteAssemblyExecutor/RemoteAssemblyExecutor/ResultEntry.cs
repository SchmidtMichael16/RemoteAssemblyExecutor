//-----------------------------------------------------------------------
// <copyright file="ResultEntry.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class ResultEntry.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class ResultEntry.
    /// </summary>
    [Serializable]
    public class ResultEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultEntry"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="type">The return type of the method.</param>
        public ResultEntry(string result, string type)
        {
            this.Result = result;
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result of the method.</value>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The return type of the method.</value>
        public string Type { get; set; }
    }
}
