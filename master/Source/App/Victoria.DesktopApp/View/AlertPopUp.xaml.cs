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

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for AlertPopUp.xaml
    /// </summary>
    public partial class AlertPopUp : Window
    {
        public AlertPopUp(string alerta)
        {
            InitializeComponent();
            this.mensaje.Text = alerta;
        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
