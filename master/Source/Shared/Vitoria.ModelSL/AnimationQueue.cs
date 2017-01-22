//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Net;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;

//namespace Vitoria.ModelSL
//{
//    public class AnimationQueue : Animation
//    {
//        public int NsLimit { get; set; }
//        public double NsActualValue { get; set; }
//        //public ObservableCollection<Ellipse> ElipseList { get; set; } //test

//        public AnimationQueue(AnimationConfigurationQueue animationConfig) : base(animationConfig)
//        {
//            this.RectangleWidth = 120;
//            this.RectangleHeight = 30;
//            this.Color = "Red";
//            this.NsLimit = animationConfig.SelectedNsLimit;

//            //this.ElipseList = new ObservableCollection<Ellipse>();
//            //for (int i = 1; i <= NsLimit; i++)
//            //{
//            //    var a = new Ellipse();
//            //    a.Height = 10;
//            //    a.Width = 10;
//            //    a.Fill = new SolidColorBrush(Colors.Black);
//            //    this.ElipseList.Add(a);
//            //}
//        }
//        public override void DoAnimation(int index)
//        {
//            var nsValuesList = this.AnimationConfiguration.Variables.Where(n => n.Name == AnimationConfiguration.SelectedVariable).First().ValuesEnumerable.ToList();
//            //var nsValuesList = this.AnimationConfiguration.Variables.Where(n => n.Name == "NS").First().ValuesEnumerable.ToList();


//            if (nsValuesList[index] == 1)
//            {
//                this.Color = "Blue";
//                this.AnimationConfiguration.AnimationName = "Test1";

//            }
//            else
//            {
//                this.Color = "Green";
//                this.AnimationConfiguration.AnimationName = "Test2";
//            }

//            this.VariableToAnimateValue = nsValuesList[index].ToString();
//            this.NsActualValue = nsValuesList[index];
//        }
//    }
//}
