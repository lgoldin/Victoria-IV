using DiagramDesigner;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : Window
    {
        private Window1 diagramWindow;

        public string SimulationXML { get; set; } //acá pone una url relativa a la pc donde esta el .vic

        public string SimulationXmlFile { get; set; } //esto es lo que tenemos que cargar si descargamos de web. Xml plano

        public Window1 DiagramWindow 
        {
            get
            {
                if (this.diagramWindow == null)
                {
                    this.diagramWindow = new Window1();
                }

                return this.diagramWindow;
            }

            set
            {
                this.diagramWindow = value;
            }
        }

        public WelcomeView()
        {
            InitializeComponent();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnOpenSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Vic files (*.vic)|*.vic";
                    openFileDialog.Title = "Abrir Simulacion";
                    openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        SimulationXML = openFileDialog.FileName;
                        var mainWindow = new MainWindow(SimulationXML, false);
                        mainWindow.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                var viewExpection = new AlertPopUp("Se produjo un error al abrir la silumación. Para ver detalles, despliegue el control correspondiente.");
                viewExpection.ShowDialog();
            }
        }

        private void btnOpenWebSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            try {
                var webExcercisePopUp = new GetWebExcercisePopUp();
                webExcercisePopUp.ShowDialog();
                switch (webExcercisePopUp.Result)
                {
                    case UI.SharedWPF.DialogResult.Accept:
                        {
                            if (!webExcercisePopUp.GetUserExcercisesOk)
                            {
                                var alertPopUp = new AlertPopUp("Las credenciales ingresadas son invalidas");
                                alertPopUp.Show();
                                return;
                            }

                            if (string.IsNullOrEmpty(webExcercisePopUp.File))
                            {
                                var alertPopUp = new AlertPopUp("Error en descarga de ejercicio seleccionado. Intente más tarde");
                                alertPopUp.Show();
                                return;
                            }

                            SimulationXmlFile = webExcercisePopUp.File;
                            this.DialogResult = true;
                            this.Close();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var viewExpection = new AlertPopUp("Se produjo un error al abrir la silumación. Para ver detalles, despliegue el control correspondiente.");
                viewExpection.ShowDialog();
            }
        }

        private void btnOpenExercise_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var addExercisePopUp = new Window1();
                addExercisePopUp.Height = 650;
                addExercisePopUp.ShowDialog();
                switch (addExercisePopUp.Result)
                {
                    case UI.SharedWPF.DialogResult.Accept:
                        {
                            ((MainViewModel)this.DataContext).AddStageCommand.Execute(addExercisePopUp.StageName);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al agregar un escenario. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void btnAnalisisPrevio_OnClick(object sender, RoutedEventArgs e)
        {
            
                var addExercisePopUp = new AddAnalisisPrevioPopUp(DiagramWindow);
                addExercisePopUp.ShowDialog();
        
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnNew_Exercise_OnClick(object sender, RoutedEventArgs e)
        {
            DiagramWindow.diagrama().Children.Clear();
            DiagramWindow.diagrama().Public_Open();
            if (DiagramWindow.diagrama().Children.Count > 0)
            {
                DiagramWindow.Height = 650;
                DiagramWindow.ShowDialog();
            }            
        }
    }
}