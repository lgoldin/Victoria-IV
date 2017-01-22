using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using Victoria.DesktopApp;
using Victoria.Shared;
using Victoria.DesktopApp.Behavior;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Victoria.Shared.AnalisisPrevio;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Victoria.DesktopApp.View;
using System.Printing;
using System.Reflection;

namespace DiagramDesigner
{
    public partial class DesignerCanvas
    {
        public static RoutedCommand Group = new RoutedCommand();
        public static RoutedCommand Ungroup = new RoutedCommand();
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand AlignTop = new RoutedCommand();
        public static RoutedCommand AlignVerticalCenters = new RoutedCommand();
        public static RoutedCommand AlignBottom = new RoutedCommand();
        public static RoutedCommand AlignLeft = new RoutedCommand();
        public static RoutedCommand AlignHorizontalCenters = new RoutedCommand();
        public static RoutedCommand AlignRight = new RoutedCommand();
        public static RoutedCommand DistributeHorizontal = new RoutedCommand();
        public static RoutedCommand DistributeVertical = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();

        public DataGrid dataGridVariables { get; internal set; }

        public DesignerCanvas()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Group, Group_Executed, Group_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Ungroup, Ungroup_Executed, Ungroup_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringForward, BringForward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringToFront, BringToFront_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendBackward, SendBackward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendToBack, SendToBack_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignTop, AlignTop_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignVerticalCenters, AlignVerticalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignBottom, AlignBottom_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignLeft, AlignLeft_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignHorizontalCenters, AlignHorizontalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignRight, AlignRight_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeHorizontal, DistributeHorizontal_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeVertical, DistributeVertical_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveVic_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview, Imprimir_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, Help_Executed));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));

            this.AllowDrop = true;
            Clipboard.Clear();
        }
        
        private void Imprimir_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)this.Parent;
            scroll.ScrollToTop();

            //CODIGO DE PRINT DE DIAGRAMADOR
            SelectionService.ClearSelection();
            PrintDialog printDialog = new PrintDialog();
            if (true == printDialog.ShowDialog())
            {
              
                printDialog.PrintVisual(this, "Diagrama");
             }
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parentFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var sourcePath = Path.Combine(parentFolder, @"manual de usuario\Manual de usuario Victoria.pdf");
            
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

        #region New Command

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.SelectionService.ClearSelection();

        }

        #endregion

        #region Open Command

        public void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Public_Open();
        }

        public void Public_Open()
        {
            try
            {
                XElement root = LoadSerializedDataFromFile();

                if (root == null)
                    return;

                this.Children.Clear();
                this.SelectionService.ClearSelection();

                string content = root.Element("variables").Value;
                JObject jobject = JObject.Parse(content);
                List<VariableAP> variables = JsonConvert.DeserializeObject<List<VariableAP>>(jobject.GetValue("variables").ToString());
                dataGridVariables.ItemsSource = variables;
                IEnumerable<XElement> itemsXML = root.Elements("Diagrama").Elements("Flowchart").Elements("DesignerItem");
                foreach (XElement itemXML in itemsXML)
                {
                    Guid id = new Guid(itemXML.Element("ID").Value);
                    DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                    this.Children.Add(item);
                    SetConnectorDecoratorTemplate(item);
                }

                this.InvalidateVisual();

                IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
                foreach (XElement connectionXML in connectionsXML)
                {
                    Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                    Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al abrir el diagrama.");
                viewException.ShowDialog();
                return;
            }
        }

        #endregion

        #region Save Command

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Simulacion");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);
            root.Add(generarTagDeVariables());
            

            SaveFile(root);
        }

        public XElement generarTagDeVariables()
        {
           

            return new XElement("variables",
               @" { ""variables"":" + collectionJson() + @"}"); ;
        }

        public string collectionJson()
        {
           List<VariableAP> variables = new List<VariableAP>();
          
            foreach (VariableAP d in dataGridVariables.ItemsSource)
            {
                if (d.nombre != null && !d.nombre.Equals(""))
                {
                    
                    variables.Add(d);
                    
                }
          
            }
            VariableAP variableN = variables.Find(item => item.nombre.Equals("N"));

            
                foreach(VariableAP v in (variables.FindAll(item => item.vector))) {
                    string[] words = v.nombre.Split('(');
                    v.nombre = words.GetValue(0) + "("+ (variableN!=null?variableN.valor:10).ToString() +")";
                }

                if(variables.Find(item => item.vector) != null) { 
                    if(variableN == null )
                    {
                        variables.Add(new VariableAP() { nombre = "N", valor = 10, vector = false });
                    }
                    if (variables.Find(item => item.nombre.Equals("I")) == null)
                    {
                        variables.Add(new VariableAP() { nombre = "I", valor = 10, vector = false });
                    }
            }
                
          
            return JsonConvert.SerializeObject(variables);

        }

        #endregion


        #region Print Command

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Validación del diagrama
            var errorList = ValidateUseOfCorrectVariables().Concat(ValidateFinDiagrama());
            if (errorList.Any())
            {
                var viewException = new AlertPopUp(String.Join("\n", errorList.ToArray()));
                viewException.ShowDialog();
                return;
            }

            ///////////////////////////////////////ARMO XML Y LEVANTO VIKY 2 ////////////////////////
           
            try
            {
                var root = this.GenerateVicXmlOfDiagram();
                var mainWindow = new MainWindow(root.ToString(), true);
                mainWindow.Show();
            }
            catch (Victoria.DesktopApp.DiagramDesigner.LoopException ex)
            {
                var viewException = new AlertPopUp("Tu diagrama contiene un ciclo. Debes reemplazar esa conexión por un par de nodos referencia.");
                viewException.ShowDialog();
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Error de parseo. Revisa tu diagrama.");
                viewException.ShowDialog();
            }

            ////////////////////////////////////////////////////////////////////////////////////////

            //PASANDOLE UNA RUTA
            //var mainWindow = new MainWindow("C:\\Users\\Ignacio\\Desktop\\xmls\\Colas 1.vic", false);
            //mainWindow.Show();

            //PASANDOLE UN XML
            //var simuFile = File.ReadAllText("C:\\Users\\Ignacio\\Desktop\\xmls\\Colas 1.vic"); //Esta linea no iria, es de prueba..se sacaria el xml del diagrama
            //var mainWindow = new MainWindow(simuFile, true);
            //mainWindow.Show();

            //CODIGO QUE SUELE HABER CUANDO ABRIS V2 Y NO INCLUÍ (para hacer que se cierre la ap cuando cerras mundo2)
            //Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //Current.MainWindow = mainWindow;


            //CODIGO DE PRINT DE DIAGRAMADOR
            //SelectionService.ClearSelection();
            //PrintDialog printDialog = new PrintDialog();
            //if (true == printDialog.ShowDialog())
            //{
            //    printDialog.PrintVisual(this, "WPF Diagram");
            //}
        }

        #endregion

        #region Copy Command

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Paste Command

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

            // create DesignerItems
            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            List<ISelectable> newItems = new List<ISelectable>();
            IEnumerable<XElement> itemsXML = root.Elements("Diagrama").Elements("Flowchart").Elements("DesignerItem");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                DesignerItem item = DeserializeDesignerItem(itemXML, newID, offsetX, offsetY);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
                newItems.Add(item);
            }

            // update group hierarchy
            SelectionService.ClearSelection();
            foreach (DesignerItem el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerItem item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    SelectionService.AddToSelection(item);
                }
            }

            // create Connections
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);

                    SelectionService.AddToSelection(connection);
                }
            }

            DesignerCanvas.BringToFront.Execute(null, this);

            // update paste offset
            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Command

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Cut Command

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Group Command

        private void Group_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from item in this.SelectionService.CurrentSelection.OfType<DesignerItem>()
                        where item.ParentID == Guid.Empty
                        select item;

            Rect rect = GetBoundingRectangle(items);

            DesignerItem groupItem = new DesignerItem();
            groupItem.IsGroup = true;
            groupItem.Width = rect.Width;
            groupItem.Height = rect.Height;
            Canvas.SetLeft(groupItem, rect.Left);
            Canvas.SetTop(groupItem, rect.Top);
            Canvas groupCanvas = new Canvas();
            groupItem.Content = groupCanvas;
            Canvas.SetZIndex(groupItem, this.Children.Count);
            this.Children.Add(groupItem);

            foreach (DesignerItem item in items)
                item.ParentID = groupItem.ID;

            this.SelectionService.SelectItem(groupItem);
        }

        private void Group_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            int count = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                         where item.ParentID == Guid.Empty
                         select item).Count();

            e.CanExecute = count > 1;
        }

        #endregion

        #region Ungroup Command

        private void Ungroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var groups = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                          where item.IsGroup && item.ParentID == Guid.Empty
                          select item).ToArray();

            foreach (DesignerItem groupRoot in groups)
            {
                var children = from child in SelectionService.CurrentSelection.OfType<DesignerItem>()
                               where child.ParentID == groupRoot.ID
                               select child;

                foreach (DesignerItem child in children)
                    child.ParentID = Guid.Empty;

                this.SelectionService.RemoveFromSelection(groupRoot);
                this.Children.Remove(groupRoot);
                UpdateZIndex();
            }
        }

        private void Ungroup_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID != Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 0;
        }

        #endregion

        #region BringForward Command

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) descending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region BringToFront Command

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, childrenSorted.Count - selectionSorted.Count + j++);
                }
                else
                {
                    Canvas.SetZIndex(item, i++);
                }
            }
        }

        #endregion

        #region SendBackward Command

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) ascending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Max(i, currentIndex - 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region SendToBack Command

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();
            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, j++);

                }
                else
                {
                    Canvas.SetZIndex(item, selectionSorted.Count + i++);
                }
            }
        }        

        #endregion

        #region AlignTop Command

        private void AlignTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = top - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        private void Align_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region AlignVerticalCenters Command

        private void AlignVerticalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignBottom Command

        private void AlignBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignLeft Command

        private void AlignLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = left - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignHorizontalCenters Command

        private void AlignHorizontalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double center = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = center - (Canvas.GetLeft(item) + item.Width / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignRight Command

        private void AlignRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double right = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = right - (Canvas.GetLeft(item) + item.Width);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region DistributeHorizontal Command

        private void DistributeHorizontal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemLeft = Canvas.GetLeft(item)
                                orderby itemLeft
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Double.MaxValue;
                double right = Double.MinValue;
                double sumWidth = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    left = Math.Min(left, Canvas.GetLeft(item));
                    right = Math.Max(right, Canvas.GetLeft(item) + item.Width);
                    sumWidth += item.Width;
                }

                double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                double offset = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                    offset = offset + item.Width + distance;
                }
            }
        }

        private void Distribute_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region DistributeVertical Command

        private void DistributeVertical_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemTop = Canvas.GetTop(item)
                                orderby itemTop
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Double.MaxValue;
                double bottom = Double.MinValue;
                double sumHeight = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    top = Math.Min(top, Canvas.GetTop(item));
                    bottom = Math.Max(bottom, Canvas.GetTop(item) + item.Height);
                    sumHeight += item.Height;
                }

                double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                double offset = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                    offset = offset + item.Height + distance;
                }
            }
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();
        }

        #endregion

        #region SaveAs Command

        private void SaveVic_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var root = this.GenerateVicXmlOfDiagram();
            SaveFile(root);

            //Si descomentas esto, despues de guardar intenta levantar vik2
            //try
            //{
            //    var mainWindow = new MainWindow(root.ToString(), true);
            //    mainWindow.Show();
            //}
            //catch (Exception ex)
            //{
            //     MessageBoxResult result = MessageBox.Show("Error de parseo");
            //}

        }

        private XElement GenerateVicXmlOfDiagram()
        {
            VariablesTag = generarTagDeVariables();
            var variables = JsonConvert.DeserializeObject<List<VariableAP>>(collectionJson());

            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            List<XElement> listaDesignerItemsXML = SerializeVic(designerItems, connections, variables);

            XElement root = new XElement("Simulacion");
            listaDesignerItemsXML.ForEach(root.Add);

            return root;
        }

        private HashSet<string> ValidateUseOfCorrectVariables()
        {
            VariablesTag = generarTagDeVariables();

            var variables = JsonConvert.DeserializeObject<List<VariableAP>>(collectionJson());

            foreach(var variable in variables)
            {
                variable.nombre = variable.nombre.Split('(')[0];
            }

            var variableNames = variables.Select(x => x.nombre).ToList();
            
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            var errorList = new HashSet<string>();
            var referencesList = new List<string>();

            foreach (var item in designerItems)
            {
                if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "")
                {
                    var tipo = "";
                    var contenido = (Grid)item.Content;
                    var listaChildren = contenido.Children;
                    var pretipo = listaChildren.OfType<System.Windows.Shapes.Path>().FirstOrDefault();
                    if(pretipo != null)
                    {
                        tipo = ((UIElement)pretipo).GetAnimationBaseValue(ToolTipProperty).ToString();
                    }
                    var textBox = listaChildren.OfType<TextBox>().FirstOrDefault();

                    if (tipo.ToString() == "nodo_sentencia" || tipo.ToString() == "nodo_condicion")
                    {
                        ValidateUseOfCorrectVariablesInTextBox(errorList, textBox.Text, variableNames);
                        ValidateUseOfCorrectCharacters(errorList, textBox.Text);
                        if(tipo.ToString() == "nodo_condicion")
                        {
                            ValidateUseOfValidConditionOperators(errorList, textBox.Text);
                        }
                    }
                }

                if (((System.Windows.Controls.Grid)item.Content).Tag != null && ((System.Windows.Controls.Grid)item.Content).Tag.ToString() == "REFR")
                {

                    var tipo = "";
                    var contenido = (Grid)item.Content;
                    var listaChildren = contenido.Children;
                    var pretipo = listaChildren.OfType<System.Windows.Shapes.Ellipse>().FirstOrDefault();
                    if (pretipo != null)
                    {
                        tipo = ((UIElement)pretipo).GetAnimationBaseValue(ToolTipProperty).ToString();
                    }
                    var textBox = listaChildren.OfType<TextBox>().FirstOrDefault();

                    if (tipo.ToString() == "nodo_referencia")
                    {
                        referencesList.Add(textBox.Text);
                    }
                }
            }

            ValidateReferences(errorList, referencesList);

            return errorList;
        }


        private void ValidateReferences(HashSet<string> errorLIst, List<string> referencesList)
        {
            var masDeDosReferencias = referencesList.GroupBy(x => x)
                        .Where(group => group.Count() > 2)
                        .Select(group => group.Key);

            var soloUnaReferencia = referencesList.GroupBy(x => x)
                        .Where(group => group.Count() == 1)
                        .Select(group => group.Key);

            if (masDeDosReferencias.Any())
            {
                foreach (var refe in masDeDosReferencias)
                {
                    errorLIst.Add("-Se esta repitiendo el uso de la referencia " + '"' + refe + '"' + ". Por favor, utilice otro nombre.");
                }
            }

            if (soloUnaReferencia.Any())
            {
                foreach (var refe in soloUnaReferencia)
                {
                    errorLIst.Add("-No se esta cerrando el uso de la referencia " + '"' + refe + '"' + ".");
                }
            }
        }

        private HashSet<string> ValidateFinDiagrama()
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();
            var cantidadNodosFin = 0;
            var cantidadDiagramas = 0;
            var errorList = new HashSet<string>();

            foreach (var item in designerItems)
            {

                if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "DIAG")
                {
                    cantidadDiagramas++;
                }
                if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "")
                {
                  
                    var tipo = "";
                    var contenido = (Grid)item.Content;
                    var listaChildren = contenido.Children;
                    var pretipo = listaChildren.OfType<System.Windows.Shapes.Ellipse>().FirstOrDefault();
                    if (pretipo != null)
                    {
                        tipo = ((UIElement)pretipo).GetAnimationBaseValue(ToolTipProperty).ToString();
                    }
                    var textBox = listaChildren.OfType<TextBox>().FirstOrDefault();

                    if (tipo.ToString() == "nodo_fin" )
                    {
                        cantidadNodosFin++;
                    }
                }
            }
            if (cantidadDiagramas!=cantidadNodosFin)
            {
                errorList.Add("-Tenes " + cantidadDiagramas + " diagrama/s y " + cantidadNodosFin + " nodo/s de cierre. Deben coincidir.");
            }
            return errorList;
        }


        private void ValidateUseOfCorrectVariablesInTextBox(HashSet<string> errorLIst, string textBoxText, List<string> variableNames)
        {
            var regex = "[a-zA-Z0-9]+";
            var matches = Regex.Matches(textBoxText, regex);

            foreach(var match in matches.Cast<Match>().Select(match => match.Value).ToList())
            {
                int n;
                bool isNumeric = int.TryParse(match, out n);                

                if (!isNumeric && !variableNames.Contains(match) && match != "T" && match != "R")
                {
                    errorLIst.Add("-Estas utilizando una variable " + '"' + match + '"' + " no declarada.");
                }
            }
        }

        private void ValidateUseOfCorrectCharacters(HashSet<string> errorLIst, string textBoxText)
        {
            var regex = @"(?![a-zA-Z0-9\!\&\|\ \<\>\%\+\=\-\*\/\(\)]+).";
            var matches = Regex.Matches(textBoxText, regex);

            foreach (var match in matches.Cast<Match>().Select(match => match.Value).ToList())
            {
                errorLIst.Add("-Estas utilizando un caracter desconocido " + '"' + match + '"' + ".");
            }
        }

        private void ValidateUseOfValidConditionOperators(HashSet<string> errorLIst, string textBoxText)
        {
            var regex = @"[^a-zA-Z0-9\ ]+";
            var operatorsUsed = Regex.Matches(textBoxText, regex);

            foreach (var op in operatorsUsed.Cast<Match>().Select(op => op.Value).ToList())
            {
                if (op != "==" && op != "<=" && op != ">=" && op != "!=" && op != "<" && op != ">" && op != "&&" && op != "||" && op != "(" && op != ")" && op != "-" && op != "+" && op != "*" && op != "/" && op != "%")
                {
                    errorLIst.Add("-Estas utilizando un operador incorrecto " + '"' + op + '"' + " en una condición.");
                }
                if (op == "=")
                {
                    errorLIst.Add("Para hacer comparaciones utilizá " + '"' +"==" + '"' + ".");
                }
            }
        }
        #endregion

        #region Helper Methods

        private XElement LoadSerializedDataFromFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                return XElement.Load(openFile.FileName);
                //try
                //{
                //    return XElement.Load(openFile.FileName);
                //}
                //catch (Exception e)
                //{
                //    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }

            return null;
        }

        void SaveFile(XElement xElement)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    xElement.Save(saveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            List<Connector> connectors2 = new List<Connector>();
            List<Connection> connectors3 = new List<Connection>();


            foreach (DesignerItem item in designerItems)
            {
                Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                GetConnectors(cd, connectors2);
            }

            foreach (Connector connector in connectors2)
            {
                foreach (Connection con in connector.Connections)
                {

                }
            }


            XElement serializedItems = new XElement("Diagrama",
                                       new XElement("Flowchart",
                                       from item in designerItems
                                       let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                       select new XElement("DesignerItem",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("ID", item.ID),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("IsGroup", item.IsGroup),
                                                  new XElement("ParentID", item.ParentID),
                                                  new XElement("Content", contentXaml),
                                                  new XElement("Connection", connectors2)

                                              )
                                   ));
            return serializedItems;
        }

        //Nueva funcion que trae los hijos de un item.id en base a las conexiones -- RECURSIVIDAD

        private IEnumerable<DesignerItem> traeNodosHijos(Guid item1, IEnumerable<Connection> connections, List<Guid> listaAncestros)
        {
            var nodoshijos = from connection in connections where (connection.Source.ParentDesignerItem.ID == item1) select connection.Sink.ParentDesignerItem;
            listaAncestros.Add(item1);
            foreach (DesignerItem item2 in nodoshijos)
            {
                if (!listaAncestros.Contains(item2.ID)) { 
                    var nodoshijos2 = traeNodosHijos(item2.ID, connections, listaAncestros);
                    nodoshijos = nodoshijos.Union(nodoshijos2.Where(x => (x.GetAnimationBaseValue(TagProperty) ?? "").ToString() != "DIAG")) ;
               }
            }
            return (nodoshijos); 
        }

        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("SourceArrowSymbol", connection.SourceArrowSymbol),
                                      new XElement("SinkArrowSymbol", connection.SinkArrowSymbol),
                                      new XElement("zIndex", Canvas.GetZIndex(connection))
                                     )
                                  );

            return serializedConnections;
        }

        public static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
            item.Content = content;
            return item;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<DesignerItem> selectedDesignerItems =
                this.SelectionService.CurrentSelection.OfType<DesignerItem>();

            List<Connection> selectedConnections =
                this.SelectionService.CurrentSelection.OfType<Connection>().ToList();

            foreach (Connection connection in this.Children.OfType<Connection>())
            {
                if (!selectedConnections.Contains(connection))
                {
                    DesignerItem sourceItem = (from item in selectedDesignerItems
                                               where item.ID == connection.Source.ParentDesignerItem.ID
                                               select item).FirstOrDefault();

                    DesignerItem sinkItem = (from item in selectedDesignerItems
                                             where item.ID == connection.Sink.ParentDesignerItem.ID
                                             select item).FirstOrDefault();

                    if (sourceItem != null &&
                        sinkItem != null &&
                        BelongToSameGroup(sourceItem, sinkItem))
                    {
                        selectedConnections.Add(connection);
                    }
                }
            }

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            XElement connectionsXML = SerializeConnections(selectedConnections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void DeleteCurrentSelection()
        {
            foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
            {
                this.Children.Remove(connection);
            }

            foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
            {
                Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;

                List<Connector> connectors = new List<Connector>();
                GetConnectors(cd, connectors);

                foreach (Connector connector in connectors)
                {
                    foreach (Connection con in connector.Connections)
                    {
                        this.Children.Remove(con);
                    }
                }
                this.Children.Remove(item);
            }

            SelectionService.ClearSelection();
            UpdateZIndex();
        }

        private void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerItem> items)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItem item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        public Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.ID == itemID
                                         select item).FirstOrDefault();

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            IGroupable root1 = SelectionService.GetGroupRoot(item1);
            IGroupable root2 = SelectionService.GetGroupRoot(item2);

            return (root1.ID == root2.ID);
        }

        private List<XElement> SerializeVic(IEnumerable<DesignerItem> designerItems, IEnumerable<Connection> connections, List<VariableAP> variables) //antes devolvia un XElement ahora devuelve una lista de XElements
        {
            Boolean hayLoop = false;
            XElement serializedItems = null;
            XElement serializedReference = null;
            List<XElement> serializedItemsAccum = new List<XElement>();
            List<Guid> listaAncestros = new List<Guid>();
            List<Guid> listaAuxiliar = new List<Guid>();
            var refrElements = new List<DesignerItem>();

            var tagInit = CreateInitializaionDiagram(variables);
            serializedItemsAccum.Add(tagInit);

            connections = connections.OrderBy(x => x.Source.Orientation); //Para invertir validaciones cambiar por orderByDescending

            foreach (DesignerItem item in designerItems)
            {
                if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "DIAG")
                {
                    var nodoshijos = traeNodosHijos(item.ID, connections, listaAncestros);  // RECURSIVIDAD
                    foreach (var nodo in nodoshijos)
                    {
                        listaAuxiliar.Clear();
                        var nodosnietos = traeNodosHijos(nodo.ID, connections, listaAuxiliar);
                        var idsNodosNietos = nodosnietos.Select(x => x.ID);

                        if (hayLoop == false)
                        {
                            hayLoop = idsNodosNietos.Contains(nodo.ID);
                        }

                        if ((nodo.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "REFR")
                        {
                            var nombre = ((Grid)nodo.Content).Children.OfType<TextBox>().FirstOrDefault().Text;
                            var itemsWithSameName = designerItems.Where(x => ((Grid)x.Content).Children.OfType<TextBox>().FirstOrDefault().Text == nombre);
                            if (itemsWithSameName.Count() == 2)
                            {
                                var itemHuerfano = itemsWithSameName.Where(x => x.ID != nodo.ID).FirstOrDefault();                  
                                itemHuerfano.Uid = nodo.ID.ToString();
                                itemHuerfano.Tag = item.ID.ToString();
                                refrElements.Add(itemHuerfano);
                            }
                        }
                    }

                    if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "DIAG")
                    {
                        var contenido = (Grid)item.Content;
                        var listaChildren = contenido.Children;
                        var tipo = listaChildren.OfType<System.Windows.Shapes.Path>().FirstOrDefault().GetAnimationBaseValue(ToolTipProperty);
                        var textBox = listaChildren.OfType<TextBox>().FirstOrDefault();

                        serializedItems = new XElement("Diagrama", new XAttribute("Name", item.GetAnimationBaseValue(UidProperty).ToString() == "" ? textBox.Text: item.GetAnimationBaseValue(UidProperty)),
                                          new XElement("flowchart",
                                                 new XElement("block",
                                                 new XAttribute("id", item.ID),
                                                 new XAttribute("caption", ((string)tipo == "nodo_inicializador") ? "Inicializar" : textBox.Text ?? ""),
                                                 new XAttribute("type", tipo),
                                                 new XAttribute("left", Canvas.GetLeft(item)),
                                                 new XAttribute("top", Canvas.GetTop(item)),
                                                 new XAttribute("width", item.Width),
                                                 new XAttribute("height", item.Height),
                                                 new XAttribute("zIndex", Canvas.GetZIndex(item)),
                                                     from connection in connections
                                                     where connection.Source.ParentDesignerItem.ID == item.ID
                                                     select new XElement("connection",
                                                     new XAttribute("ref", connection.Sink.ParentDesignerItem.ID)
                                                                                               )
                                                              )
                                                             ,
                                                                 from item2 in nodoshijos.Concat(refrElements)
                                                                 let contenido2 = (Grid)item2.Content
                                                                 let listaChildren2 = contenido2.Children
                                                                 let textBox2 = listaChildren2.OfType<TextBox>().FirstOrDefault()
                                                                 let tipo2 = (UIElement)(listaChildren2.OfType<System.Windows.Shapes.Path>().FirstOrDefault())
                                                                 let tipo3 = (UIElement)(listaChildren2.OfType<System.Windows.Shapes.Ellipse>().FirstOrDefault())  //Definir acá los tipos de shape que precisemos
                                                                 let tipo4 = (tipo2 ?? tipo3).GetAnimationBaseValue(ToolTipProperty)
                                                                 where( !refrElements.Contains(item2) || item.ID.ToString() == item2.GetAnimationBaseValue(TagProperty).ToString())
                                                                 select new XElement("block",
                                                                        new XAttribute("id", refrElements.Contains(item2) ? "r" + item2.Uid : "" + item2.ID ),
                                                                        new XAttribute("caption", ((string)tipo4 == "nodo_iterador") ? "" : (textBox2.Text ?? "")),
                                                                        new XAttribute("type", ((string)tipo4 == "nodo_iterador") ? "nodo_sentencia" : (tipo4 ?? "")),
                                                                        new XAttribute("left", Canvas.GetLeft(item2)),
                                                                        new XAttribute("top", Canvas.GetTop(item2)),
                                                                        new XAttribute("width", item2.Width),
                                                                        new XAttribute("height", item2.Height),
                                                                        new XAttribute("zIndex", Canvas.GetZIndex(item2)),
                                                                             from connection in connections
                                                                             where (connection.Source.ParentDesignerItem.ID == item2.ID && ((string)tipo4 != "nodo_referencia" || refrElements.Contains(item2)))
                                                                             select new XElement("connection",
                                                                             new XAttribute("ref", connection.Sink.ParentDesignerItem.ID)
                                                                                               )

                                                              )))
                                                              ; 
                    }
                        if (hayLoop)
                        {
                        throw new Victoria.DesktopApp.DiagramDesigner.LoopException();
                        }
                        serializedItemsAccum.Add(serializedItems);
                }
                /*
                if ((item.GetAnimationBaseValue(TagProperty) ?? "").ToString() == "REFR") 
                {
                    var contenido = (Grid)item.Content;
                    var listaChildren = contenido.Children;
                    var nodo = listaChildren.OfType<System.Windows.Shapes.Ellipse>().FirstOrDefault();
                    var padre = (DesignerItem)nodo.Parent;
                    var tipo = nodo.GetAnimationBaseValue(ToolTipProperty);
                    var textBox = listaChildren.OfType<TextBox>().FirstOrDefault();
                    if (padre.ParentID == null)
                    {
                        serializedReference = new XElement("block",
                        new XAttribute("id", item.ID),
                        new XAttribute("caption", textBox.Text ?? ""),
                        new XAttribute("type", tipo),
                        new XAttribute("left", Canvas.GetLeft(item)),
                        new XAttribute("top", Canvas.GetTop(item)),
                        new XAttribute("width", item.Width),
                        new XAttribute("height", item.Height),
                        new XAttribute("zIndex", Canvas.GetZIndex(item)),
                            from connection in connections
                            where connection.Source.ParentDesignerItem.ID == item.ID
                            select new XElement("connection",
                            new XAttribute("ref", connection.Sink.ParentDesignerItem.ID)
                                                                      )
                                     );
                    }

                }
                serializedItemsAccum.Add(serializedReference);  */
            }
            serializedItemsAccum.Add(VariablesTag);
            return serializedItemsAccum;
        }

        private XElement CreateInitializaionDiagram(List<VariableAP> variables)
        {
            int i = 1;
           // int f = variables.Where(v => v.vector == false).Count();        
            var vectores = (from variable in variables
                            where (variable.vector == true)
                            select variable);
            int cantVec = vectores.Count();
            return new XElement("Diagrama", new XAttribute("Name", "Inicializar"),
                                          new XElement("flowchart",
                                                new XElement("block",
                                                new XAttribute("id", "Inicializar0"),
                                                new XAttribute("caption", "Inicializar"),
                                                new XAttribute("type", "nodo_titulo_inicializador"),
                                                        new XElement("connection",
                                                        new XAttribute("ref", "Inicializar1")
                                                         )),
                                            from variable in variables
                                            select new XElement("block",
                                                new XAttribute("id", "Inicializar" + (i++).ToString()),
                                                new XAttribute("caption", variable.nombre + "=" + variable.valor),
                                                new XAttribute("type", "nodo_sentencia"),
                                                        //new XAttribute("left", "600"),
                                                        //new XAttribute("top", "200"),
                                                        //new XAttribute("width", "123"),
                                                        //new XAttribute("height","123"),
                                                        //new XAttribute("zIndex", "123"),         
                                                        new XElement("connection",
                                                        new XAttribute("ref", "Inicializar" + (i).ToString()),
                                                        new XAttribute("left", ((variable.vector == true) ? setitovector(variable, (i++)) : (i).ToString()))
                                                                        )
                                        ),
                                           from variable in variables
                                           where (variable.vector == true)
                                           select new XElement("block",
                                                new XAttribute("id", "Inicializar" + variable.i.ToString()),
                                                new XAttribute("caption", "1;10;1;I"),
                                                new XAttribute("type", "nodo_iterador"),
                                               new XElement("connection",
                                                new XAttribute("ref", "Inicializar" + ((variable.i) - 1).ToString())),
                                               new XElement("connection",
                                                new XAttribute("ref", "Inicializar" + (++variable.i).ToString())
                                                                       )

                                                ),

                                                new XElement("block",
                                                new XAttribute("id", "Inicializar" + (variables.Count() + cantVec + 1 ).ToString()),
                                                new XAttribute("caption", ""),
                                                new XAttribute("type", "nodo_fin")
                                        )))

                                        ;
        }

        public string setitovector(VariableAP variable, double i)
        {
            variable.i = i;
            return ("");
        }

        #endregion
    }
}
