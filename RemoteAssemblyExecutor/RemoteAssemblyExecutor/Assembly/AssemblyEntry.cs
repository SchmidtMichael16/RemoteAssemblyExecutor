//-----------------------------------------------------------------------
// <copyright file="AssemblyEntry.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class AssemblyEntry.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class AssemblyEntry.
    /// </summary>
    [Serializable]
    public class AssemblyEntry
    {
        /// <summary>
        /// The assembly.
        /// </summary>
        [NonSerialized]
        private Assembly assembly;

        /// <summary>
        /// The app domain.
        /// </summary>
        [NonSerialized]
        private AppDomain appDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyEntry"/> class.
        /// </summary>
        /// <param name="name">The name of the assembly.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="members">The members of the assembly.</param>
        /// <param name="dependencies">The referenced assemblies.</param>
        public AssemblyEntry(string name, string filename, Assembly assembly, List<AssemblyMember> members, List<Dependency> dependencies)
        {
            this.Name = name;
            this.FileName = filename;
            this.Assembly = assembly;
            this.Members = members;
            this.Dependencies = dependencies;

            if (this.Members == null)
            {
                this.Members = new List<AssemblyMember>();
            }

            if (this.Dependencies == null)
            {
                this.Dependencies = new List<Dependency>();
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename of the assembly.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The internal id of the assembly.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        public Assembly Assembly
        {
            get
            {
                return this.assembly;
            }

            set
            {
                this.assembly = value;
            }
        }

        /// <summary>
        /// Gets or sets the app domain.
        /// </summary>
        /// <value>The app domain.</value>
        public AppDomain AppDomain
        {
            get
            {
                return this.appDomain;
            }

            set
            {
                this.appDomain = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of members.
        /// </summary>
        /// <value>The list of members.</value>
        public List<AssemblyMember> Members { get; set; }

        /// <summary>
        /// Gets or sets the list of referenced assemblies.
        /// </summary>
        /// <value>The list of referenced assemblies.</value>
        public List<Dependency> Dependencies { get; set; }

        /// <summary>
        /// Gets a value indicating whether all dependencies are available.
        /// </summary>
        /// <value>If all dependencies are available.</value>
        public bool AllDependenciesAvailable
        {
            get
            {
                if (this.Dependencies != null)
                {
                    for (int i = 0; i < this.Dependencies.Count; i++)
                    {
                        if (!this.Dependencies[i].Available)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }

                return true;
            }
        }
    }
}
