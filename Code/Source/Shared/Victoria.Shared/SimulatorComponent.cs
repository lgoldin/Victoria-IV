using System.Collections.Generic;
using System.Linq;
using Victoria.Shared.Contract;
using Victoria.Shared.Mocks;

namespace Victoria.Shared
{
    public class SimulatorComponent : IComponent
    {
        #region Fields
        private string xmlSimulation;
        private Simulation simulation;
        #endregion

        #region Properties

        #endregion

        #region Constructor

        public SimulatorComponent(string xmlSimulation)
        {
            this.xmlSimulation = xmlSimulation;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize dimulation component
        /// </summary>
        public void Initialize()
        {
            //TODO: Change mock
            this.simulation = XMlParserMock.GetSimulation(this.xmlSimulation);

        }

        /// <summary>
        /// Uninitialize dimulation component
        /// </summary>
        public void UnInitialize()
        {

        }

        /// <summary>
        /// Run simulation
        /// </summary>
        public void Run()
        {
            this.simulation.Execute();
            //TODO: Analize actions when simulation finished
        }

        public List<Variable> GetVariables()
        {
            return simulation.Variables;
        }

        public Variable GetVariable(string name)
        {
			return simulation.Variables.First ( v => v.Name == name);
		}
        #endregion




    }
}
