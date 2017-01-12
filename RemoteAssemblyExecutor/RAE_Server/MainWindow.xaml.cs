//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="fhwn.ac.at">
//     Copyright (c) fhwn.ac.at. All rights reserved.
// </copyright>
// <author>Michael Schmidt</author>
// <summary>Class ParameterWindow.</summary>
//-----------------------------------------------------------------------

namespace RAE_Server
{
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls; 
    using RemoteAssemblyExecutor;

    /// <summary>
    /// Represent the class MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The server.
        /// </summary>
        private Server server;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            
            var startWindow = new StartWindow();

            if (startWindow.ShowDialog() == false)
            {
                if (startWindow.Start)
                {
                    this.server = new Server(startWindow.Port, SynchronizationContext.Current);
                    this.server.CleanUpWorkingDirectory();
                    this.RunList.ItemsSource = this.server.RunList;

                    var listViewLog = this.ListViewLog.FindName("LogList") as ListView;

                    if (listViewLog != null)
                    {
                        listViewLog.ItemsSource = this.server.LogList;
                    }

                    this.server.ConnectionListenerStart();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// The callback method of the port click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void MenuItem_Port_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
