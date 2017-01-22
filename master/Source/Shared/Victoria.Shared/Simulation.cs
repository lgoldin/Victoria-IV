using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Victoria.Shared.Mocks;
using Victoria.Shared.EventArgs;

namespace Victoria.Shared
{
    public class Simulation
    {
        private bool stopExecution;

        public List<Diagram> Diagrams
        {
            get;
            set;
        }

        public List<Variable> Variables
        {
            get;
            set;
        }

		List<Stage> stages = new List<Stage>();
		public List<Stage> Stages {
			get {
				return stages;
			}
			set {
				stages = value;
			}
		}
		private Diagram mainDiagram { get { return Diagrams.First(x => x.Name == "Principal"); } }

        #region Events
        public delegate void SimulationStatusChangedDelegate(object sender, SimulationStatusChangedEventArgs e);
        /// <summary>
        /// Event raised when variable value changed
        /// </summary>
        public event SimulationStatusChangedDelegate SimulationStatusChanged;

        #endregion


        #region Constructor

        public void InitializeForNewStage()
        {
            foreach (var variable in Variables)
            {
                variable.InitialValue = 0;
                variable.ActualValue = 0;
            }
        }

        public Simulation(IList<Diagram> diagrams, Dictionary<string, Variable> variables)
        {
            this.Diagrams = diagrams.ToList();
            this.Variables = variables.Values.ToList();
            this.Variables.Insert(0, new Variable() { ActualValue = 0, InitialValue = 0, Name = "T" });
        }

		public Simulation (List<Diagram> diagramas, Dictionary<string, Variable> variables, List<Stage> stages):this(diagramas, variables)
		{
			this.Stages = stages;
		}

        public Simulation(DiagramMock diagram, Dictionary<string, Variable> variables)
        {

        }
        public Simulation()
        {
        }

        #endregion

        #region Methods

        public void Execute()
        {
            if (SimulationStatusChanged != null)
                SimulationStatusChanged(this, new SimulationStatusChangedEventArgs(SimulationStatus.Started));

            //inicializo las variables segun la configuracion del escenario
            foreach (var variable in Variables.Where(v => v.Name != "T"))
            {
                if (variable is VariableArray)
                {
                    foreach (var variableItem in ((VariableArray)variable).Variables)
                    {
                        variableItem.ActualValue = variableItem.InitialValue;
                    }
                }
                else
                {
                    variable.ActualValue = variable.InitialValue;
                }
            }
            var timeVariable = Variables.First(v => v.Name == "T");
            timeVariable.ActualValue = timeVariable.InitialValue;

            Node node = mainDiagram.Execute(Variables); ;
            while (node != null && !stopExecution)
            {
                node = node.Execute(Variables);
            }

            if (SimulationStatusChanged != null)
                SimulationStatusChanged(this, new SimulationStatusChangedEventArgs(SimulationStatus.Stoped));
            this.stopExecution = false;
        }

        public void StopExecution()
        {
            this.stopExecution = true;
        }
        #endregion


    }
}
