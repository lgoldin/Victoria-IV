using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Practices.Prism.Commands;
using Victoria.Shared;
using Victoria.Shared.EventArgs;
using Variable = Victoria.ModelSL.Variable;

namespace Victoria.ViewModelSL
{
    public class StageViewModel : StageViewModelBase
    {
        #region Properties

        #endregion

        #region Constructor

        public StageViewModel(Simulation simulation)
        {
            this.Simulation = simulation;
            this.Simulation.SimulationStatusChanged += SimulationOnSimulationStatusChanged;
            Initialize();
        }
        #endregion

        #region Methods

        private void SimulationOnSimulationStatusChanged(object sender, SimulationStatusChangedEventArgs simulationStatusChangedEventArgs)
        {
            switch (simulationStatusChangedEventArgs.Status)
            {
                case SimulationStatus.Stoped:
                    this.Executing = false;
                    break;
                case SimulationStatus.Started:
                    this.Executing = true;
                    break;
            }
        }

        protected override void Initialize()
        {
            this.Charts = new ObservableCollection<ChartViewModelBase>();
            this.Variables = new ObservableCollection<ModelSL.Variable>();
            foreach (var variable in this.Simulation.Variables)
            {
                this.Variables.Add(new Variable(variable));
            }
            this.addChartCommand = new DelegateCommand(this.AddChart);
            this.deleteChartCommand = new DelegateCommand(this.DeleteChart);
            this.exportStageCommand = new DelegateCommand(this.ExportStage);
            this.executeStageCommand = new DelegateCommand(this.ExecuteStage);
            this.stopExecutionStageCommand = new DelegateCommand(this.StopExecution);

        }



        private void AddChart(object parameters)
        {
            var name = ((List<object>)parameters)[0] != null ? ((List<object>)parameters)[0].ToString() : "Grafico";
            List<Variable> selectedVariables = (List<Variable>)((List<object>)parameters)[1];
            var variableTime = ((List<Variable>)selectedVariables).First(v => v.Name == "T").VariableComponent;
            var dependentVariables =
                (from variable in ((List<Variable>)selectedVariables)
                 where variable.Name != "T"
                 select variable.VariableComponent).ToList();

            this.Charts.Add(new ChartViewModel(name, variableTime, dependentVariables));
        }

        private void DeleteChart()
        {
            if (SelectedChart != null && Charts.Contains(SelectedChart))
            {
                this.Charts.Remove(SelectedChart);
                SelectedChart = null;
            }
        }

        private void ExportStage(  )
        {
         
        }

        private void ExecuteStage()
        {
            try
            {
                if (!Executing)
                {
                    foreach (var chart in Charts)
                    {
                        chart.Reset();
                    }
                    BackgroundWorker.RunInBackGround(delegate { this.Simulation.Execute(); });
                }
            }
            catch (Exception)
            {

                throw;
            }


        }


        private void StopExecution()
        {
            try
            {
                if (Executing)
                {
                    BackgroundWorker.RunInBackGround(delegate { this.Simulation.StopExecution(); });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
