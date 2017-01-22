using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Victoria.ViewModelWPF;


namespace Victoria.ModelWPF
{
    public class AnimationConfigurationReservoir : AnimationConfigurationBase
    {
        public string NrVariable { get; set; }
        public string SelectedHVVariable { get; set; }

        #region Constructor
        public AnimationConfigurationReservoir(List<Variable> variables) : base(variables)
        {
            base.AnimationType = "Reservorio";

            base.DllExtraConfigurations = new List<StackPanel>(); //esto va en el base

            var nrVariableLabel = new Label();
            nrVariableLabel.Content = "Variable de nivel de reservorio:";

            var nrVariableCombo = new ComboBox();
            nrVariableCombo.Name = "nrVariableCombo";
            List<string> nrVariableOptions = new List<string>();
            foreach (var variable in variables)
            {
                nrVariableOptions.Add(variable.Name);
            }

            nrVariableCombo.ItemsSource = nrVariableOptions;
            nrVariableCombo.SelectedIndex = 0;

            var maxValueVariableLabel = new Label();
            maxValueVariableLabel.Content = "Variable de valor máximo reservorio:";

            var variablesForMaxValueCombo = new ComboBox();
            variablesForMaxValueCombo.Name = "maxValueCombo";
            variablesForMaxValueCombo.ItemsSource = base.Variables.Select(x => x.Name);
            variablesForMaxValueCombo.SelectedIndex = 0;

            var maxValueStackPanel = new StackPanel();

            maxValueStackPanel.Name = "maxValueStackPanel";
            maxValueStackPanel.Children.Add(nrVariableLabel);
            maxValueStackPanel.Children.Add(nrVariableCombo);
            maxValueStackPanel.Children.Add(maxValueVariableLabel);
            maxValueStackPanel.Children.Add(variablesForMaxValueCombo);

            DllExtraConfigurations.Add(maxValueStackPanel);
        }
        #endregion

        public override void BindProperties()
        {
            var stackChildrens = DllExtraConfigurations.Where(x => x.Name == "maxValueStackPanel").First().Children;

            foreach (var s in stackChildrens)
            {
                if (s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "maxValueCombo")
                {
                    ComboBox maxValueCombo = (ComboBox)s;
                    SelectedHVVariable = (string)maxValueCombo.SelectedValue;
                }
                if (s.GetType() == typeof(ComboBox) && ((ComboBox)s).Name == "nrVariableCombo")
                {
                    ComboBox nrVariablesCombo = (ComboBox)s;
                    NrVariable = (string)nrVariablesCombo.SelectedValue;
                }
            }

        }
    }

    public class AnimationReservoirViewModel : AnimationViewModel
    {
        //ESTA PROPIEDAD LA USO PARA ENCONTRAR EN VICTORIA QUE ANIMATIONVIEWMODEL TENGO QUE CREAR.
        public override string ConfigurationType { get; set; } = "AnimationConfigurationReservoir";

        public double NrActualValue { get; set; }

        public List<double> NrValuesList { get; set; }

        public double MaxValue { get; set; }

        public AnimationReservoirViewModel()
        {

        }

        public override void BindSimulationVariableValues()
        {
            AnimationConfigurationReservoir animationConfigReservoir = (AnimationConfigurationReservoir)this.AnimationConfig;

            this.NrValuesList = AnimationConfig.Variables.Where(n => n.Name == animationConfigReservoir.NrVariable).First().ValuesEnumerable.ToList();
            this.MaxValue = animationConfigReservoir.Variables.Where(n => n.Name == animationConfigReservoir.SelectedHVVariable).First().ActualValue;
        }

        public override void InitializeAnimation(AnimationConfigurationBase animationConfig)
        {
            base.InitializeAnimation(animationConfig);

            AnimationConfigurationReservoir animationConfigQueue = (AnimationConfigurationReservoir)animationConfig;
            this.VariableToAnimateName = "Nivel reservorio: ";

            this.AnimationElementsList = new ObservableCollection<Shape>();      
            var r = new Rectangle();
            r.MaxHeight = 100;
            r.Height = 100;
            r.Width = 100;
            r.Fill = new SolidColorBrush(Colors.Blue);
            this.AnimationElementsList.Add(r);            
        }

        public override void DoAnimation(int index)
        {
            this.VariableToAnimateValue = this.NrValuesList[index].ToString();
            this.NrActualValue = this.NrValuesList[index];

            if (this.NrActualValue < 0)
            {
                this.NrActualValue = 0;
            }

            if (this.AnimationElementsList != null && this.AnimationElementsList.Any())
            {
                Rectangle reservoir = (Rectangle)AnimationElementsList[0];
                var waterColor = new Color();

                var percentOfReservoir = NrActualValue * reservoir.MaxHeight / this.MaxValue;
                if (percentOfReservoir > 100)
                {
                    waterColor = Colors.Red;
                    percentOfReservoir = 100;
                }
                else
                {
                    waterColor = Colors.Blue;
                }

                double percentage = percentOfReservoir / 100;
                LinearGradientBrush waterEffect = new LinearGradientBrush();
                waterEffect.StartPoint = new Point(1, 1);
                waterEffect.EndPoint = new Point(1, 0);
                waterEffect.GradientStops.Add(new GradientStop(waterColor, 0));
                waterEffect.GradientStops.Add(new GradientStop(waterColor, percentage));
                waterEffect.GradientStops.Add(new GradientStop(Colors.Transparent, percentage));

                reservoir.Fill = waterEffect;
            }
        }

    }
}
