//-----------------------------------------------------------------------
// <copyright file="AssemblyMember.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class AssemblyMember.</summary>
//-----------------------------------------------------------------------

namespace RemoteAssemblyExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the class AssemblyMember.
    /// </summary>
    [Serializable]
    public class AssemblyMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyMember"/> class.
        /// </summary>
        /// <param name="name">The assembly name.</param>
        /// <param name="declairingFullname">The full name of the class.</param>
        /// <param name="isStatic">If the method is static.</param>
        /// <param name="type">The member type.</param>
        /// <param name="paramList">The parameter list.</param>
        public AssemblyMember(string name, string declairingFullname, bool isStatic, MemberType type, List<ParameterEntry> paramList)
        {
            this.Name = name;
            this.DeclairingFullname = declairingFullname;
            this.IsStatic = isStatic;
            this.Type = type;
            this.ParamList = paramList;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class. </value>
        public string DeclairingFullname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the method is static.
        /// </summary>
        /// <value>If the method is static.</value>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type of the member.</value>
        public MemberType Type { get; set; }

        /// <summary>
        /// Gets or sets the parameter list..
        /// </summary>
        /// <value>The parameter list of the member.</value>
        public List<ParameterEntry> ParamList { get; set; }

        /// <summary>
        /// Gets the name and parameter.
        /// </summary>
        /// <value>The name of the assembly. </value>
        public string NameAndParameters
        {
            get
            {
                string parameters = string.Empty;

                for (int i = 0; i < this.ParamList.Count; i++)
                {
                    parameters = parameters + this.ParamList[i].Type + " " + this.ParamList[i].Name;
                    if (i < this.ParamList.Count - 1)
                    {
                        parameters = parameters + ", ";
                    }
                }

                return this.Name + " (" + parameters + ")";
            }
        }

        /// <summary>
        /// The ToString method.
        /// </summary>
        /// <returns>A string indicating the class.</returns>
        public override string ToString()
        {
            return this.Name + " - " + this.Type + " - #Params:" + this.ParamList.Count;
        }
    }
}
