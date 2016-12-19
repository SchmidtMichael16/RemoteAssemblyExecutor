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

namespace RAE_Server
{
    /// <summary>
    /// Interaktionslogik für StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private int port;
        
        public StartWindow()
        {
            InitializeComponent();
        }

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

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            string strPort = TxtPort.Text;

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
