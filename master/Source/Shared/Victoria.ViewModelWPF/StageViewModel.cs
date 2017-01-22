using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Victoria.ModelWPF;
using Victoria.Shared;
using Victoria.Shared.EventArgs;
using Variable = Victoria.ModelWPF.Variable;

namespace Victoria.ViewModelWPF
{
    public class StageViewModel : StageViewModelBase    
    {
        #region Fields
        protected new DelegateCommand addChartCommand;
        protected new DelegateCommand deleteChartCommand;
        protected new DelegateCommand executeStageCommand;
        protected new DelegateCommand stopExecutionStageCommand;
        protected new DelegateCommand exportStageCommand;
        protected new DelegateCommand addAnimationToCanvasCommand;
        private ObservableCollection<ModelWPF.Variable> variables;
        private ObservableCollection<ChartViewModelBase> charts;
        private ObservableCollection<AnimationViewModel> animations;

        private ChartViewModelBase selectedChart;

        private List<AnimationConfigurationBase> dllConfigurations;
        private List<AnimationViewModel> dllAnimations;

        #endregion

        #region Properties

        public List<AnimationConfigurationBase> DllConfigurations 
        {
            get
            {
                if (this.dllConfigurations == null)
                {
                    this.dllConfigurations = new List<AnimationConfigurationBase>();
                }

                return this.dllConfigurations;
            }

            set
            {
                this.dllConfigurations = value;
            }
        }

        public List<AnimationViewModel> DllAnimations 
        { 
            get
            {
                if (this.dllAnimations == null)
                {
                    this.dllAnimations = new List<AnimationViewModel>();
                }

                return this.dllAnimations;
            }

            set 
            {
                this.dllAnimations = value;
            }
        }

        public new ObservableCollection<ModelWPF.Variable> Variables
        {
            get { return variables; }
            set
            {
                variables = value;
                variables.CollectionChanged += VariablesCollectionChanged;

                this.RaisePropertyChanged("Variables");
            }
        }

        public new List<Variable> FilteredVariables
        {
            get
            {
                return string.IsNullOrEmpty(FilterText) ? this.Variables.ToList() : variables.Where(v => v.Name.ToUpper().Contains(FilterText.ToUpper())).ToList();
            }
        }

        public new ObservableCollection<ChartViewModelBase> Charts
        {
            get { return charts; }
            set
            {
                charts = value;
                this.RaisePropertyChanged("Charts");
            }
        }

        public ObservableCollection<AnimationViewModel> Animations
        {
            get { return animations; }
            set
            {
                animations = value;
                this.RaisePropertyChanged("Animations");
            }
        }


        public new ChartViewModelBase SelectedChart
        {
            get { return selectedChart; }
            set
            {
                selectedChart = value;
                this.RaisePropertyChanged("SelectedCharts");
            }
        }


        #endregion

        #region Commands

        /// <summary>
        /// Gets OpenSimulationCommand.
        /// </summary>
        public new ICommand AddChartCommand
        {
            get
            {
                return this.addChartCommand;
            }
        }

        /// <summary>
        /// Gets SaveSimulationCommand.
        /// </summary>
        public new ICommand DeleteChartCommand
        {
            get
            {
                return this.deleteChartCommand;
            }
        }

        /// <summary>
        /// Gets ExecuteSimulationCommand.
        /// </summary>
        public new ICommand ExecuteStageCommand
        {
            get
            {
                return this.executeStageCommand;
            }
        }

        /// <summary>
        /// Gets StopExecutionSimulationCommand.
        /// </summary>
        public new ICommand StopExecutionStageCommand
        {
            get
            {
                return this.stopExecutionStageCommand;
            }
        }

        /// <summary>
        /// Gets ExportStageCommand.
        /// </summary>
        public new ICommand ExportStageCommand
        {
            get
            {
                return this.exportStageCommand;
            }
        }

        public new ICommand AddAnimationToCanvasCommand
        {
            get
            {
                return this.addAnimationToCanvasCommand;
            }
        }

        #endregion

        public StageViewModel(Simulation simulation)
        {
            this.Simulation = simulation;
            this.Simulation.SimulationStatusChanged += SimulationOnSimulationStatusChanged;
            Initialize();
        }

        public StageViewModel(Simulation simulation, Stage stage)
        {
            this.Simulation = simulation;
            this.Simulation.SimulationStatusChanged += SimulationOnSimulationStatusChanged;
            Initialize(stage.Name, stage.Charts, stage.Variables);
        }

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

        #region Methods

        protected void Initialize(string name, List<Chart> charts, List<Shared.Variable> variables)
        {
            this.Charts = new ObservableCollection<ChartViewModelBase>();    
            this.Animations = new ObservableCollection<AnimationViewModel>();


            foreach (var chart in charts)
            {
                var dependentVariables = this.Simulation.Variables.Where(v => chart.DependentVariables.Any(dv=>dv == v.Name)).ToList();
                var chartToAdd = new ChartViewModel(chart.Name, this.Simulation.Variables.First(v=>v.Name == "T"), dependentVariables);
                this.Charts.Add(chartToAdd);
            }
            this.Variables = new ObservableCollection<ModelWPF.Variable>();

            foreach (var variable in this.Simulation.Variables)
            {
                if (variable is VariableArray)
                {
                    foreach (var variableItem in ((VariableArray)variable).Variables)
                    {
                        var variableToAdd = new Variable(variableItem);
                        variableToAdd.InitialValue = variables.First(v => v.Name == variableItem.Name).InitialValue;
                        this.Variables.Add(variableToAdd);

                    }
                }
                else
                {
                    var variableToAdd = new Variable(variable);
                    variableToAdd.InitialValue = variables.First(v => v.Name == variable.Name).InitialValue;
                    this.Variables.Add(variableToAdd);
                }
            }

            Load_DLLs();

            this.addChartCommand = new DelegateCommand(this.AddChart);
            this.deleteChartCommand = new DelegateCommand(this.DeleteChart);
            this.exportStageCommand = new DelegateCommand(this.ExportStage);
            this.executeStageCommand = new DelegateCommand(this.ExecuteStage);
            this.stopExecutionStageCommand = new DelegateCommand(this.StopExecution);
            this.addAnimationToCanvasCommand = new DelegateCommand(this.AddAnimationToCanvas);
        }


