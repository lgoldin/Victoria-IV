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

namespace Victoria.WebApp
{
    public partial class LineChartOxyPlot : UserControl
    {
        public LineChartOxyPlot()
        {
            CompositionTarget.Rendering += CompositionTargetRendering;
            InitializeComponent();
        }


        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.DataContext is ChartViewModel)
                ((ChartViewModel)this.DataContext).Update();
            else
                CompositionTarget.Rendering -= CompositionTargetRendering;
        }
    }
}
