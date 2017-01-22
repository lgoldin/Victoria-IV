using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Victoria.ViewModelSL;

namespace Victoria.WebApp.View
{
    public partial class SimulationControl : UserControl
    {
        public SimulationControl()
        {
            InitializeComponent();
        }

        private void BtnAddChart_OnClick(object sender, RoutedEventArgs e)
        {
            List<object> parameters = new List<object>();

            var addChartPopUp = new AddChartChildWindow();
            foreach (var variable in ((StageViewModel)this.DataContext).Variables)
            {
                variable.IsSelected = false;
            }
            addChartPopUp.DataContext = this.DataContext;
            addChartPopUp.Closed += new EventHandler((object o, EventArgs e1) =>
            {
                if (addChartPopUp.DialogResult.HasValue && addChartPopUp.DialogResult.Value)
                {
                    parameters.Add(addChartPopUp.ChartName);
                    parameters.Add(((StageViewModel)this.DataContext).Variables.Where(v => v.IsSelected).ToList());
                    ((StageViewModel)this.DataContext).AddChartCommand.Execute(parameters);
                }
            });
            addChartPopUp.Show();
        }

    }
}
