using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Victoria.ModelSL;
using Victoria.Shared;
using Victoria.Shared.EventArgs;
using Victoria.Shared.Prism;
using Variable = Victoria.ModelSL.Variable;

namespace Victoria.ViewModelSL
{
    public abstract class TestViewModelBase : NotificationObject
    {
        protected SimulatorComponent simulator;
        protected ObservableCollection<Victoria.ModelSL.Variable> variables;
        private ChartViewModelBase chartViewModel;
        protected DelegateCommand startCommand;
        /// <summary>
        /// Gets StartCommand.
        /// </summary>
        public ICommand StartCommand
        {
            get
            {
                return this.startCommand;
            }
        }

        public ObservableCollection<Variable> Variables
        {
            get { return variables; }
            set
            {
                variables = value;
                this.RaisePropertyChanged("Variables");
            }
        }

        public ChartViewModelBase ChartViewModel
        {
            get { return chartViewModel; }
            set
            {
                chartViewModel = value;
                this.RaisePropertyChanged("ChartViewModel");
            }
        }

        public TestViewModelBase()
        {

            this.startCommand = new DelegateCommand(this.RunSimulation);
            simulator = new SimulatorComponent("");
            simulator.Initialize();
            Variables = new ObservableCollection<Variable>();
            foreach (var variable in this.simulator.GetVariables())
            {
                this.Variables.Add(new Variable(variable) { Name = variable.Name, Values = new ObservableCollection<double>() { variable.ActualValue } });
            }

        }

        public abstract void RunSimulation();


    }
}
