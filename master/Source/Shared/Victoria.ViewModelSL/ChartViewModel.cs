using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Victoria.Shared;
namespace Victoria.ViewModelSL
{
    public class ChartViewModel : ChartViewModelBase
    {
        #region Fields

        private LinearAxis horizontalAxis;
        private LinearAxis verticalAxis;
        private bool HasNewData = false;

        #endregion

        #region Properties

        public PlotModel PlotModel { get; private set; }

        #endregion

        public ChartViewModel(string name, Variable independentVariable, List<Variable> dependentVariables)
            : base(name, independentVariable, dependentVariables)
        {
            PlotModel = new PlotModel();
            PlotModel.Title = name;
            this.verticalAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 100,
                Title = "Unidades"
            };
            PlotModel.Axes.Add(verticalAxis);
            this.horizontalAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 100,
                Title = "Tiempo"
            }; PlotModel.LegendTitle = "Referencias";
            PlotModel.LegendOrientation = LegendOrientation.Vertical;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.RightMiddle;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            foreach (var dependentVariable in dependentVariables)
            {
                PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Title = dependentVariable.Name });
            }
            this.IsVisible = true;
            this.RaisePropertyChanged("PlotModel");
        }


        protected override void DoAddPoint()
        {
            lock (PlotModel.SyncRoot)
            {
                if (!this.PlotModel.Series.Any(s => ((DataPointSeries)s).Points.Any()))
                {
                    PlotModel.Axes[1].Minimum = independentVariable.ActualValue;
                }

                if (PlotModel.Axes[1].Minimum > independentVariable.ActualValue)
                    PlotModel.Axes[1].Minimum = independentVariable.ActualValue;
                if (PlotModel.Axes[1].Maximum < independentVariable.ActualValue)
                    PlotModel.Axes[1].Maximum = independentVariable.ActualValue;

                for (int i = 0; i < dependentVariables.Count; i++)
                {
                    var s = (LineSeries)PlotModel.Series[i];
                    s.Points.Add(new DataPoint(independentVariable.ActualValue, dependentVariables[i].ActualValue));

                    if (PlotModel.Axes[0].Minimum > dependentVariables[i].ActualValue - 1)
                        PlotModel.Axes[0].Minimum = dependentVariables[i].ActualValue - 1;
                    if (PlotModel.Axes[0].Maximum < dependentVariables[i].ActualValue + 1)
                        PlotModel.Axes[0].Maximum = dependentVariables[i].ActualValue + 1;
                }
                HasNewData = true;
            }
        }

        public void Update()
        {
            if (HasNewData)
            {
                lock (PlotModel.SyncRoot)
                {
                    this.PlotModel.InvalidatePlot(true);
                    HasNewData = false;
                }
            }
        }

        public override void Reset()
        {
            foreach (DataPointSeries serie in this.PlotModel.Series)
            {
                serie.Points.Clear();
            }
            this.horizontalAxis.Maximum = this.horizontalAxis.Minimum;

            this.PlotModel.ResetAllAxes();
            this.PlotModel.InvalidatePlot(true);
        }
    }
}