        protected override void Initialize()
        {
            this.Charts = new ObservableCollection<ChartViewModelBase>();
            this.Variables = new ObservableCollection<ModelWPF.Variable>();
            this.Animations = new ObservableCollection<AnimationViewModel>();

            foreach (var variable in this.Simulation.Variables)
            {
                if (variable is VariableArray)
                {
                    foreach (var variableItem in ((VariableArray)variable).Variables)
                    {
                        this.Variables.Add(new Variable(variableItem));
                    }
                }
                else
                {
                    this.Variables.Add(new Variable(variable));
                }
            }

            Load_DLLs();

            this.addChartCommand = new DelegateCommand(this.AddChart);
            this.deleteChartCommand = new DelegateCommand(this.DeleteChart);
            this.exportStageCommand = new DelegateCommand(this.ExportStage);
            this.executeStageCommand = new DelegateCommand(this.ExecuteStage);
            this.stopExecutionStageCommand = new DelegateCommand(this.StopExecution);
            this.addAnimationToCanvasCommand = new DelegateCommand(this.AddAnimationToCanvas);
        }

        private void AddChart(object parameters)
        {
            var name = ((List<object>)parameters)[0] != null ? ((List<object>)parameters)[0].ToString() : "Gráfico";
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

        private void ExportStage(object simulacionFileName)
        {
            List<StageViewModelBase> stages = new List<StageViewModelBase>();
            stages.Add(this);
            if (simulacionFileName.ToString().Substring(simulacionFileName.ToString().Length - 4, 4).Equals("xlsx"))
                HelperExport.HelperExport.ExportStagesToExcel(stages, simulacionFileName.ToString());
            else if (simulacionFileName.ToString().Substring(simulacionFileName.ToString().Length - 3, 3).Equals("pdf"))
                HelperExport.HelperExport.ExportStagesToPDF(stages, simulacionFileName.ToString());
            else
                throw new Exception("Extensión no compatible");
        }

        private void AddAnimationToCanvas(object parameters)
        {
            List<object> parametersDefined = (List<object>)parameters;

            var animation = this.CreateAnimation((AnimationConfigurationBase)parametersDefined[0], (List<AnimationViewModel>)parametersDefined[1]);

            this.Animations.Add(animation);    
        }
        private AnimationViewModel CreateAnimation(AnimationConfigurationBase animationConfig, List<AnimationViewModel> DllAnimations)
        {
            //OBTENGO DE LAS DLLS LA ANIMACION CUYO CONFIG TYPE SEA IGUAL AL CONFIG TYPE QUE LE PASO, Y USO ESA INSTANCIA

            var newAnimationFromDlls = DllAnimations.Where(x => x.ConfigurationType == animationConfig.GetType().Name).First();
            var newAnimation = Activator.CreateInstance(newAnimationFromDlls.GetType()) as AnimationViewModel;
            newAnimation.InitializeAnimation(animationConfig);

            return newAnimation;
        }

        private void ExecuteStage()
        {
            if (!Executing)
            {
                BackgroundWorker.RunInBackGround(delegate
                {
                    foreach (var chart in Charts)
                    {
                        chart.Reset();
                    }
                    this.Simulation.Execute();

                });
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

        private void Load_DLLs()
        {
            // Gets the folder path in which your .exe is located
            var parentFolder = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            // Makes the absolute path
            var absolutePath = System.IO.Path.Combine(parentFolder, @"dlls");

            foreach (string dll in Directory.GetFiles(absolutePath, "*.dll"))
            {
                Assembly asm = Assembly.LoadFile(dll);
                //la dll ahora tendria que tener 3 types.. la config, la animation y la animation on execution.. filtrar por tipo "padre" y crear 3 listas distintas!
                Type[] typesList = asm.GetTypes();

                foreach (var t in typesList)
                {
                    //ver si se puede meter un switch
                    //validar que siempre vengan estos 3 tipos -> flag?
                    if (t.BaseType == typeof(AnimationConfigurationBase))
                    {
                        var config = Activator.CreateInstance(t, Variables.ToList()) as AnimationConfigurationBase;
                        DllConfigurations.Add(config);
                    }
                    //comentado para que siga funcionando ok
                    if (t.BaseType == typeof(AnimationViewModel))
                    {
                        var animation = Activator.CreateInstance(t) as AnimationViewModel;
                        DllAnimations.Add(animation);
                    }
                }
            }
        }

        public void VariablesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Variable item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Variables_CollectionChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Variable item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Variables_CollectionChanged;
                }
            }
        }

        private void Variables_CollectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if(this.Variables.Any(x=>x.IsTimeVariable && x.TimeHasChange))
            {
                var timeVariable = this.Variables.First(x => x.IsTimeVariable);

                var timeValuesCount = timeVariable.Values.Count;
                
                if(timeValuesCount > 0)
                {
                    foreach (var variable in this.Variables)
                    {
                        if (!variable.IsTimeVariable && variable.Values.Count < timeValuesCount)
                        {
                            variable.Values.Add(variable.ActualValue);
                        }
                    }
                }

                timeVariable.TimeHasChange = false;
            }
        }


        #endregion
    }
}
