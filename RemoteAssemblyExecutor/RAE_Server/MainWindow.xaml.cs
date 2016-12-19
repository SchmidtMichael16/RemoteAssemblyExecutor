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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RemoteAssemblyExecutor;
using System.Threading;

namespace RAE_Server
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server;

        public MainWindow()
        {
            InitializeComponent();
            var startWindow = new StartWindow();
            //startWindow.ShowDialog();

            if (startWindow.ShowDialog() == false)
            {
                if (startWindow.Start)
                {
                    
                    this.server = new Server(startWindow.Port, SynchronizationContext.Current);
                    
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

        private void MenuItem_Port_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
