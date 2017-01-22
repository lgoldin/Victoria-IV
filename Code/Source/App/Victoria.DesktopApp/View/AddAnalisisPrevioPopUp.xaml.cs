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
using System.Collections.ObjectModel;
using Victoria.Shared.AnalisisPrevio;
using DiagramDesigner;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using Microsoft.Win32;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for AddAnalisisPrevioPopUp.xaml
    /// </summary>
    public partial class AddAnalisisPrevioPopUp : Window
    {
        private static object _syncLock = new object();

        private AnalisisPrevio  _analisisPrevio;

        private ObservableCollection<string> _conditions;

        private Window1 VentanaDiagramador { get; set; }

        public AnalisisPrevio AnalisisPrevio
        {
            get
            {
                return _analisisPrevio;
            }

            set
            {
                _analisisPrevio = value;
            }
        }

        public ObservableCollection<string> Conditions
        {
            get
            {
                return _conditions;
            }

            set
            {
                _conditions = value;
            }
        }

        public AddAnalisisPrevioPopUp(Window1 diagramador)
        {
            InitializeComponent();
            VentanaDiagramador = diagramador;
            comboBox.SelectedItem = EaE;
            AnalisisPrevio = new AnalisisPrevio(AnalisisPrevio.Tipo.EaE);
            initializeCollections();
            eventos.Visibility = Visibility.Visible;

        }

        public void initializeCollections()
        {
            Conditions = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.Datos, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.VariablesDeControl, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.VariablesEstado, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.VariablesResultado, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.EventosEaE, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.Propios, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.ComprometidosFuturos, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.ComprometidosAnterior, _syncLock);
            BindingOperations.EnableCollectionSynchronization(AnalisisPrevio.Tefs, _syncLock);
            BindingOperations.EnableCollectionSynchronization(Conditions, _syncLock);
            Conditions.Add("");
            datos.ItemsSource = AnalisisPrevio.Datos;
            variablesControl.ItemsSource = AnalisisPrevio.VariablesDeControl;
            variablesEstado.ItemsSource = AnalisisPrevio.VariablesEstado;
            variablesResultado.ItemsSource = AnalisisPrevio.VariablesResultado;
            propios.ItemsSource = AnalisisPrevio.Propios;
            comprometidosAnteriores.ItemsSource = AnalisisPrevio.ComprometidosAnterior;
            comprometidosFuturos.ItemsSource = AnalisisPrevio.ComprometidosFuturos;
            tefs.ItemsSource = AnalisisPrevio.Tefs;
            dataGridEventos.ItemsSource = AnalisisPrevio.EventosEaE;
        }

        private void BtnMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_Dato(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.Datos);
        }

        private void MenuItem_Click_Control(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.VariablesDeControl);
        }

        private void MenuItem_Click_Estado(object sender, RoutedEventArgs e)
        {
            AgregarVariableVectorAP(AnalisisPrevio.VariablesEstado);
        }

        private void AgregarVariableAP(ObservableCollection<VariableAP> collection)
        {
            AddSimpleVariablePopUp popUp = new AddSimpleVariablePopUp();
            
            popUp.ShowDialog();
            if (popUp.Result == UI.SharedWPF.DialogResult.Accept)
            {
                string variable = popUp.nombreBox.Text.ToUpper();
                if (variable.ToUpper().Equals("TF") || variable.ToUpper().Equals("T") || variable.ToUpper().Equals("HV") || variable.ToUpper().Equals("R"))
                {
                    new AlertPopUp("No puede agregarse una variable con ese nombre, se encuentra reservado.").Show();
                    return;
                }
                if (!collection.Any(item => item.nombre == variable))
                {
                    collection.Add(new VariableAP() { nombre = variable, valor = 0});
                    new InformationPopUp("Variable creada con éxito.").ShowDialog();
                    return;
                } else {
                    new AlertPopUp("Error. Ya existe una variable con el nombre especificado.").Show();
                    return;
            }
        }
        }

        private void AgregarVariableVectorAP(ObservableCollection<VariableAP> collection)
        {
            AddVectorVariablePopUp popUp = new AddVectorVariablePopUp();

            popUp.ShowDialog();
            if (popUp.Result == UI.SharedWPF.DialogResult.Accept)
            {
                string variable = popUp.nombreBox.Text.ToUpper();
                if (variable.ToUpper().Equals("TF") || variable.ToUpper().Equals("T") || variable.ToUpper().Equals("HV") || variable.ToUpper().Equals("R") || variable.ToUpper().Equals("I") || variable.ToUpper().Equals("N"))
                {
                    new AlertPopUp("No puede agregarse una variable con ese nombre, se encuentra reservado.").Show();
                    return;
                }
                if (!collection.Any(item => item.nombre == variable))
                {
                    //variable = (bool) popUp.vector.IsChecked ? variable + "(I)" : variable;
                    collection.Add(new VariableAP() { nombre = variable, valor = 0, vector = (bool) popUp.vector.IsChecked });

                    new InformationPopUp("Variable creada con éxito.").ShowDialog();
                    return;
                }
                else {
                    new AlertPopUp("Error. Ya existe una variable con el nombre especificado.").Show();
                    return;
                }
            }
        }

        private void MenuItem_Click_Resultado(object sender, RoutedEventArgs e)
        {
            AgregarVariableVectorAP(AnalisisPrevio.VariablesResultado);
        }

        private void Agregar(ObservableCollection<string> collection)
        {

            AddSimpleVariablePopUp popUp = new AddSimpleVariablePopUp();
            popUp.ShowDialog(); 
            if(popUp.Result == UI.SharedWPF.DialogResult.Accept)
            {
                
                string variable = popUp.nombreBox.Text;
                if (variable.ToUpper().Equals("TF") || variable.ToUpper().Equals("T") || variable.ToUpper().Equals("HV") || variable.ToUpper().Equals("R") || variable.ToUpper().Equals("I") || variable.ToUpper().Equals("N"))
                { 
                    new AlertPopUp("Error. No puede agregarse una variable con ese nombre, se encuentra reservado.").Show();
                    return;
                }
                if (comboBox.SelectedItem == EaE)
                    variable = variable.ToUpper();
                if (!collection.Any(item => item == variable))
                {
                    collection.Add(variable);
                    new InformationPopUp("Variable creada con éxito.").ShowDialog();
                    return;
                } else {
                    new AlertPopUp("Error. Ya existe una variable con el nombre especificado.").Show();
                    return;
                }


            }


        }

        private void MenuItem_Click_EliminarDato(object sender, RoutedEventArgs e)
        {
            try {
                Eliminar(datos, AnalisisPrevio.Datos, true, "los Datos");         
            } catch (Exception ex) {
                new AlertPopUp(ex.Message).Show();
            }
     
        }

        private void MenuItem_Click_EliminarVarControl(object sender, RoutedEventArgs e)
        {
            Eliminar(variablesControl, AnalisisPrevio.VariablesDeControl, false, "las Variables de Control");
        }

        private void MenuItem_Click_EliminarVarEstado(object sender, RoutedEventArgs e)
        {
            Eliminar(variablesEstado, AnalisisPrevio.VariablesEstado, false, "las Variables de Estado");
        }

        private void MenuItem_Click_EliminarVarResultado(object sender, RoutedEventArgs e)
        {
            Eliminar(variablesResultado, AnalisisPrevio.VariablesResultado, false, "las Variables de Resultado");
        }

        private void MenuItem_Click_Evento(object sender, RoutedEventArgs e)
        {
         
            AddEventPopUp popUp = new AddEventPopUp(AnalisisPrevio, Conditions);
            popUp.ShowDialog();
            if(popUp.Result == UI.SharedWPF.DialogResult.Accept) { 

                if(AnalisisPrevio.EventosEaE.Any(item => item.Nombre == popUp.Evento.Nombre)) {
                    new AlertPopUp("Error. Ya existe un evento con el nombre especificado.").Show();
                    return;
                }
                if (AnalisisPrevio.EventosEaE.Any(item => item.TEF == popUp.Evento.TEF))
                {
                    new AlertPopUp("Error. Ya existe un evento con el campo TEF especificado.").Show();
                    return;
                }

                if (popUp.Evento.TEF.ToUpper().Equals("TF") || popUp.Evento.TEF.ToUpper().Equals("T") || popUp.Evento.TEF.ToUpper().Equals("HV") || popUp.Evento.TEF.ToUpper().Equals("R"))
                {
                    new AlertPopUp("Error. No puede agregarse un evento con ese nombre, se encuentra reservado.").Show();
                    return;
                }

                if (popUp.Evento.Nombre == null || popUp.Evento.Nombre.Equals("")) 
                {
                    new AlertPopUp("No se puede crear el Evento. El campo Nombre es obligatorio.").Show();
                    return;
                }
                if (popUp.Evento.TEF == null || popUp.Evento.TEF.Equals(""))
                {
                    new AlertPopUp("No se puede crear el Evento. El campo TEF es obligatorio.").Show();
                    return;
                }
               
                AnalisisPrevio.EventosEaE.Add(popUp.Evento);
                new InformationPopUp("Evento creado con éxito.").ShowDialog();
            }

        }



        private void btnDeleteEvento_OnClick(object sender, RoutedEventArgs e)
        {
            EventoAP selected = dataGridEventos.SelectedItem as EventoAP;
            if (selected != null)
            {
                if (selected.Nombre == "Llegada" || selected.Nombre == "Salida")
                {
                    new AlertPopUp("No se puede eliminar el Evento '" + selected.Nombre +"' cargado por defecto.").Show();
                }
                else {

                    foreach (EventoAP evento in AnalisisPrevio.EventosEaE)
                    {
                        if (evento.EventoCondicionado == selected.Nombre)
                        {
                            evento.EventoCondicionado = null;
                        }
                        if (evento.EventoNoCondicionado == selected.Nombre)
                        {
                            evento.EventoNoCondicionado = null;
                        }
                    }
                    AnalisisPrevio.EventosEaE.Remove(selected);
                    dataGridEventos.Items.Refresh();
                    dataGridEventos.SelectedItem = null;
                    new InformationPopUp("Evento eliminado con éxito.").ShowDialog();
                }
            }
        }

        private void btnEditEvento_OnClick(object sender, RoutedEventArgs e)
        {
          
                if (dataGridEventos.SelectedItem != null)
                {
                    AddEventPopUp popUp = new AddEventPopUp(AnalisisPrevio, Conditions, dataGridEventos.SelectedItem as EventoAP);
                    popUp.ShowDialog();
                    if(popUp.Result == UI.SharedWPF.DialogResult.Accept) { 
                        dataGridEventos.Items.Refresh();
                        dataGridEventos.SelectedItem = null;
                        new InformationPopUp("Evento editado con éxito.").ShowDialog();
                }
            }
           

        }

        private void Eliminar<T>(ListBox listBox, ObservableCollection<T> collection, bool isDatos, string title)
        {
            DeleteSimpleVariablePopUp popUp = new DeleteSimpleVariablePopUp(title);

            popUp.listBox.ItemsSource = listBox.ItemsSource;
            popUp.ShowDialog();
            List<T> toRemove = new List<T>();

            if (popUp.Result == UI.SharedWPF.DialogResult.Accept)
            {

                foreach (T selected in popUp.listBox.SelectedItems)
                {
                    if (!isDatos || (isDatos && !AnalisisPrevio.EventosEaE.Any(item => item.Encadenador == selected as string)))
                        toRemove.Add(selected);
                    else
                        throw new Exception("El dato '" + selected + "' no puede ser eliminado porque está asignado como Encadenador en la Tabla de Eventos." );
                }

                foreach (T removed in toRemove)
                {
                    collection.Remove(removed);
                }
                new InformationPopUp("Variable eliminada con éxito.").ShowDialog();
            }

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var tipo = tipoSeleccionado(comboBox.SelectedItem);
            AnalisisPrevio = new AnalisisPrevio(tipo);
            eventos.Visibility = tipo.Equals(AnalisisPrevio.Tipo.EaE)? Visibility.Visible:Visibility.Hidden;
            eventosDeltaT.Visibility = !tipo.Equals(AnalisisPrevio.Tipo.EaE) ? Visibility.Visible : Visibility.Hidden;
            nuevoEventoIndependiente.IsEnabled = tipo.Equals(AnalisisPrevio.Tipo.EaE);
            nuevoEventoDeltaT.IsEnabled = !tipo.Equals(AnalisisPrevio.Tipo.EaE);
            eliminarEventoDeltaT.IsEnabled = !tipo.Equals(AnalisisPrevio.Tipo.EaE);
            nuevaCondicion.IsEnabled = tipo.Equals(AnalisisPrevio.Tipo.EaE); 
            initializeCollections();
            }
                                      
        
        private AnalisisPrevio.Tipo tipoSeleccionado(object selectedItem)
        {
            return selectedItem == EaE ? AnalisisPrevio.Tipo.EaE : AnalisisPrevio.Tipo.DeltaT;
        
        }

        private void MenuItem_Click_Propio(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.Propios);
        }

        private void MenuItem_Click_ComprometidoAnterior(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.ComprometidosAnterior);
        }

        private void MenuItem_Click_ComprometidoFuturo(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.ComprometidosFuturos);
        }

        private void MenuItem_Click_Tef(object sender, RoutedEventArgs e)
        {
            Agregar(AnalisisPrevio.Tefs);
        }

        private void MenuItem_Click_EliminarPropio(object sender, RoutedEventArgs e)
        {
            Eliminar(propios, AnalisisPrevio.Propios, false, "los Eventos");
        }

        private void MenuItem_Click_EliminarComprometidoAnterior(object sender, RoutedEventArgs e)
        {
            Eliminar(comprometidosAnteriores, AnalisisPrevio.ComprometidosAnterior, false, "los Eventos");
        }

        private void MenuItem_Click_EliminarComprometidoFuturo(object sender, RoutedEventArgs e)
        {
            Eliminar(comprometidosFuturos, AnalisisPrevio.ComprometidosFuturos, false, "los Eventos");
        }

        private void MenuItem_Click_EliminarTef(object sender, RoutedEventArgs e)
        {
            Eliminar(tefs, AnalisisPrevio.Tefs, false, "los Eventos");
        }

        private void MenuItem_Click_Condicion(object sender, RoutedEventArgs e)
        {
           AddConditionPopUp popUp = new AddConditionPopUp(AnalisisPrevio);
            popUp.ShowDialog();
            if (popUp.Result == UI.SharedWPF.DialogResult.Accept)
            {
                this.Conditions.Add(popUp.Condition);
            }
        }

        private void btnGenerarDiagrama_OnClick(object sender, RoutedEventArgs e)
        {

            crearDiagrama(VentanaDiagramador.diagrama());
            VentanaDiagramador.Height = 650;
            List<VariableAP> variables = AnalisisPrevio.obtenerVariables(AnalisisPrevio.collecctionString());
            VentanaDiagramador.dataGridVariables.ItemsSource = variables;
            
            VentanaDiagramador.Show();

        }

        private void crearDiagrama(DesignerCanvas canvas)
        {
            var templateManager = new TemplateManager();
            XElement xml = crearXML(templateManager);
           
            canvas.Children.Clear();
            IEnumerable<XElement> itemsXML = xml.Elements("Diagrama").Elements("Flowchart").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
               
                DesignerItem item = DesignerCanvas.DeserializeDesignerItem(itemXML, id, 0, 0);
                string contentStr = itemXML.Element("Content").Value;
                foreach (var entry in templateManager.obtenerPlaceholders(AnalisisPrevio))
                {
                   contentStr = contentStr.Replace(entry.Key, System.Security.SecurityElement.Escape(entry.Value));
                }
                
                item.Content = XamlReader.Parse(contentStr);
                canvas.Children.Add(item);
                canvas.SetConnectorDecoratorTemplate(item);
            }
            canvas.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = xml.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = canvas.GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = canvas.GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                canvas.Children.Add(connection);
            }

            canvas.VariablesTag = AnalisisPrevio.generarTagDeVariables();
          

        }

        private XElement crearXML(TemplateManager templateManager )
        {   
            var parentFolder = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var absolutePath = System.IO.Path.Combine(parentFolder, @"templates");
            var fileName = templateManager.obtenerTemplate(AnalisisPrevio);
            XElement xml = XElement.Load(System.IO.Path.Combine(absolutePath, fileName));
            return xml;  
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MenuItem_Click_Ayuda(object sender, RoutedEventArgs e)
        {
            var parentFolder = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var sourcePath = System.IO.Path.Combine(parentFolder, @"manual de usuario\Manual de usuario Victoria.pdf");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Manual de usuario Victoria.pdf";
            saveFileDialog.Filter = "Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(sourcePath, saveFileDialog.FileName, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
