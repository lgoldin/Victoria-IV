using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Victoria.ModelWPF;
using Victoria.Shared.Prism;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace Victoria.ViewModelWPF
{
    public abstract class AnimationViewModel : NotificationObject
    {
        private string animationName;
        private string variableToAnimateValue;
        public string animationOrientation = "Vertical";
        private ObservableCollection<Shape> animationElementsList;
        public AnimationConfigurationBase AnimationConfig { get; set; }

        public abstract string ConfigurationType { get; set; }

        public string VariableToAnimateName { get; set; }

        public ObservableCollection<Shape> AnimationElementsList
        {
            get { return animationElementsList; }
            set
            {
                animationElementsList = value;
                this.RaisePropertyChanged("AnimationElementsList");
            }
        }

        public string AnimationName
        {
            get { return animationName; }
            set
            {
                animationName = value;
                this.RaisePropertyChanged("AnimationName");
            }
        }

        public string AnimationOrientation
        {
            get { return animationOrientation; }
            set
            {
                animationOrientation = value;
                this.RaisePropertyChanged("AnimationOrientation");
            }
        }

        private double x;
        private double y;

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                this.RaisePropertyChanged("Y");
            }
        }

        public double X
        {
            get { return x; }
            set
            {
                x = value;
                this.RaisePropertyChanged("X");
            }
        }

        public string VariableToAnimateValue
        {
            get { return variableToAnimateValue; }
            set
            {
                variableToAnimateValue = value;
                this.RaisePropertyChanged("VariableToAnimateValue");
            }
        }

        public AnimationViewModel()
        {

        }

        public virtual void InitializeAnimation(AnimationConfigurationBase animationConfig)
        {
            this.AnimationConfig = animationConfig;
            this.AnimationName = this.AnimationConfig.AnimationName;
        }

        public abstract void BindSimulationVariableValues();

        public abstract void DoAnimation(int index);

    }
}
