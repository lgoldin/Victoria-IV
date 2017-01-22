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
using Victoria.UI.SharedWPF;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for AddStageWindow.xaml
    /// </summary>
    public partial class AddStageWindow : Window
    {
        public DialogResult Result { get; set; }

        private string stageName;
        public string StageName
        {
            get { return this.stageName; }
            set { this.stageName = value; }
        }

        public AddStageWindow()
        {
            InitializeComponent();
        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Accept;
            this.Close();
        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Cancel;
            this.Close();
        }

    }
}
