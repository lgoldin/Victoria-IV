using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Victoria.Shared.Prism;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;

namespace Victoria.ViewModelWPF
{
    public class AnimationExecutionViewModel : AnimationExecutionViewModelBase
    {
        protected new DelegateCommand executeAnimationsCommand;
        protected new DelegateCommand stopAnimationsCommand;

        private ObservableCollection<AnimationViewModel> animations;
        private ObservableCollection<ModelWPF.Variable> variables;
        private bool executeButtonEnabled;
        public int animationWait = 400;
        public ObservableCollection<AnimationViewModel> Animations
        {
            get { return animations; }
            set
            {
                animations = value;
                this.RaisePropertyChanged("Animations");
            }
        }

        public ObservableCollection<ModelWPF.Variable> Variables
        {
            get { return variables; }
            set
            {
                variables = value;
                this.RaisePropertyChanged("Variables");
            }
        }

        public bool ExecuteButtonEnabled
        {
            get { return executeButtonEnabled; }
            set
            {
                executeButtonEnabled = value;
                this.RaisePropertyChanged("ExecuteButtonEnabled");
            }
        }

        public int AnimationWait
        {
            get { return animationWait; }
            set
            {
                animationWait = value;
                this.RaisePropertyChanged("AnimationWait");
            }
        }

        public bool AnimationsExecuting { get; set; }

        public new ICommand ExecuteAnimationsCommand
        {
            get
            {
                return this.executeAnimationsCommand;
            }
        }
        public new ICommand StopAnimationsCommand
        {
            get
            {
                return this.stopAnimationsCommand;
            }
        }

        public AnimationExecutionViewModel()
        {
            this.executeAnimationsCommand = new DelegateCommand(this.ExecuteAnimations);
            this.stopAnimationsCommand = new DelegateCommand(this.StopAnimations);
        }

        private void ExecuteAnimations()
        {
            //////Inyección de valores de variables pedido por la catedra de simulación para el caso de ejercicios de vectores (del cual no funciona la simulación, pero se pidió se pueda animar con valores falsos)
            if(this.Variables.Any(x => x.Name.Contains("(")))
            {
                RutinaDeValoresHarcodeadosParaEjercicioDeVectores();          
            }

            this.AnimationsExecuting = true;
            var tValuesList = this.Variables.Where(n => n.Name == "T").First().ValuesEnumerable.ToList();
            ThreadPool.QueueUserWorkItem(ignored =>
            {
                for (var i = 0; i < tValuesList.Count; i++)
                {
                    if (this.AnimationsExecuting)
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var animation in this.Animations)
                            {
                                foreach (var variable in Variables)
                                {
                                    if (variable.Values.Count > i)
                                    {
                                        variable.ActualValue = variable.Values[i];
                                    }
                                }
                                animation.DoAnimation(i);
                            }
                        }));
                        Thread.Sleep(AnimationWait);
                    }
                    else
                    {
                        break;
                    }
                }

                ExecuteButtonEnabled = true;

            });

        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.StopAnimations();
        }

        private void StopAnimations()
        {
            this.AnimationsExecuting = false;
        }

        private void RutinaDeValoresHarcodeadosParaEjercicioDeVectores()
        {
            //Llenar con valores falsos todas las variables de NS TPS y HV

            //Logica para guardar jsons con valores
            //File.WriteAllText("ValoresVariablesVectoresHardcodeadas/VariableT.json", JsonConvert.SerializeObject(this.Variables.Where(n => n.Name == "T").First().Values.ToList().GetRange(0, 1000)));
            //File.WriteAllText("ValoresVariablesVectoresHardcodeadas/VariableTPLL.json", JsonConvert.SerializeObject(this.Variables.Where(n => n.Name == "TPLL").First().Values.ToList().GetRange(0, 1000)));
            //File.WriteAllText("ValoresVariablesVectoresHardcodeadas/VariableHV.json", JsonConvert.SerializeObject(this.Variables.Where(n => n.Name == "HV").First().Values.ToList().GetRange(0, 1000)));
            //File.WriteAllText("ValoresVariablesVectoresHardcodeadas/VariableNS1.json", JsonConvert.SerializeObject(this.Variables.Where(n => n.Name == "NS").First().Values.ToList().GetRange(0, 1000)));
            //File.WriteAllText("ValoresVariablesVectoresHardcodeadas/VariableTPS1.json", JsonConvert.SerializeObject(this.Variables.Where(n => n.Name == "TPS").First().Values.ToList().GetRange(0, 1000)));


            //Logica para leer jsons con valores (Se paso a StageControl.xaml.cs)
            //Llenado de variables
            //FillValuesWithJson("T", "ValoresVariablesVectoresHardcodeadas/VariableT.json");
            //FillValuesWithJson("TPLL", "ValoresVariablesVectoresHardcodeadas/VariableTPLL.json");
            //FillValuesWithJson("HV", "ValoresVariablesVectoresHardcodeadas/VariableHV.json");
            //FillValuesWithJson("NS(1)", "ValoresVariablesVectoresHardcodeadas/VariableNS1.json");
            //FillValuesWithJson("TPS(1)", "ValoresVariablesVectoresHardcodeadas/VariableTPS1.json");
        }

        private void FillValuesWithJson(string variableName, string jsonName)
        {
            var jsonValuesList = JsonConvert.DeserializeObject<List<double>>(File.ReadAllText(jsonName));

            this.Variables.Where(x => x.Name == variableName).First().Values = new ObservableCollection<double>();

            for (var i = 0; i <= jsonValuesList.Count - 1; i++)
            {
                this.Variables.Where(x => x.Name == variableName).First().Values.Add(jsonValuesList[i]);
            }
        }
    }
}