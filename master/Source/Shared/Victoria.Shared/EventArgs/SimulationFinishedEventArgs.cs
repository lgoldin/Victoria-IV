using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.EventArgs
{
    public class SimulationStatusChangedEventArgs : System.EventArgs
    {

        public SimulationStatusChangedEventArgs(SimulationStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Simulation Status
        /// </summary>
        public SimulationStatus Status { get; set; }


    }

    public enum SimulationStatus
    {
        Started,
        Stoped
    }
}
