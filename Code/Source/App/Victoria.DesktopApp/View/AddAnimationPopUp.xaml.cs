using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Victoria.ModelWPF;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Interaction logic for AddAnimationPopUp.xaml
    /// </summary>
    public partial class AddAnimationPopUp : Window
    {
        public UI.SharedWPF.DialogResult Result { get; set; }
        public IEnumerable<Variable> Variables { get; set; }


        public string AnimationType //esto deberia ser un enum o algo asi
        {
            get { return this.animationTypeCombo.SelectedValue.ToString(); }
        }

        public string AnimationName { get; set; }

        public List<AnimationConfigurationBase> DllConfigurations { get; set; }

        public AnimationConfigurationBase ResultConfig { get; set; }

        public AddAnimationPopUp(IEnumerable<Variable> variables, List<AnimationConfigurationBase> dllConfigurations)
        {
            this.Variables = variables;
            AnimationName = "Animación";
            InitializeComponent();
            DllConfigurations = dllConfigurations;
            FillCombos();
        }

        //CONSTRUCTOR PARA LA EDICION
        public AddAnimationPopUp(AnimationViewModel animationVm, List<AnimationConfigurationBase> dllConfigurations) //Mas problemas DLL...aca le puedellegar cualquier tipo de animationviewmodel, con mas propiedades a tocar...ver
        {
            Variables = animationVm.AnimationConfig.Variables;
            AnimationName = animationVm.AnimationName;
            InitializeComponent();
            DllConfigurations = dllConfigurations;
            FillCombos();
            animationTypeCombo.SelectedIndex = ((List<string>)animationTypeCombo.ItemsSource).FindIndex(i=>i.ToString() == animationVm.AnimationConfig.AnimationType); 
        }

        private void FillCombos()
        {
            //////////////////ANIMATION TYPES FROM DLLS
            List<string> types = new List<string>();

            foreach(var config in DllConfigurations)
            {
                types.Add(config.AnimationType);             
            }

            animationTypeCombo.ItemsSource = types;

            animationTypeCombo.SelectedIndex = 0;


            //////////////////VARIABLES
            var variablesOptions = this.Variables.Select(x => x.Name);

        }

        private void AnimationTypes_SelectionChanged(object sender, RoutedEventArgs e)
        {
            extraConfigsContainer.Children.Clear();

            var animationTypeSelected = ((System.Windows.Controls.ComboBox)sender).SelectedValue.ToString();

            var configurationType = DllConfigurations.Where(x => x.AnimationType == animationTypeSelected).First();

            if (configurationType.DllExtraConfigurations != null && configurationType.DllExtraConfigurations.Any())
            {
                foreach (var extraField in configurationType.DllExtraConfigurations)
                {
                    extraConfigsContainer.Children.Add(extraField);
                }

            }
        }


        private void btnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            ResultConfig = DllConfigurations.Where(x => x.AnimationType == AnimationType).First();
            ResultConfig.BindProperties(); 
            ResultConfig.AnimationName = AnimationName;
            ResultConfig.Variables = Variables;

            this.Result = UI.SharedWPF.DialogResult.Accept;
            this.Close();
        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Result = UI.SharedWPF.DialogResult.Cancel;
            this.Close();
        }

        private void mainBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }

        }
    }
}
