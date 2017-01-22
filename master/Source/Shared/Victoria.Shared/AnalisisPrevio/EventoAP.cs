using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.AnalisisPrevio
{
    public class EventoAP
    {
        #region Fields

        private string _nombre;

        private string _eventoNoCondicionado;

        private string _eventoCondicionado;

        private string _encadenador;

        private string _condicion;

        private string _tef;

        private bool _arrepentimiento;

        private string _arrepentimientoStr;

        #endregion Fields

        #region Properties

        public string Nombre
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

        private Boolean _vector = false;

        public Boolean Vector
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

        public string EventoNoCondicionado
        {
            get
            {
                return _eventoNoCondicionado;
            }

            set
            {
                _eventoNoCondicionado = value;
            }
        }

        public string EventoCondicionado
        {
            get
            {
                return _eventoCondicionado;
            }

            set
            {
                _eventoCondicionado = value;
            }
        }

        public string Encadenador
        {
            get
            {
                return _encadenador;
            }

            set
            {
                _encadenador = value;
            }
        }
        public string TEF
        {
            get
            {
                return _tef;
            }

            set
            {
                _tef = value;
            }
        }

        public bool Arrepentimiento
        {
            get
            {
                return _arrepentimiento;
            }

            set
            {
                _arrepentimiento = value;
            }
        }

        public string ArrepentimientoStr
        {
            get
            {
                return _arrepentimiento? "Sí": "No";
            }

            set
            {
                _arrepentimientoStr = value;
            }
        }

        public string Condicion
        {
            get
            {
                return _condicion;
            }

            set
            {
                _condicion = value;
            }
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Nombre;
        }

        public bool validarEncadenador(List<string> itemsSource)
        {
            return itemsSource.Any(item => item == Encadenador);
        }

       
        #endregion Methods
    }
}
