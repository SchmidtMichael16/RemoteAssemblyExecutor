//-----------------------------------------------------------------------
// <copyright file="StartWindow.xaml.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class StartWindow.</summary>
//-----------------------------------------------------------------------

namespace RAE_Server
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

    /// <summary>
    /// Represent the class StartWindow.
    /// </summary>
    public partial class StartWindow : Window
    {
        /// <summary>
        /// The port of the server.
        /// </summary>
        private int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartWindow"/> class.
        /// </summary>
        public StartWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user clicked on start.
        /// </summary>
        /// <value>If the user clicked on start.</value>
        public bool Start { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port of the controller. </value>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(nameof(this.Port), "The port must be > 0.");
                }
                else if (value > 65535)
                {
                    throw new ArgumentException(nameof(this.Port), "The port must be < 65536.");
                }
                else
                {
                    this.port = value;
                }
            }
        }

        /// <summary>
        /// The callback method of the start click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            string strPort = TxtPort.Text;

            if (int.TryParse(strPort, out this.port))
            {
                if (this.port < 0)
                {
                    MessageBox.Show("The port must be > 0.");
                }
                else if (this.port > 65535)
                {
                    MessageBox.Show("The port must be < 65536.");
                }
                else
                {
                    this.Start = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Port ist not valid!");
            }
        }
    }
}
