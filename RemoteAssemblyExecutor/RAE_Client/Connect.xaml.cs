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
using System.Net;

namespace RAE_Client
{
    /// <summary>
    /// Interaktionslogik für Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        public Connect()
        {
            InitializeComponent();
        }
        
        public int Port { get; set; }

        public IPAddress IPAddress { get; set; }

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

        private void BtAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
