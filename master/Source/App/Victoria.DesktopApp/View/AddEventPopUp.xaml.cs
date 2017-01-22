using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Victoria.Shared.AnalisisPrevio;
using Victoria.UI.SharedWPF;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for AddEventPopUp.xaml
    /// </summary>
    public partial class AddEventPopUp : Window
    {

        public DialogResult Result { get; set; }

        public EventoAP Evento { get; set; }

        public AnalisisPrevio AnalisisPrevio { get; set; }

        public AddEventPopUp()
        {
            InitializeComponent();
        }

        public AddEventPopUp(AnalisisPrevio analisisPrevio, ObservableCollection<string> conditions)
        {
            InitializeComponent();
            Evento = new EventoAP();
            titulo.Content = "Nuevo Evento";
            eventoCondicionado.ItemsSource = analisisPrevio.EventosEaE.Select(item => item.Nombre); ;
            eventoNoCondicionado.ItemsSource = analisisPrevio.EventosEaE.Select(item => item.Nombre);
            condicion.ItemsSource = conditions;
            encadenador.ItemsSource = analisisPrevio.Datos;
            this.AnalisisPrevio = analisisPrevio;
            arrepentimiento.IsEnabled = (nombre.Text == "Salida" ? false : true);

        }

        public AddEventPopUp(AnalisisPrevio analisisPrevio, ObservableCollection<string> conditions, EventoAP eventoAP) : this(analisisPrevio, conditions)
        {
            InitializeComponent();
            eventoCondicionado.ItemsSource = analisisPrevio.EventosEaE.Select(item => item.Nombre);
            eventoNoCondicionado.ItemsSource = analisisPrevio.EventosEaE.Select(item => item.Nombre);
            condicion.ItemsSource = conditions;
            encadenador.ItemsSource = analisisPrevio.Datos;
            titulo.Content = "Editar Evento";
            Evento = eventoAP;
            eventoCondicionado.SelectedItem = Evento.EventoCondicionado;
            eventoNoCondicionado.SelectedItem = Evento.EventoNoCondicionado;
            encadenador.SelectedItem = Evento.Encadenador;
            arrepentimiento.IsChecked = Evento.Arrepentimiento;
            vector.IsChecked = Evento.Vector;
            condicion.SelectedItem = Evento.Condicion;
            nombre.Text = Evento.Nombre;
            nombre.IsReadOnly = true;
            arrepentimiento.IsEnabled = (nombre.Text == "Salida"? false : true);
            TEF.Text = Evento.TEF;
            this.AnalisisPrevio = analisisPrevio;


        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Accept;
            
            Evento.Nombre = nombre.Text;

            
            if(eventoCondicionado.SelectedItem != null)
            {
                Evento.EventoCondicionado = eventoCondicionado.SelectedItem.ToString();
            }

            if (eventoNoCondicionado.SelectedItem != null)
            {
                Evento.EventoNoCondicionado = eventoNoCondicionado.SelectedItem.ToString();
            }

            if (encadenador.SelectedItem != null)
            {
                Evento.Encadenador = encadenador.SelectedItem.ToString();
            }
          

            Evento.Arrepentimiento = (bool) arrepentimiento.IsChecked;

            Evento.Vector = (bool)vector.IsChecked;

            if (condicion.SelectedItem != null) { 
                Evento.Condicion = condicion.SelectedItem.ToString();
            }
            if (this.AnalisisPrevio.EventosEaE.Any(item => (item.TEF == TEF.Text.ToUpper() && item.Nombre != Evento.Nombre)))
            {
                new AlertPopUp("Error. Ya existe un evento con el campo TEF especificado.").Show();
                return;
            } else { 
     
                Evento.TEF = TEF.Text.ToUpper();
            }
            this.Close();
            


        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Cancel;
            this.Close();
        }
    }
}
