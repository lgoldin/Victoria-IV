using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Victoria.Shared;
using Victoria.Shared.Prism;

namespace Victoria.ViewModelWPF
{
    public abstract class ChartViewModelBase : NotificationObject
    {
        #region Fields

        private bool isVisible;
        protected Variable independentVariable;
        protected List<Variable> dependentVariables;
        private string name;

        #endregion

        #region Properties
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                this.RaisePropertyChanged("IsVisible");
            }
        }

        public Variable IndependentVariable
        {
            get { return independentVariable; }
            set
            {
                independentVariable = value;
                this.RaisePropertyChanged("IndependentVariable");
            }
        }

        public List<Variable> DependentVariables
        {
            get { return dependentVariables; }
            set
            {
                dependentVariables = value;
                this.RaisePropertyChanged("DependentVariable");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.RaisePropertyChanged("");
            }
        }
        #endregion


        #region Constructor

        public ChartViewModelBase(string name, Variable independentVariable, List<Variable> dependentVariables)
        {
            this.Name = name;
            this.DependentVariables = dependentVariables;
            this.IndependentVariable = independentVariable;
            independentVariable.ValueChanged += independentVariable_ValueChanged;

        }

        private void independentVariable_ValueChanged(object sender, Shared.EventArgs.VariableValueChangeEventArgs e)
        {
            this.DoAddPoint();

        }
        #endregion

        #region Methods

        protected abstract void DoAddPoint();
        public abstract void Reset();

        #endregion


    }
}
