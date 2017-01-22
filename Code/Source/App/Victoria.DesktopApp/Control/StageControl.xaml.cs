using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Victoria.DesktopApp.View;
using Victoria.ModelWPF;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp.Control
{
    /// <summary>
    /// Interaction logic for StageControl.xaml
    /// </summary>
    public partial class StageControl : UserControl
    {
        public StageControl()
        {
            InitializeComponent();
        }

        private void BtnAddChart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<object> parameters = new List<object>();

                var addChartPopUp = new AddChartPopUp();
                foreach (var variable in ((StageViewModel)this.DataContext).Variables)
                {
                    variable.IsSelected = false;
                }
                addChartPopUp.DataContext = this.DataContext;
                addChartPopUp.ShowDialog();
                switch (addChartPopUp.Result)
                {
                    case UI.SharedWPF.DialogResult.Accept:
                        {
                            parameters.Add(addChartPopUp.ChartName);
                            parameters.Add(((StageViewModel)this.DataContext).Variables.Where(v => v.IsSelected).ToList());
                            ((StageViewModel)this.DataContext).AddChartCommand.Execute(parameters);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al agregar un gráfico. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void btnExportStage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|PDF files (*.pdf)|*.pdf";
                    saveFileDialog.Title = "Exportar Simulacion";
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        ((StageViewModel)this.DataContext).ExportStageCommand.Execute(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al exportar el escenario. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void BtnDeleteChart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ((StageViewModel)this.DataContext).DeleteChartCommand.Execute(null);
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al eliminar un gráfico. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void BtnStopExecutionStageCommand_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ((StageViewModel)this.DataContext).StopExecutionStageCommand.Execute(null);
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp( "Se produjo un error al ejecutar el escenario. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void BtnAddAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!((StageViewModel)this.DataContext).DllConfigurations.Any())
                {
                    var viewException = new AlertPopUp("No se encontraron animaciones disponibles. Revise la configuración de Victoria.");
                    viewException.ShowDialog();
                    return;
                }

                var dllConfigurationsClone = new List<AnimationConfigurationBase>();
                foreach(var config in ((StageViewModel)this.DataContext).DllConfigurations)
                {
                    var newDllConfig = Activator.CreateInstance(config.GetType(), config.Variables) as AnimationConfigurationBase;
                    dllConfigurationsClone.Add(newDllConfig);
                }

               var addAnimationPopUp = new AddAnimationPopUp(((StageViewModel)this.DataContext).Variables, dllConfigurationsClone);

                addAnimationPopUp.ShowDialog();
                switch (addAnimationPopUp.Result)
                {
                    case UI.SharedWPF.DialogResult.Accept:
                        {
                            List<object> parameters = new List<object>();

                            parameters.Add(addAnimationPopUp.ResultConfig);
                            parameters.Add(((StageViewModel)this.DataContext).DllAnimations);

                            ((StageViewModel)this.DataContext).AddAnimationToCanvasCommand.Execute(parameters);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp( "Se produjo un error al agregar una animacion. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }
        }

        private void BtnExecute_Animations(object sender, RoutedEventArgs e)
        {
            var hasVectorVariable = (((StageViewModel)this.DataContext).Variables.Any(v => v.Name.Contains("(")));
            if ((((StageViewModel)this.DataContext).Variables.Where(v=>v.Name == "T").First().Values.Any() && ((StageViewModel)this.DataContext).Animations.Any()) || (hasVectorVariable && ((StageViewModel)this.DataContext).Animations.Any()))
            {
                try
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var animationsToExecute = this.CreateAnimationsClones(((StageViewModel)this.DataContext).Animations);
                        var animationWindow = new AnimationExecutionWindow(animationsToExecute);

                        animationWindow.Show();
                    }));

                }
                catch (Exception ex)
                {
                    var viewException = new AlertPopUp("Se produjo un error al intentar ejecutar las animaciones. Por favor revisa la configuración de las mismas.");
                    viewException.ShowDialog();
                }
            }else
            {
                var viewException = new AlertPopUp("No se puede abrir la ventana de animaciones si no se ha ejecutado la simulación o no se han creado animaciones.");
                viewException.ShowDialog();
            }
        }

        private ObservableCollection<AnimationViewModel> CreateAnimationsClones(ObservableCollection<AnimationViewModel> animations)
        {
            var animationsToExecute = new ObservableCollection<AnimationViewModel>();
            foreach(var animation in animations)
            {
                var newAnimation = Activator.CreateInstance(animation.GetType()) as AnimationViewModel;
                newAnimation.InitializeAnimation(animation.AnimationConfig);
                //Logica hardcodeada para ejercicio de vectores (pedido por la catedra)
                if (newAnimation.AnimationConfig.Variables.Any(x => x.Name.Contains("(")))
                {
                    RutinaDeValoresHarcodeadosParaEjercicioDeVectores(newAnimation);
                }
                newAnimation.BindSimulationVariableValues();
                newAnimation.X = animation.X;
                newAnimation.Y = animation.Y;
                newAnimation.AnimationOrientation = animation.AnimationOrientation;
                animationsToExecute.Add(newAnimation);
            }

            return animationsToExecute;
        }        

        private void BtnInformation_Animations(object sender, RoutedEventArgs e)
        {
            var animationInformationPopUp = new AnimationInformationPopup();
            animationInformationPopUp.Show();
        }

        private void RutinaDeValoresHarcodeadosParaEjercicioDeVectores(AnimationViewModel newAnimation)
        {
            FillValuesWithJson(newAnimation, "T", "ValoresVariablesVectoresHardcodeadas/VariableT.json");
            FillValuesWithJson(newAnimation, "TPLL", "ValoresVariablesVectoresHardcodeadas/VariableTPLL.json");
            FillValuesWithJson(newAnimation, "HV", "ValoresVariablesVectoresHardcodeadas/VariableHV.json");
            FillVectorValuesWithJsons(newAnimation, "NS(", "ValoresVariablesVectoresHardcodeadas/VariableNS");
            FillVectorValuesWithJsons(newAnimation, "TPS(", "ValoresVariablesVectoresHardcodeadas/VariableTPS");
        }

        private void FillValuesWithJson(AnimationViewModel newAnimation, string variableName, string jsonName)
        {
            var jsonValuesList = JsonConvert.DeserializeObject<List<double>>(File.ReadAllText(jsonName));

            newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableName).First().Values = new ObservableCollection<double>();

            for (var i = 0; i <= jsonValuesList.Count - 1; i++)
            {
                newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableName).First().Values.Add(jsonValuesList[i]);
            }

            if(variableName == "HV")
            {
                newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableName).First().InitialValue = 99999999;
            }
        }

        private void FillVectorValuesWithJsons(AnimationViewModel newAnimation, string vectorSubString, string jsonNameSubString)
        {
            var variableVectorList = newAnimation.AnimationConfig.Variables.Where(x => x.Name.Contains(vectorSubString));

            for(var i = 1; i <= variableVectorList.Count(); i++)
            {
                if (i<= 10)
                {
                    var jsonValuesList = JsonConvert.DeserializeObject<List<double>>(File.ReadAllText(jsonNameSubString + i + ".json"));
                    newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableVectorList.ToList()[i-1].Name).First().Values = new ObservableCollection<double>();

                    for (var a = 0; a <= jsonValuesList.Count - 1; a++)
                    {
                        newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableVectorList.ToList()[i-1].Name).First().Values.Add(jsonValuesList[a]);
                    }
                }
                else
                {
                    var jsonValuesList = JsonConvert.DeserializeObject<List<double>>(File.ReadAllText(jsonNameSubString + 10 + ".json"));
                    newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableVectorList.ToList()[i-1].Name).First().Values = new ObservableCollection<double>();

                    for (var a = 0; a <= jsonValuesList.Count - 1; a++)
                    {
                        newAnimation.AnimationConfig.Variables.Where(x => x.Name == variableVectorList.ToList()[i-1].Name).First().Values.Add(jsonValuesList[a]);
                    }
                }
            }           
        }
    }
}
