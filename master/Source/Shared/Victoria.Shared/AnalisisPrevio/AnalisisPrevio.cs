using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Victoria.Shared.AnalisisPrevio
{
    public class AnalisisPrevio
    {
        public enum Tipo
        {
            EaE,
            DeltaT
        };

        public Tipo TipoDeEjercicio { get; set; }

        public AnalisisPrevio(Tipo tipo)
        {
            Datos = new ObservableCollection<string>();
            VariablesDeControl = new ObservableCollection<string>();
            VariablesEstado = new ObservableCollection<VariableAP>();
            VariablesResultado = new ObservableCollection<VariableAP>();
            Propios = new ObservableCollection<string>();
            ComprometidosAnterior = new ObservableCollection<string>();
            ComprometidosFuturos = new ObservableCollection<string>();
            Tefs = new ObservableCollection<string>();
            VariablesEstado.Add(new VariableAP() { nombre = "NS", valor = 0 });
            VariablesResultado.Add(new VariableAP() { nombre = "PPS", valor = 0 });
            VariablesResultado.Add(new VariableAP() { nombre = "PTO", valor = 0 });
            EventosEaE = new ObservableCollection<EventoAP>();
            if (Tipo.EaE.Equals(tipo))
            {
                EventosEaE.Add(new EventoAP() { Nombre = "Llegada", Arrepentimiento = false, EventoCondicionado = "Salida", EventoNoCondicionado = "Llegada", TEF = "TPLL", Encadenador = "IA" });
                EventosEaE.Add(new EventoAP() { Nombre = "Salida", Arrepentimiento = false, EventoCondicionado = "Salida", TEF = "TPS", Encadenador = "TA" });
                Datos.Add("IA");
                Datos.Add("TA");
            }
            else
            {
                Propios.Add("Venta");
                ComprometidosFuturos.Add("Emisión");
                ComprometidosAnterior.Add("Llegada");
                Tefs.Add("TPLL");
            }

            TipoDeEjercicio = tipo;

        }

        internal string evento(string key, ObservableCollection<string> collection)
        {
            if(collection.Count == 0) return null;
            var str = collection.First(item => item.ToUpper() == key.ToUpper());
            return str;
        }

        internal EventoAP evento(string key)
        {
            if (EventosEaE.Count == 0) return null;
            return EventosEaE.First(item => item.Nombre.ToUpper() == key.ToUpper());
            
        }

        #region Fields

        private ObservableCollection<string> _datos;

        private ObservableCollection<string> _variablesDeControl;

        private ObservableCollection<VariableAP> _variablesResultado;

        private ObservableCollection<VariableAP> _variablesEstado;

        private ObservableCollection<EventoAP> _eventosEaE;

        private ObservableCollection<string> _propios;

        internal string resultados()
        {
            var resultadoStr = "";
            foreach(var resultado in VariablesResultado) {
                resultadoStr = resultadoStr + resultado + ": ";
            }
            
            return resultadoStr;
        }

        private ObservableCollection<string> _comprometidosFuturos;

        private ObservableCollection<string> _comprometidosAnterior;

        private ObservableCollection<string> _tefs;

        #endregion Fields

        #region Properties
        public ObservableCollection<string> Datos
        {
            get
            {
                return _datos;
            }

            set
            {
                _datos = value;
            }
        }


        public ObservableCollection<string> VariablesDeControl
        {
            get
            {
                return _variablesDeControl;
            }

            set
            {
                _variablesDeControl = value;
            }
        }


        public ObservableCollection<VariableAP> VariablesEstado
        {
            get
            {
                return _variablesEstado;
            }

            set
            {
                _variablesEstado = value;
            }
        }


        public ObservableCollection<VariableAP> VariablesResultado
        {
            get
            {
                return _variablesResultado;
            }

            set
            {
                _variablesResultado = value;
            }
        }

        public ObservableCollection<EventoAP> EventosEaE
        {
            get
            {
                return _eventosEaE;
            }

            set
            {
                _eventosEaE = value;
            }
        }

        public ObservableCollection<string> Propios
        {
            get
            {
                return _propios;
            }

            set
            {
                _propios = value;
            }
        }

        public ObservableCollection<string> ComprometidosFuturos
        {
            get
            {
                return _comprometidosFuturos;
            }

            set
            {
                _comprometidosFuturos = value;
            }
        }

        public ObservableCollection<string> ComprometidosAnterior
        {
            get
            {
                return _comprometidosAnterior;
            }

            set
            {
                _comprometidosAnterior = value;
            }
        }

        public ObservableCollection<string> Tefs
        {
            get
            {
                return _tefs;
            }

            set
            {
                _tefs = value;
            }
        }



        #endregion Properties

        public XElement generarTagDeVariables()
        {

            return new XElement("variables",
               @" { ""variables"":" + collecctionString() + @"}"); ;
        }

        public string collecctionString()
        {
            return @"[{""nombre"":""Tf"",""valor"":""50000.0"", ""vector"":""False""},{""nombre"":""HV"",""valor"":""99999999.0"", ""vector"":""False""}"
                + tagVariablesDeEstado()
                + tagEventos()
                + tagResultados()
                + tagControl()
                + tagDatos() 
                + tagVectores()
                + @"]";
        }

        public string collecctionStringSinVectores()
        {
            return @"[{""nombre"":""Tf"",""valor"":""50000.0"", ""vector"":""False""},{""nombre"":""HV"",""valor"":""99999999.0"", ""vector"":""False""}"
                + tagVariablesDeEstado()
                + tagEventos()
                + tagResultados()
                + tagControl()
                + tagDatos()
                + @"]";
        }


        private string tagVectores()
        {
            List<VariableAP> variables = obtenerVariables(collecctionStringSinVectores());
            string tag = "";
            if (variables.Any(item => item.vector))
            {
                if(!variables.Any(item => item.nombre == "I")) { 
                    tag = @",{""nombre"":""I"",""valor"":""1.0"", ""vector"":""False""}";
                }
                if (!variables.Any(item => item.nombre == "I"))
                {
                    tag = tag + @", {""nombre"":""N"",""valor"":""10.0"", ""vector"":""False""}";
                }
                
            }
            
               return tag;
        }

        public List<VariableAP> obtenerVariables(string collectionString) {
            List<VariableAP> variables = JsonConvert.DeserializeObject<List<VariableAP>>(collectionString);
            return variables;
        }

        private string tagDatos()
        {
            return obtenerTagGeneral(Datos);
        }

        private string tagControl()
        {
            return obtenerTagGeneral(VariablesDeControl);
        }

        private string tagResultados()
        {
            return obtenerTagVariablesAP(VariablesResultado);
        }

        private string tagEventos()
        {
            if(Tipo.EaE.Equals(TipoDeEjercicio)) {
                return obtenerTagEventos(EventosEaE);
            } else { 
            return obtenerTagGeneral(Tefs);
            }
        }

        private string obtenerTagEventos(ObservableCollection<EventoAP> eventos)
        {
            var result = "";
            foreach (EventoAP item in eventos)
            {
                result = result + @",{ ""nombre"":""" + item.TEF + @""",""valor"":""0.0"", ""vector"":""" + item.Vector + @"""}";
            }
            var arrepentimiento = new VariableAP();
            arrepentimiento.nombre = "A";
            result = result + (eventos.Any(evento => evento.Arrepentimiento) ? @"," + JsonConvert.SerializeObject(arrepentimiento)  : "");
            return result;
        }

        private string tagVariablesDeEstado()
        {
            return obtenerTagVariablesAP(VariablesEstado);     
        }

        private string obtenerTagVariablesAP(ObservableCollection<VariableAP> variablesEstado)
        {
            var result = "";
            foreach (VariableAP item in variablesEstado)
            {
                result = result + @",{ ""nombre"":""" + item.nombre + @""",""valor"":""" + item.valor + @""", ""vector"":""" + item.vector + @"""}";
            }
            return result;
        }

        private string obtenerTagGeneral(ObservableCollection<string> collection)
        {
            var result = "";
            foreach (string item in collection)
            {
                result = result + @",{ ""nombre"":""" + item + @""",""valor"":""0.0"", ""vector"":""false""}";
            }
            return result;
        }


       
    }
}
