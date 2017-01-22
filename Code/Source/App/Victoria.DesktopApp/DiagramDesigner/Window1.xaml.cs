using System.Windows;
using Victoria.UI.SharedWPF;
using DiagramDesigner;
using System.ComponentModel;
using System.Windows.Input;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {

        public DialogResult Result { get; set; }

        private string stageName;
        public string StageName
        {
            get { return this.stageName; }
            set { this.stageName = value; }
        }
        public Window1()
        {
            InitializeComponent();
            Closing += HideWindow;
            this.diagrama().dataGridVariables = this.dataGridVariables;
        }

        public DesignerCanvas diagrama() {
            return this.MyDesigner;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        public void HideWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Normal;
                this.dataGridVariables.Height = 180;
            } else { 
                this.WindowState = WindowState.Maximized;
                this.dataGridVariables.Height = 300;
            }
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
