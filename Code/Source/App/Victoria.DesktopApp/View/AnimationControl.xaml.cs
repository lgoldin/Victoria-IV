using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Victoria.DesktopApp.Behavior;
using Victoria.DesktopApp.Helpers;
using Victoria.ModelWPF;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp.View
{
    /// <summary>
    /// Lógica de interacción para AnimationControl.xaml
    /// </summary>
    public partial class AnimationControl : UserControl
    {
        protected bool isDragging;
        private Point clickPosition;
        public AnimationControl()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(Control_MouseMove);
        }

        public void BtnEditAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            AnimationViewModel animationViewModel = (AnimationViewModel)((Button)sender).DataContext;
            Button dataContext = (Button)sender;
            ItemsControl itemsControl = VisualTreeHelpers.FindAncestor<ItemsControl>(dataContext);
           
            var dllConfigurationsClone = new List<AnimationConfigurationBase>();
            foreach (var config in ((StageViewModel)itemsControl.DataContext).DllConfigurations)
            {
                var newDllConfig = Activator.CreateInstance(config.GetType(), config.Variables) as AnimationConfigurationBase;
                dllConfigurationsClone.Add(newDllConfig);
            }

            try
            {
                var addAnimationPopUp = new AddAnimationPopUp(animationViewModel, dllConfigurationsClone);

                addAnimationPopUp.ShowDialog();
                switch (addAnimationPopUp.Result)
                {
                    case UI.SharedWPF.DialogResult.Accept:
                        {
                            var newAnimationFromDlls = ((StageViewModel)itemsControl.DataContext).DllAnimations.Where(x => x.ConfigurationType == addAnimationPopUp.ResultConfig.GetType().Name).First();
                            var newAnimation = Activator.CreateInstance(newAnimationFromDlls.GetType()) as AnimationViewModel;
                            newAnimation.InitializeAnimation(addAnimationPopUp.ResultConfig);
                            newAnimation.BindSimulationVariableValues();
                            newAnimation.X = animationViewModel.X; //NO funciona
                            newAnimation.Y = animationViewModel.Y; //NO funciona
                            ((StageViewModel)itemsControl.DataContext).Animations.Remove(animationViewModel); //borro y creo nueva 
                            ((StageViewModel)itemsControl.DataContext).Animations.Add(newAnimation);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var viewException = new AlertPopUp("Se produjo un error al editar una animacion. Para ver detalles, despliegue el control correspondiente.");
                viewException.ShowDialog();
            }

        }

        public void BtnRemoveAnimation_OnClick(object sender, RoutedEventArgs e)
        {           
            AnimationViewModel animationViewModel = (AnimationViewModel)((Button)sender).DataContext;
            Button dataContext = (Button)sender;
            ItemsControl itemsControl = VisualTreeHelpers.FindAncestor<ItemsControl>(dataContext);

            ((StageViewModel)itemsControl.DataContext).Animations.Remove(animationViewModel);          
        }

        public void BtnChangeOrientation_OnClick(object sender, RoutedEventArgs e)
        {
            AnimationViewModel animationViewModel = (AnimationViewModel)((Button)sender).DataContext;       
            animationViewModel.AnimationOrientation = animationViewModel.AnimationOrientation == "Vertical" ? "Horizontal" : "Vertical";
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableControl = sender as UserControl;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as UserControl;
            draggable.ReleaseMouseCapture();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as UserControl;

            if (isDragging && draggableControl != null)
            {
                Grid grid = this.FindAncestorMethod<Grid>();

                var itemsControl = this.FindAncestorMethod<ItemsControl>();
                var stackpa = itemsControl.FindAncestorMethod<StackPanel>(); //Acá logré obtener la referencia al stackpanel que tiene adentro a las anims, para intentar obtener el width y heigth de sus "paredes" para limitar el dragandrop. Pero los limites son más grandes que lo que se ve visualmente .
                var border = stackpa.FindAncestorMethod<Border>();

                Point currentPosition = e.GetPosition(grid);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                if((currentPosition.Y - clickPosition.Y) >= 0 && (currentPosition.Y - clickPosition.Y) <= stackpa.ActualHeight - 80 && (currentPosition.X - clickPosition.X) >= 0 && (currentPosition.X - clickPosition.X) <= (stackpa.ActualWidth - 120))
                {
                    transform.X = currentPosition.X - clickPosition.X;
                    transform.Y = currentPosition.Y - clickPosition.Y;
                }

                ((AnimationViewModel)this.DataContext).X = transform.X;
                ((AnimationViewModel)this.DataContext).Y = transform.Y;
            }
        }
    }
}
