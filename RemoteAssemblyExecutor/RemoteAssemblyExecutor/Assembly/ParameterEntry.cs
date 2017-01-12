//-----------------------------------------------------------------------
// <copyright file="ParameterEntry.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class ParameterEntry.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class ParameterEntry.
    /// </summary>
    [Serializable]
    public class ParameterEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterEntry"/> class.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter type.</param>
        public ParameterEntry(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public string Value { get; set; }

        /// <summary>
        /// The ToString method.
        /// </summary>
        /// <returns>A string indicating the class.</returns>
        public override string ToString()
        {
            return this.Name + " - " + this.Type;
        }
    }
}