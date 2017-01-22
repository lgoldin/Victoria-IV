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
using Victoria.ModelWPF;

namespace Victoria.DesktopApp.Control
{
    /// <summary>
    /// Interaction logic for AnimationWindow.xaml
    /// </summary>
    public partial class AnimationWindow : Window
    {
        //private Storyboard s;
        public AnimationWindow(IEnumerable<Variable> variables)
        {
            InitializeComponent();

            DoAnimation(variables);
        }


        private void DoAnimation(IEnumerable<Variable> variables)
        {
            var nsValuesList = variables.Where(n => n.Name == "NS").First().ValuesEnumerable.ToList();
      
            this.QueueAnimation(null, null, 0, nsValuesList, false);   
        }

        private void QueueAnimation(object sender, EventArgs e, int i, List<double> valuesList, bool previousLeaveQueue)
        {
            if (previousLeaveQueue)
            {
                canvastest.Children.RemoveAt(0);
            }

            TranslateTransform trans = new TranslateTransform();

            previousLeaveQueue = false;

            var anim = new DoubleAnimation();

            //cola arrancó o bug de valores negativos...setteo una animacion mega corta que no hace nada para que no se cague la cadena de animaciones
            if (i == 0 || (valuesList[i] < 0 || valuesList[i - 1] < 0))
            {           
                anim.Duration = TimeSpan.FromMilliseconds(1);
            }

            //Se sumo alguien a la cola!
            else if (valuesList[i] == valuesList[i - 1] + 1)
            {
                var newPerson = this.CreatePerson();

                newPerson.RenderTransform = trans;

                anim.From = 400; 
                anim.To = LeftValueForNewPerson() == 1 ? 10 : LeftValueForNewPerson();
                anim.Duration = TimeSpan.FromSeconds(1);
            }

            //Se fue alguien!
            else if (valuesList[i] == valuesList[i - 1] - 1)
            {
                previousLeaveQueue = true;

                canvastest.Children[0].RenderTransform = trans;
                anim.From = 10;
                anim.To = 100;
                anim.Duration = TimeSpan.FromSeconds(1);

                if(canvastest.Children.Count > 1)
                {
                    for(var a = 1; a <= (canvastest.Children.Count - 1); a++)
                    {
                        this.MoveOnUnitToLeft((Rectangle)canvastest.Children[a]);
                    }
                }
            }

            i = i + 1;

            if(i <= valuesList.Count)
            {
                anim.Completed += new EventHandler((s, r) => QueueAnimation(s, r, i, valuesList, previousLeaveQueue));  
            }

            if (!previousLeaveQueue)
            {
                trans.BeginAnimation(TranslateTransform.XProperty, anim, HandoffBehavior.Compose);
            }
            else
            {
                trans.BeginAnimation(TranslateTransform.YProperty, anim, HandoffBehavior.Compose);
            }
        }

        //Aca solo se entra si se sumo alguien a la cola
        private Rectangle CreatePerson()
        {
            var newPerson = new Rectangle { Width = 10, Height = 10 };
            newPerson.Fill = new SolidColorBrush(Colors.Red);

            canvastest.Children.Add(newPerson);
            Canvas.SetLeft(newPerson, LeftValueForNewPerson());

            return newPerson;
        }

        private double LeftValueForNewPerson()
        {
            if (canvastest.Children.Count < 2)
            {
                return 1;
            }
            else
            {
                double lastPersonLeftValue = 10;

                if (Canvas.GetLeft(canvastest.Children[canvastest.Children.Count - 2]) != 1)
                {
                    lastPersonLeftValue = Canvas.GetLeft(canvastest.Children[canvastest.Children.Count - 2]);
                }

                return lastPersonLeftValue + 10;
            }
        }

        private void MoveOnUnitToLeft(Rectangle person)
        {
            TranslateTransform trans = new TranslateTransform();
            var anim = new DoubleAnimation();

            person.RenderTransform = trans;
            var personLeftValue = Canvas.GetLeft(person);

            anim.From = personLeftValue;
            anim.To = personLeftValue - 10; 

            Canvas.SetLeft(person, personLeftValue - 10);

            anim.Duration = TimeSpan.FromSeconds(1);

            trans.BeginAnimation(TranslateTransform.XProperty, anim, HandoffBehavior.Compose);
        }


        //private void PauseAnimate(object sender, RoutedEventArgs e)
        //{
        //    s.Pause(this);
        //}

        //private void ResumeAnimate(object sender, RoutedEventArgs e)
        //{
        //    s.Resume(this);
        //}
    }
}




//LOGICA DE ANIMACION BASICA COMPLETA COMENTADA POR LAS DU

//var newPerson = this.CreatePerson();

//TranslateTransform trans = new TranslateTransform();
////var translationName = "myTranslation";
////RegisterName(translationName, trans);
//newPerson.RenderTransform = trans;   //LE ASIGNO AL CUADRADITO EL TRANSLATETRANSFORM, ASI TIENE UNA DEPENDENCY PROPERTY SOBRE LA CUAL HACER LA ANIMACION

//DoubleAnimation anim = new DoubleAnimation(Canvas.GetLeft(newPerson), 10, TimeSpan.FromSeconds(1))
//{
//    //FillBehavior = FillBehavior.Stop,
//    //AutoReverse = true,
//    //RepeatBehavior = RepeatBehavior.Forever
//    Duration = TimeSpan.FromSeconds(1)
//};

//anim.Completed += QueueAnimation;

//trans.BeginAnimation(TranslateTransform.XProperty, anim, HandoffBehavior.Compose);

////s = new Storyboard();
////s.Children.Add(anim);
////Storyboard.SetTargetName(anim, translationName);
////Storyboard.SetTargetProperty(anim, new PropertyPath(TranslateTransform.XProperty));
////s.Begin(this, true);    