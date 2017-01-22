using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Victoria.ViewModelWPF;

namespace Victoria.ModelWPF
{
    public class AnimationConfigurationSemaphore : AnimationConfigurationBase
    {
        public string SelectedConditionVariable { get; set; }
        public int SelectedConditionValueForGreen { get; set; }
        public int SelectedConditionValueForYellow { get; set; }

        #region Constructor
        public AnimationConfigurationSemaphore(List<Variable> variables) : base(variables)
        {
            base.AnimationType = "Semáforo";

            base.DllExtraConfigurations = new List<StackPanel>(); 

            addComboExtraConf("Variable de condición de semáforo:", "VarCondition", base.Variables.Select(x => x.Name));

            var greenValueLabel = new Label();
            greenValueLabel.Content = "Valor tope para verde: ";
            var greenValueTextBox = new TextBox();
            greenValueTextBox.PreviewTextInput += OnlyNumbers;
            var greenValueStackPanel = new StackPanel();
            greenValueStackPanel.Name = "GreenValStack";
            greenValueStackPanel.Children.Add(greenValueLabel);
            greenValueStackPanel.Children.Add(greenValueTextBox);
            DllExtraConfigurations.Add(greenValueStackPanel);

            var yellowValueLabel = new Label();
            yellowValueLabel.Content = "Valor tope para amarillo: ";
            var yellowValueTextBox = new TextBox();
            yellowValueTextBox.PreviewTextInput += OnlyNumbers;
            var yellowValueStackPanel = new StackPanel();
            yellowValueStackPanel.Name = "YellowValStack";
            yellowValueStackPanel.Children.Add(yellowValueLabel);
            yellowValueStackPanel.Children.Add(yellowValueTextBox);
            DllExtraConfigurations.Add(yellowValueStackPanel);
        }

        private void addComboExtraConf(string label, string variableName, IEnumerable<string> items)
        {
            var Label = new Label();
            Label.Content = label;

            var variablesForCombo = new ComboBox();
            variablesForCombo.ItemsSource = items;
            variablesForCombo.SelectedIndex = 0;

            var StackPanel = new StackPanel();

            StackPanel.Name = variableName;
            StackPanel.Children.Add(Label);
            StackPanel.Children.Add(variablesForCombo);

            DllExtraConfigurations.Add(StackPanel);
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+$");
            return !regex.IsMatch(text);
        }

        #endregion

        public override void BindProperties()
        {
            var stackChildrensVarCondition = DllExtraConfigurations.Where(x => x.Name == "VarCondition").First().Children;
            foreach (var s in stackChildrensVarCondition)
            {
                if (s.GetType() == typeof(ComboBox))
                {
                    ComboBox variablesCombo = (ComboBox)s;
                    SelectedConditionVariable = (string)variablesCombo.SelectedValue;
                }
            }

            var stackChildrensGreenCondVal = DllExtraConfigurations.Where(x => x.Name == "GreenValStack").First().Children;
            foreach (var s in stackChildrensGreenCondVal)
            {
                if (s.GetType() == typeof(TextBox))
                {
                    TextBox valTextBox = (TextBox)s;

                    int greenVal;

                    if (int.TryParse(valTextBox.Text.Trim(), out greenVal))
                    {
                        SelectedConditionValueForGreen = greenVal;
                    }
                    else
                    {
                        SelectedConditionValueForGreen = 0;
                    }
                }
            }

            var stackChildrensYellowCondVal = DllExtraConfigurations.Where(x => x.Name == "YellowValStack").First().Children;
            foreach (var s in stackChildrensYellowCondVal)
            {
                if (s.GetType() == typeof(TextBox))
                {
                    TextBox valTextBox = (TextBox)s;

                    int yellowVal;

                    if (int.TryParse(valTextBox.Text.Trim(), out yellowVal))
                    {
                        SelectedConditionValueForYellow = yellowVal;
                    }
                    else
                    {
                        SelectedConditionValueForYellow = 0;
                    }
                }
            }
        }
    }

    public class AnimationSemaphoreViewModel : AnimationViewModel
    {
        //ESTA PROPIEDAD LA USO PARA ENCONTRAR EN VICTORIA QUE ANIMATIONVIEWMODEL TENGO QUE CREAR.
        public override string ConfigurationType { get; set; } = "AnimationConfigurationSemaphore";

        public double SelectedVarActualValue { get; set; }

        public List<double> SelectedVarValuesList { get; set; }

        public int GreenConditionValue;

        public int YellowConditionValue;

        public AnimationSemaphoreViewModel()
        {

        }

        public override void BindSimulationVariableValues()
        {
            AnimationConfigurationSemaphore animationConfigSem = (AnimationConfigurationSemaphore)this.AnimationConfig;
            this.SelectedVarValuesList = animationConfigSem.Variables.Where(n => n.Name == animationConfigSem.SelectedConditionVariable).First().ValuesEnumerable.ToList();
            this.GreenConditionValue = animationConfigSem.SelectedConditionValueForGreen;
            this.YellowConditionValue = animationConfigSem.SelectedConditionValueForYellow;
        }

        private void GreenSempahore()
        {
            this.AnimationElementsList[0].Fill = new SolidColorBrush(Colors.Green);
        }

        private void YellowSemaphore()
        {
            this.AnimationElementsList[0].Fill = new SolidColorBrush(Colors.Yellow);
        }

        private void RedSemaphore()
        {
            this.AnimationElementsList[0].Fill = new SolidColorBrush(Colors.Red);
        }

        public override void InitializeAnimation(AnimationConfigurationBase animationConfig)
        {
            base.InitializeAnimation(animationConfig);

            this.AnimationElementsList = new ObservableCollection<Shape>();

            var e = new Ellipse();
            e.Height = 20;
            e.Width = 20;
            e.Fill = new SolidColorBrush(Colors.White);
            e.Margin = new Thickness(10.0, 5.0, 0, 0);

            this.AnimationElementsList.Add(e);
        }

        public override void DoAnimation(int index)
        {
            this.SelectedVarActualValue = this.SelectedVarValuesList[index];

            if (GreenCondition(SelectedVarActualValue))
            {
                GreenSempahore();
            }
            else if(YellowCondition(SelectedVarActualValue))
            {
                YellowSemaphore();
            }
            else 
            {
                RedSemaphore();
            }
        }

        private bool GreenCondition(double actualValue)
        {
            return ActualValueLessOrEqualThanCondition(actualValue, this.GreenConditionValue);
        }

        private bool YellowCondition(double actualValue)
        {
            return ActualValueLessOrEqualThanCondition(actualValue, this.YellowConditionValue);
        }

        private bool ActualValueLessOrEqualThanCondition(double actualValue, int conditionValue)
        {
            return actualValue <= conditionValue;
        }
    }

}
