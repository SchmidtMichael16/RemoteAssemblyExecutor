//-----------------------------------------------------------------------
// <copyright file="Dependency.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class Dependency.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Represent the class Dependency.
    /// </summary>
    [Serializable]
    public class Dependency
    {
        /// <summary>
        /// The background color.
        /// </summary>
        [NonSerialized]
        private SolidColorBrush backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        /// <param name="available">If the dependency is available.</param>
        public Dependency(string name, bool available)
        {
            this.Name = name;
            this.Available = available;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the assembly. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dependency is available.
        /// </summary>
        /// <value>If the dependency is available.</value>
        public bool Available { get; set; }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        /// <value>The background color. </value>
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (this.Available)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
        }

        /// <summary>
        /// The ToString method.
        /// </summary>
        /// <returns>A string indicating the class.</returns>
        public override string ToString()
        {
            return this.Name + " - Available:" + this.Available;
        }
    }
}
