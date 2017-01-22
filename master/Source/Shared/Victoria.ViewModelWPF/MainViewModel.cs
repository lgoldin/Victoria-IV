using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using System.Windows.Input;
using iTextSharp.text.exceptions;
using Microsoft.Practices.Prism.Commands;
using Victoria.Shared;
using Victoria.Shared.Mocks;
using Victoria.Shared.Prism;
using Victoria.UI.SharedWPF;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Victoria.ViewModelWPF
{
    public class MainViewModel : MainViewModelBase
    {
        #region Fields

        protected string simulationFile;
        private string simulationURI;
        private bool isXmlPlainSimulation;
        protected DelegateCommand openSimulationCommand;
        protected DelegateCommand openXmlPlainSimulationCommand;
        protected DelegateCommand saveSimulationCommand;
        protected DelegateCommand executeSimulationCommand;
        protected DelegateCommand exportSimulationCommand;
        protected DelegateCommand addStageCommand;
        protected DelegateCommand deleteStageCommand;

        #endregion

        #region Properties

        public delegate DialogResult MostrarMensajeCerrarSimulacionDelegate();
        public MostrarMensajeCerrarSimulacionDelegate MostrarMensajeCerrarSimulacion;


        public string SimulationUri
        {
            get { return simulationURI; }
            set
            {
                simulationURI = value;
                this.RaisePropertyChanged("SimulationUri");
                this.RaisePropertyChanged("IsSimulationOpen");
            }
        }

        public string SimulationFile
        {
            get { return simulationFile; }
            set
            {
                simulationFile = value;
                this.RaisePropertyChanged("SimulationFile");
                this.RaisePropertyChanged("IsSimulationOpen");
                this.RaisePropertyChanged("IsNotXmlPlainSimulationOpen");
            }
        }

        public bool IsSimulationOpen
        {
            get { return !string.IsNullOrEmpty(SimulationFile); }
        }

        public bool IsNotXmlPlainSimulationOpen
        {
            get { return !string.IsNullOrEmpty(SimulationFile) && !IsXmlPlainSimulation; }

        }

        public bool IsXmlPlainSimulation
        {
            get { return isXmlPlainSimulation; }
            set
            {
                isXmlPlainSimulation = value;
                this.RaisePropertyChanged("IsXmlPlainSimulation");
                this.RaisePropertyChanged("IsNotXmlPlainSimulationOpen");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets OpenSimulationCommand.
        /// </summary>
        public ICommand OpenSimulationCommand
        {
            get
            {
                return this.openSimulationCommand;
            }
        }

        public ICommand OpenXmlPlainSimulationCommand
        {
            get
            {
                return this.openXmlPlainSimulationCommand;
            }
        }

        /// <summary>
        /// Gets SaveSimulationCommand.
        /// </summary>
        public ICommand SaveSimulationCommand
        {
            get
            {
                return this.saveSimulationCommand;
            }
        }

        /// <summary>
        /// Gets ExecuteSimulationCommand.
        /// </summary>
        public ICommand ExecuteSimulationCommand
        {
            get
            {
                return this.executeSimulationCommand;
            }
        }

        /// <summary>
        /// Gets ExecuteSimulationCommand.
        /// </summary>
        public ICommand ExportSimulationCommand
        {
            get
            {
                return this.exportSimulationCommand;
            }
        }

        /// <summary>
        /// Gets AddStageCommand.
        /// </summary>
        public ICommand AddStageCommand
        {
            get
            {
                return this.addStageCommand;
            }
        }

        /// <summary>
        /// Gets DeleteStageCommand.
        /// </summary>
        public ICommand DeleteStageCommand
        {
            get
            {
                return this.deleteStageCommand;
            }
        }

        #endregion

        public MainViewModel()
        {
            Initialize();
        }

        #region Methods
        private void Initialize()
        {
            this.openSimulationCommand = new DelegateCommand(this.OpenSimulation);
            this.openXmlPlainSimulationCommand = new DelegateCommand(this.OpenXmlPlainSimulation);
            this.saveSimulationCommand = new DelegateCommand(this.SaveSimulation);
            this.exportSimulationCommand = new DelegateCommand(this.ExportSimulation);
            this.executeSimulationCommand = new DelegateCommand(this.ExecuteSimulation);
            this.addStageCommand = new DelegateCommand(this.AddStage);
            this.deleteStageCommand = new DelegateCommand(this.DeleteStage);
            this.Stages = new ObservableCollection<StageViewModelBase>();
        }

        private void DeleteStage(object stage)
        {
            var stageIndex = this.Stages.IndexOf((StageViewModelBase)stage);
            var selectedStageIndex = this.Stages.IndexOf(SelectedStage);

            if (this.Stages.Count == 1)
            {
                var result = this.MostrarMensajeCerrarSimulacion();
                switch (result)
                {
                    case UI.SharedWPF.DialogResult.SaveAndClose:
                        {
                            this.SaveSimulation(this.SimulationUri);
                        }
                        break;
                    case UI.SharedWPF.DialogResult.Cancel:
                        {
                            return;
                        }
                }

                this.SimulationUri = null;
                this.SimulationFile = null;
            }

            this.Stages.Remove((StageViewModelBase)stage);
            if (stageIndex == selectedStageIndex)
            {
                if (Stages.Count > 0)
                {
                    SelectedStage = stageIndex > 0 ? this.Stages[stageIndex - 1] : this.Stages[0];
                }
                else
                {
                    SelectedStage = null;
                }
            }
        }

        private void AddStage(object stageName)
        {
            var simulation = XMLParser.GetSimulation(this.SimulationFile);
            //simulation.InitializeForNewStage();
            var newStage = new StageViewModel(simulation) { Name = stageName.ToString() };
            this.Stages.Add(newStage);
            this.SelectedStage = newStage;
        }

        private void ExecuteSimulation()
        {
            foreach (StageViewModel s in this.Stages)
            {
                s.ExecuteStageCommand.Execute(null);
            }
        }

        private void ExportSimulation(object simulacionFileName)
        {
            if (simulacionFileName.ToString().Substring(simulacionFileName.ToString().Length - 4, 4).Equals("xlsx"))
                HelperExport.HelperExport.ExportStagesToExcel(this.Stages, simulacionFileName.ToString());
            else if (simulacionFileName.ToString().Substring(simulacionFileName.ToString().Length - 3, 3).Equals("pdf"))
                HelperExport.HelperExport.ExportStagesToPDF(this.Stages, simulacionFileName.ToString());
            else
                throw new Exception("Extensión no compatible");
        }

        private void SaveSimulation(object simulacionUri)
        {
            var xmlSimulation = this.GetSimulationXMLFromSimulation(this.SimulationFile);
            if (simulacionUri != null)
                xmlSimulation.Save(simulacionUri.ToString());
            else
                xmlSimulation.Save(this.SimulationUri);

        }

        private void OpenSimulation(object simulationURI)
        {
            IsXmlPlainSimulation = false;

            SimulationFile = File.ReadAllText(simulationURI.ToString());

            var simulation = XMLParser.GetSimulation(this.SimulationFile);

            if (simulation.Stages.Any())
            {
                foreach (var stage in simulation.Stages)
                {
                    var newStage = new StageViewModel(simulation, stage) { Name = stage.Name };
                    this.Stages.Add(newStage);
                }
                this.SelectedStage = this.Stages[0];
            }
            else
            {
                var newStage = new StageViewModel(simulation) { Name = "Principal" };
                this.Stages.Add(newStage);
                this.SelectedStage = newStage;
            }
            this.SimulationUri = simulationURI.ToString();
        }

        private void OpenXmlPlainSimulation(object simulationXmlText)
        {
            //esta linea va!
            this.SimulationFile = (string)simulationXmlText;
            //

            ////esta linea no va, es para prueba ahora tengo hardcodeado un path con un ejercicio!!!
           // this.SimulationFile = File.ReadAllText(simulationXmlText.ToString());
            ////

            this.IsXmlPlainSimulation = true;

            var simulation = XMLParser.GetSimulation(this.SimulationFile);

            if (simulation.Stages.Any())
            {
                foreach (var stage in simulation.Stages)
                {
                    var newStage = new StageViewModel(simulation, stage) { Name = stage.Name };
                    this.Stages.Add(newStage);
                }
                this.SelectedStage = this.Stages[0];
            }
            else
            {
                var newStage = new StageViewModel(simulation) { Name = "Principal" };
                this.Stages.Add(newStage);
                this.SelectedStage = newStage;
            }
        }

        protected XDocument GetSimulationXMLFromSimulation(string simulationFile)
        {
            var xmlSimulation = XDocument.Parse(simulationFile);
            //saco los stages que habia, para cargar los nuevos.
            xmlSimulation.Descendants("Stages").Remove();

            var stages = (this.Stages.Cast<StageViewModel>()).Select(st =>

                new XElement("Stage",

                    new XAttribute("Name", st.Name),

                    st.Variables.Select(v =>

                        new XElement("Variable",

                            new XAttribute("Name", v.Name),

                            new XAttribute("Value", v.InitialValue)

                            )

                        ),
                        st.Charts.Select(ch => new XElement("Chart",
                            new XAttribute("Name", ch.Name),

                            ch.DependentVariables.Select(v =>

                            new XElement("Variable",

                                new XAttribute("Name", v.Name)

                                ))
                            ))

                    )

                ).ToArray();

            xmlSimulation.Root.Add(new XElement("Stages", stages));

            return xmlSimulation;
        }

        public void DeleteAllStages()
        {
            this.Stages.Clear();
            this.SimulationUri = null;
            this.SimulationFile = null;
        }
        #endregion


    }
}
