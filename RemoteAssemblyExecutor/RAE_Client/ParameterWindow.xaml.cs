//-----------------------------------------------------------------------
// <copyright file="ParameterWindow.xaml.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class ParameterWindow.</summary>
//-----------------------------------------------------------------------

namespace RAE_Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using RemoteAssemblyExecutor;

    /// <summary>
    /// Represent the class ParameterWindow.
    /// </summary>
    public partial class ParameterWindow : Window
    {
        /// <summary>
        /// The parameter list.
        /// </summary>
        private List<ParameterEntry> parameterList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterWindow"/> class.
        /// </summary>
        /// <param name="parameterList">The parameter list.</param>
        public ParameterWindow(List<ParameterEntry> parameterList)
        {
            this.InitializeComponent();
            this.parameterList = parameterList;
            this.ParameterList.ItemsSource = this.parameterList;
            this.Start = false;
        }

        /// <summary>
        /// Gets a value indicating whether the user clicked on start.
        /// </summary>
        /// <value>If the user clicked on start.</value>
        public bool Start
        {
            get;
            private set;
        }

        /// <summary>
        /// The callback method of the start click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this.Start = true;
            this.Close();
        }
    }
}
