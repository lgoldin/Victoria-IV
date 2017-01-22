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
    /// Interaction logic for LoginPopuP.xaml
    /// </summary>
    public partial class GetWebExcercisePopUp : Window
    {
        public UI.SharedWPF.DialogResult Result { get; set; }

        public string File { get; set; }

        public string User { get; set; }

        public string Password
        {
            get
            {
                return PasswordBox.Password;
            }
        }

        public bool GetUserExcercisesOk { get; set; }

        public GetWebExcercisePopUp()
        {
            InitializeComponent();
        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            //LAMADO A WEB SERVICE -> 
            var exercisesNamesList = this.GetExcercisesNameListByUser(this.User, this.Password);

            if(exercisesNamesList == null || !exercisesNamesList.Any())
            {
                this.GetUserExcercisesOk = false;
                this.Result = UI.SharedWPF.DialogResult.Accept;
                this.Close();
                return;
            }

            this.GetUserExcercisesOk = true;
            var excercisesListWindow = new ExcercisesList(exercisesNamesList);
            excercisesListWindow.ShowDialog();
            switch (excercisesListWindow.Result)
            {
                case UI.SharedWPF.DialogResult.Accept:
                    {
                        //LLAMADO A SERVICIO DE DESCARGA
                        this.File = this.GetExcerciseByName(excercisesListWindow.SelectedExcerciseName);

                        this.Result = UI.SharedWPF.DialogResult.Accept;
                        this.Close();
                    }
                    break;
                case UI.SharedWPF.DialogResult.Cancel:
                    {
                        this.Result = UI.SharedWPF.DialogResult.Cancel;
                        this.Close();
                    }
                    break;
            }
        }

        //llamado a ws fake
        public IEnumerable<string> GetExcercisesNameListByUser(string user, string password)
        {
            if(user == "hola")
            {
                return null;
            }

            return new List<string>
            {
                "Ejer1.vic",
                "Cola2.vic",
                "blablaba.vic",
                "Ejer1.vic",
                "Cola2.vic",
                "blablaba.vic",
                "Ejer1.vic",
                "Cola2.vic",
                "blablaba.vic",
                "Ejer1.vic",
                "Cola2.vic",
                "blablaba.vic",
                "erererer.vic"
            };
        }

        //llamado a ws fake
        public string GetExcerciseByName(string fileName)
        {
            return "fake";
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
