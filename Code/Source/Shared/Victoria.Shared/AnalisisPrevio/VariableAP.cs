using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.AnalisisPrevio
{
    public class VariableAP
    {
        

        #region Properties
        private string _nombre;

        public string nombre
        {
            get
            {
                return _nombre;
            }

            set
            {
                _nombre = value;
            }
        }

        private double _valor;

        public double valor
        {
            get
            {
                return _valor;
            }

            set
            {
                _valor = value;
            }
        }

        private Boolean _vector = false;

        public Boolean vector
        {
            get
            {
                return _vector;
            }

            set
            {
                _vector = value;
            }
        }

        private double _i;
        public double i
        {
            get
            {
                return _i;
            }

            set
            {
                _i = value;
            }
        }


        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return nombre;
        }
        
        #endregion Methods

    }
}
