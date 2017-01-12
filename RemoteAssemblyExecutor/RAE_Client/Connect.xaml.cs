//-----------------------------------------------------------------------
// <copyright file="Connect.xaml.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class Connect.</summary>
//-----------------------------------------------------------------------

namespace RAE_Client
{
    using System.Net;
    using System.Windows;

    /// <summary>
    /// Represent the class Connect.
    /// </summary>
    public partial class Connect : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connect"/> class.
        /// </summary>
        public Connect()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port of the server.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>The IP address of the server. </value>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// The callback method of the connect click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BtConnect_Click(object sender, RoutedEventArgs e)
        {
            string strIpadress = TxtIpAdress.Text;
            string strPort = TxtPort.Text;
            IPAddress ipadress;
            int port;

            if (IPAddress.TryParse(strIpadress, out ipadress))
            {
                this.IPAddress = ipadress;
                if (int.TryParse(strPort, out port))
                {
                    if (port < 0)
                    {
                        MessageBox.Show("The port must be > 0.");
                    }
                    else if (port > 65535)
                    {
                        MessageBox.Show("The port must be < 65536.");
                    }
                    else
                    {
                        this.Port = port;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Port ist not valid!");
                }
            }
            else
            {
                MessageBox.Show("IP adress ist not valid!");
            }
        }

        /// <summary>
        /// The callback method of the abort click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BtAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
