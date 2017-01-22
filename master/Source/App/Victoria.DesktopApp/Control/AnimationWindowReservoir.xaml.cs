using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Victoria.ModelSL;

namespace Victoria.DesktopApp.Control
{
    /// <summary>
    /// Interaction logic for AnimationWindow.xaml
    /// </summary>
    public partial class AnimationWindowReservoir : Window
    {
        private Storyboard s;
        public AnimationWindowReservoir(IEnumerable<Variable> variables)
        {
            InitializeComponent();
            DoAnimation(variables);
        }


        private void DoAnimation(IEnumerable<Variable> variables)
        {
            var nsValuesList = variables.Where(n => n.Name == "NS").First().ValuesEnumerable.ToList();

            var reservoir = this.createReservoir();

            this.QueueAnimation(null, null, 0, nsValuesList, reservoir);         
        }

        private void QueueAnimation(object sender, EventArgs e, int i, List<double> valuesList, Rectangle reservoir)
        {
            TranslateTransform trans = new TranslateTransform();

            //porcentaje que aumenta o disminuye el "agua"
            var percentage = 1 / valuesList.Count;

            var anim = new DoubleAnimation();

            reservoir.RenderTransform = trans;

            //cola arranco o bug de valores negativos...setteo una animacion mega corta que no hace nada para que no se cague la cadena de animaciones

            if (i == 0 || (valuesList[i] < 0 || valuesList[i - 1] < 0))
            {           
                anim.Duration = TimeSpan.FromMilliseconds(1);
            }

            //Aumenta valor de NS
            else if (valuesList[i] == valuesList[i - 1] + 1)
            {
                anim.From = 0; 
                anim.To = 100;
                anim.Duration = TimeSpan.FromSeconds(1);
            }

            //disminuye valor NS
            else if (valuesList[i] == valuesList[i - 1] - 1)
            {
                anim.From = 100;
                anim.To = 0;
                anim.Duration = TimeSpan.FromSeconds(1);
            }

            i = i + 1;

            if(i <= valuesList.Count)
            {
                anim.Completed += new EventHandler((s, r) => QueueAnimation(s, r, i, valuesList, reservoir));  
            }
        }

        //Aca solo se entra si se sumo alguien a la cola
        private Rectangle createReservoir()
        {
            var border = new Border();
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = Brushes.Black;
            border.Background = Brushes.LightGray;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.VerticalAlignment = VerticalAlignment.Top;
            border.Width = 110;
            border.Height = 110;

            var innerCanvas = new Canvas();
            innerCanvas.Height = 100;
            innerCanvas.Width = 100;
            innerCanvas.ClipToBounds = true;

            var reservoir = new Rectangle { Width = 100, Height = 100 };
            reservoir.Fill = new SolidColorBrush(Colors.Aquamarine);

            innerCanvas.Children.Add(reservoir);
            border.Child = innerCanvas;    

            canvastest.Children.Add(border);
            //Canvas.SetLeft(newPerson, LeftValueForNewPerson());

            return reservoir;
        }

            //       <Border
            //            BorderThickness = "2"
            //            BorderBrush="Black"
            //            Background="LightGray"
            //            HorizontalAlignment="Left"
            //            VerticalAlignment="Top"
            //            Width="110"
            //            Height="110">

            //    <Canvas Height = "100" Width="100" ClipToBounds="True">
            //        <Rectangle x:Name="myrect" Height="100" Width="100" Canvas.Top="0" Canvas.Left="0" Fill="Aquamarine">
            //            <Rectangle.Triggers>
            //                <EventTrigger RoutedEvent = "Window.Loaded" >
            //                    < BeginStoryboard >
            //                        < Storyboard RepeatBehavior="Forever">
            //                            <DoubleAnimation Storyboard.TargetName="myrect" 
            //                                       Storyboard.TargetProperty= "(Canvas.Top)" From= "0" To= "100"
            //                                       Duration= "0:0:5" BeginTime= "0:0:0" />
            //                            < DoubleAnimation Storyboard.TargetName= "myrect"
            //                                       Storyboard.TargetProperty= "(Canvas.Top)" From= "100" To= "0"
            //                                       Duration= "0:0:5" BeginTime= "0:0:5" />
            //                        </ Storyboard >
            //                    </ BeginStoryboard >
            //                </ EventTrigger >
            //            </ Rectangle.Triggers >
            //        </ Rectangle >
            //    </ Canvas >

            //</ Border >

        private void PauseAnimate(object sender, RoutedEventArgs e)
        {
            s.Pause(this);
        }

        private void ResumeAnimate(object sender, RoutedEventArgs e)
        {
            s.Resume(this);
        }
    }
}