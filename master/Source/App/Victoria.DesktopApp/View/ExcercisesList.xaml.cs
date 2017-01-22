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
    /// Interaction logic for ExcercisesList.xaml
    /// </summary>
    public partial class ExcercisesList : Window
    {
        public UI.SharedWPF.DialogResult Result { get; set; }

        public string SelectedExcerciseName
        {
            get
            {
                return (string)ExcercisesListBox.SelectedItem;
            }
        }

        public ExcercisesList(IEnumerable<string> excerciseNames)
        {
            InitializeComponent();
            ExcercisesListBox.ItemsSource = excerciseNames;
        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(SelectedExcerciseName))
            {
                this.Result = UI.SharedWPF.DialogResult.Accept;
            }
            else
            {
                this.Result = UI.SharedWPF.DialogResult.Cancel;
            }
            this.Close();
        }


        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Cancel;
            this.Close();
        }

        private void mainBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }

        }
    }
}
