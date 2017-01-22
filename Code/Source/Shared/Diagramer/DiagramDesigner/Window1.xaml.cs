using System.Windows;
using Victoria.UI.SharedWPF;
using DiagramDesigner;

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
        }

        public DesignerCanvas diagrama() {
            return this.MyDesigner;
        }

        
    }
}
