using System;
using System.Collections.ObjectModel;
using Victoria.Shared.EventArgs;

namespace Victoria.Shared
{
    public class Variable
    {
        #region Fields
        private string _name;
        private double _initialValue;
        private double _actualValue;
        #endregion

        #region Events
        public delegate void VariableValueChangedEventHandler(object sender, VariableValueChangeEventArgs e);
        /// <summary>
        /// Event raised when variable value changed
        /// </summary>
        public event VariableValueChangedEventHandler ValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Variable Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Variable value
        /// </summary>
        public double ActualValue
        {
            get { return _actualValue; }
            set
            {
                var oldValue = _actualValue;
                _actualValue = value;

                if (this.ValueChanged != null)
                    this.ValueChanged.Invoke(this, new VariableValueChangeEventArgs(this, oldValue, _actualValue));

            }
        }

        /// <summary>
        /// Variable value
        /// </summary>
        public double InitialValue
        {
            get { return _initialValue; }
            set
            {
                _initialValue = value;

            }
        }
        #endregion

        #region Constructor

        #endregion

        #region Methods

        #endregion
    }
}
