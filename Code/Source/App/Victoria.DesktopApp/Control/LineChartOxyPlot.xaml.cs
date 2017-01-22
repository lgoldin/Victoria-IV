using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OxyPlot;
using Victoria.DesktopApp.View;
using Victoria.Shared;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp.Control
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
