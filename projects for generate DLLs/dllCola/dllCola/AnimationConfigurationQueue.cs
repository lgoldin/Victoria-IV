using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Victoria.ViewModelWPF;

namespace Victoria.ModelWPF
{
    public class AnimationConfigurationQueue : AnimationConfigurationBase
    {
        public int SelectedNsLimit { get; set; }
        public int SelectedNPosts { get; set; }
        public string NsVariable { get; set; }

        public string SelectedDirection { get; set; }

        #region Constructor
        public AnimationConfigurationQueue(List<Variable> variables):base(variables)
        {
            base.AnimationType = "Cola";

            base.DllExtraConfigurations = new List<StackPanel>(); //esto va en el base

            var nsVariableLabel = new Label();
            nsVariableLabel.Content = "Variable de cantidad de personas:";

            var nsVariableCombo = new ComboBox();
            nsVariableCombo.Name = "nsVariableCombo";
            List<string> nsVariableOptions = new List<string>();
            foreach(var variable in variables)
            {
                nsVariableOptions.Add(variable.Name);
            }

            nsVariableCombo.ItemsSource = nsVariableOptions;
            nsVariableCombo.SelectedIndex = 0;

            var nsLimitLabel = new Label();
            nsLimitLabel.Content = "Limite de personas en cola a mostrar:";

            var nsLimitCombo = new ComboBox();
            nsLimitCombo.Name = "nsLimitCombo";
            List<int> nsLimitOptions = new List<int>();
            for (int i = 1; i <= 10; i++)
            {
                nsLimitOptions.Add(i);
            }

            nsLimitCombo.ItemsSource = nsLimitOptions;
            nsLimitCombo.SelectedIndex = 0;

            var nPostLabel = new Label();
            nPostLabel.Content = "Cantidad de personas a restar de la cola:";

            var nPostCombo = new ComboBox();
            nPostCombo.Name = "nPostCombo";

            nPostCombo.ItemsSource = nsLimitOptions;
            nPostCombo.SelectedIndex = 0;

            var directionLabel = new Label();
            directionLabel.Content = "Dirección de cola (según orientación)";

            var directionsCombo = new ComboBox();
            directionsCombo.Name = "directionsCombo";
            List<string> directionsOptions = new List<string>();
            directionsOptions.Add("Izquierda a Derecha/Arriba a Abajo");
            directionsOptions.Add("Derecha a Izquierda/Abajo a Arriba");

            directionsCombo.ItemsSource = directionsOptions;
            directionsCombo.SelectedIndex = 0;

            var nsLimitStackPanel = new StackPanel();

            nsLimitStackPanel.Name = "extraConfigsQueue";
            nsLimitStackPanel.Children.Add(nsVariableLabel);
            nsLimitStackPanel.Children.Add(nsVariableCombo);
            nsLimitStackPanel.Children.Add(nsLimitLabel);
            nsLimitStackPanel.Children.Add(nsLimitCombo);
            nsLimitStackPanel.Children.Add(nPostLabel);
            nsLimitStackPanel.Children.Add(nPostCombo);
            nsLimitStackPanel.Children.Add(directionLabel);
            nsLimitStackPanel.Children.Add(directionsCombo);

            DllExtraConfigurations.Add(nsLimitStackPanel);
        }
        #endregion

        public override void BindProperties()
        {
            //Esto lo usamos para bindear las propiedades propias de la config. En este caso seria el valor seleccionado de nslimit a partir del combo
            var stackChildrens = DllExtraConfigurations.Where(x => x.Name == "extraConfigsQueue").First().Children;

            foreach(var s in stackChildrens)
            {
                if(s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "nsLimitCombo")
                {
                    ComboBox nsLimitCombo = (ComboBox)s;
                    SelectedNsLimit = (int)nsLimitCombo.SelectedValue;
                }
                else if (s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "nsVariableCombo")
                {
                    ComboBox nsVariablesCombos = (ComboBox)s;
                    NsVariable = (string)nsVariablesCombos.SelectedValue;
                }
                else if (s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "nPostCombo")
                {
                    ComboBox nPostCombos = (ComboBox)s;
                    SelectedNPosts = (int)nPostCombos.SelectedValue;
                }
                else if (s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "directionsCombo")
                {
                    ComboBox directionsCombo = (ComboBox)s;
                    SelectedDirection = (string)directionsCombo.SelectedValue;
                }
            }

        }
    }

    public class AnimationQueueViewModel : AnimationViewModel
    {
        public int NsLimit { get; set; }

        //ESTA PROPIEDAD LA USO PARA ENCONTRAR EN VICTORIA QUE ANIMATIONVIEWMODEL TENGO QUE CREAR.
        public override string ConfigurationType { get; set; } = "AnimationConfigurationQueue";

        public double NsActualValue { get; set; }

        public List<double> NsValuesList { get; set; }

        public int NPostsValue { get; set; }

        public string Direction { get; set; }

        public AnimationQueueViewModel()
        {

        }

        public override void BindSimulationVariableValues()
        {
            this.NsValuesList = AnimationConfig.Variables.Where(n => n.Name == ((AnimationConfigurationQueue)AnimationConfig).NsVariable).First().ValuesEnumerable.ToList();
        }

        public override void InitializeAnimation(AnimationConfigurationBase animationConfig)
        {
            base.InitializeAnimation(animationConfig);

            AnimationConfigurationQueue animationConfigQueue = (AnimationConfigurationQueue)animationConfig;
            this.NsLimit = animationConfigQueue.SelectedNsLimit;
            this.NPostsValue = animationConfigQueue.SelectedNPosts;
            this.VariableToAnimateName = "Personas: ";
            this.Direction = animationConfigQueue.SelectedDirection;

            this.AnimationElementsList = new ObservableCollection<Shape>();
            for (int i = 1; i <= NsLimit; i++)
            {
                var a = new Ellipse();
                a.Height = 10;
                a.Width = 10;
                a.Fill = new SolidColorBrush(Colors.White);
                this.AnimationElementsList.Add(a);
            }
        }

        public override void DoAnimation(int index)
        {
            this.VariableToAnimateValue = this.NsValuesList[index].ToString();
            this.NsActualValue = this.NsValuesList[index];

            if (this.AnimationElementsList != null && this.AnimationElementsList.Any())
            {
                foreach (var elipse in this.AnimationElementsList)
                {
                    elipse.Fill = new SolidColorBrush(Colors.White);
                }

                var QueueRealValue = this.NsActualValue - this.NPostsValue;
                if(QueueRealValue < 0)
                {
                    QueueRealValue = 0;
                }

                if (QueueRealValue <= this.NsLimit)
                {
                    if(this.Direction == "Izquierda a Derecha/Arriba a Abajo")
                    {
                        for (int i = 0; i < QueueRealValue; i++)
                        {
                            if (this.AnimationElementsList[i] != null)
                            {
                                this.AnimationElementsList[i].Fill = new SolidColorBrush(Colors.Black);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = this.NsLimit - 1; i > this.NsLimit - 1 - QueueRealValue; i--)
                        {
                            if (this.AnimationElementsList[i] != null)
                            {
                                this.AnimationElementsList[i].Fill = new SolidColorBrush(Colors.Black);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.NsLimit; i++)
                    {
                        if (this.AnimationElementsList[i] != null)
                        {
                            this.AnimationElementsList[i].Fill = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            break;
                        }
                    }                 
                }
            }
        }

    }

}
