using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddConditionPopUp.xaml
    /// </summary>
    public partial class AddConditionPopUp : Window
    {
        private AnalisisPrevio analisisPrevio;

        public DialogResult Result { get; set; }

        public string Condition
        {
            get
            {
                return condicion;
            }

            set
            {
                condicion = value;
            }
        }

        private List<string> operadores;

        private string condicion;
        
        public AddConditionPopUp(AnalisisPrevio analisisPrevio)
        {
            this.analisisPrevio = analisisPrevio;
            InitializeComponent();
            this.variableo1c1.ItemsSource = listaDeVariables();
            this.variableo1c2.ItemsSource = listaDeVariables();
            this.variableo2c1.ItemsSource = listaDeVariables();
            this.variableo2c2.ItemsSource = listaDeVariables();
            tipoo1c1.SelectedIndex = 0;
            tipoo2c1.SelectedIndex = 0;
            tipoo1c2.SelectedIndex = 0;
            tipoo2c2.SelectedIndex = 0;
            variableo1c1.SelectedIndex = 0;
            variableo1c2.SelectedIndex = 0;
            variableo2c1.SelectedIndex = 0;
            variableo2c2.SelectedIndex = 0;
            changeComponents(tipoo1c1, valoro1c1, variableo1c1);
            changeComponents(tipoo2c1, valoro2c1, variableo2c1);
            changeComponents(tipoo1c2, valoro1c2, variableo1c2);
            changeComponents(tipoo2c2, valoro2c2, variableo2c2);
            operadores = new List<string>();
            operadores.Add("==");
            operadores.Add("!=");
            operadores.Add("<");
            operadores.Add("<=");
            operadores.Add(">");
            operadores.Add(">=");
            operadorc1.ItemsSource = operadores;
            operadorc2.ItemsSource = operadores;
            operadorc1.SelectedItem = operadores.ElementAt(0);
            operadorc2.SelectedItem = operadores.ElementAt(0);
            this.Condition = "";

        }

        private IEnumerable listaDeVariables()
        {
            var collection = new ObservableCollection<VariableAP>();
            foreach(VariableAP item in analisisPrevio.VariablesEstado) {
                collection.Add(item);
            }
            foreach (String item in analisisPrevio.VariablesDeControl)
            {
                collection.Add(new VariableAP { nombre = item, valor = 0 });
            }

            collection.Add(new VariableAP { nombre = "T", valor = 0 });

            foreach (EventoAP item in analisisPrevio.EventosEaE)
            {
                collection.Add(new VariableAP { nombre = item.TEF, valor = 0 });
            }

            return collection;
        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Cancel;
            this.Close();
        }

        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            if (validateOperands())
            {
                new AlertPopUp("Debe completar los valores de los operandos y operadores.").Show();
            }else {
                this.Result = UI.SharedWPF.DialogResult.Accept;
                createCondition();
                this.Close();
                new InformationPopUp("Condición creada con éxito.").ShowDialog();
            }
        }

        private void createCondition()
        {
            this.Condition =
                createConditionGrid(tipoo1c1, valoro1c1, variableo1c1, tipoo2c1,
                valoro2c1, variableo2c1, operadorc1.SelectedItem.ToString()); 
            if(encadenador.SelectedItem != ninguno) { 
             this.Condition += (encadenador.SelectedItem == and ? " && "  : " ││ ") +
                createConditionGrid(tipoo1c2, valoro1c2, variableo1c2, tipoo2c2,
                valoro2c2, variableo2c2, operadorc2.SelectedItem.ToString());
            }
        }

      

        private string createConditionGrid(ComboBox tipoo1, TextBox valoro1, ComboBox variableo1, ComboBox tipoo2, TextBox valoro2, ComboBox variableo2, string operador)
        {
            return obtenerOperando(tipoo1, valoro1, variableo1) + " " + operador+ " " + obtenerOperando(tipoo2, valoro2, variableo2);
        }

        private string obtenerOperando(ComboBox tipo, TextBox valor, ComboBox variable)
        {
            return tipo.SelectedIndex == 0 ? variable.SelectedItem.ToString() : valor.Text;
        }

        private bool validateOperands()
        {
            return (tipoo1c1.SelectedIndex != 0 && String.IsNullOrEmpty(valoro1c1.Text) 
                || tipoo2c1.SelectedIndex != 0 && String.IsNullOrEmpty(valoro2c1.Text)
                || tipoo1c2.SelectedIndex != 0 && String.IsNullOrEmpty(valoro1c2.Text)
                || tipoo2c2.SelectedIndex != 0 && String.IsNullOrEmpty(valoro2c2.Text)
                || tipoo1c1.SelectedIndex == 0 && variableo1c1.SelectedItem == null
                || tipoo1c2.SelectedIndex == 0 && variableo1c2.SelectedItem == null
                || tipoo2c1.SelectedIndex == 0 && variableo2c1.SelectedItem == null
                || tipoo2c2.SelectedIndex == 0 && variableo2c2.SelectedItem == null);
               
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsNumber(c)); 

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            condicion2Grid.Visibility = encadenador.SelectedItem == ninguno ? Visibility.Hidden : Visibility.Visible;
        }

        private void comboBoxTipoo1c1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeComponents(tipoo1c1, valoro1c1, variableo1c1);
        }

        private void comboBoxTipoo2c1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeComponents(tipoo2c1, valoro2c1, variableo2c1);
        }

        private void comboBoxTipoo1c2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeComponents(tipoo1c2, valoro1c2, variableo1c2);
        }

        private void comboBoxTipoo2c2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeComponents(tipoo2c2, valoro2c2, variableo2c2);
        }

        private void changeComponents(ComboBox tipo, TextBox valor, ComboBox variable)
        {
            valor.Visibility = tipo.SelectedIndex == 0 ? Visibility.Hidden : Visibility.Visible;
            variable.Visibility = tipo.SelectedIndex == 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
