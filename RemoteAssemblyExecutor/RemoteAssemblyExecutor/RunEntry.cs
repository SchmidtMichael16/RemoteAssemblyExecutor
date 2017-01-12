//-----------------------------------------------------------------------
// <copyright file="RunEntry.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class RunEntry.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class RunEntry.
    /// </summary>
    public class RunEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunEntry"/> class.
        /// </summary>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="result">The result.</param>
        public RunEntry(DateTime startDateTime, long duration, string assemblyName, string methodName, object[] parameters, string result)
        {
            this.StartDateTime = startDateTime;
            this.Duration = duration;
            this.AssemblyName = assemblyName;
            this.Parameters = string.Empty;
            this.MethodName = methodName;
            this.Result = result;

            for (int i = 0; i < parameters.Length; i++)
            {
                this.Parameters += parameters[i];

                if (i < parameters.Length - 1)
                {
                    this.Parameters += ", ";
                }
            }
        }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        /// <value>The start date time.</value>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the assembly name.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the method name.
        /// </summary>
        /// <value>The method name.</value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public string Parameters { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public string Result { get; set; }
    }
}
