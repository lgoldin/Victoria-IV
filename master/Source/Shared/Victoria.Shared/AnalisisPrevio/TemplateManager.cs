using System;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Generic;



namespace Victoria.Shared.AnalisisPrevio
{
    public class TemplateManager
    {
        public String obtenerContent(AnalisisPrevio analisisPrevio, string s)
        {
           
            foreach(var item in obtenerPlaceholders(analisisPrevio))
            {
                s = s.Replace(item.Key, item.Value);
            }

            return s;

        }

        public string obtenerTemplate(AnalisisPrevio analisisPrevio)
        {
            var template = (analisisPrevio.TipoDeEjercicio == AnalisisPrevio.Tipo.DeltaT ? @"diagramadT.xml" : @"diagramaEaE.xml");
            if(analisisPrevio.TipoDeEjercicio == AnalisisPrevio.Tipo.EaE) { 
                var eventoLlegada = analisisPrevio.evento("Llegada");
                var eventoSalida = analisisPrevio.evento("Salida");
                template = eventoLlegada != null && eventoLlegada.Arrepentimiento ? @"diagramaEaEArrepentimientoLlegada.xml": template;
                template = eventoSalida != null && eventoSalida.Arrepentimiento ? @"diagramaEaEArrepentimientoSalida.xml" : template;
                template = eventoLlegada != null && eventoSalida != null && eventoSalida.Arrepentimiento && eventoLlegada.Arrepentimiento ? @"diagramaEaEArrepentimientoLlegadaSalida.xml" : template;
            } else
            {
                template = analisisPrevio.evento("TPLL", analisisPrevio.Tefs) == null? @"diagramadTSinTPLL.xml" : template;
                template = analisisPrevio.evento("Llegada", analisisPrevio.ComprometidosAnterior) == null ? @"diagramadTSinLlegada.xml" : template;
                template = analisisPrevio.evento("TPLL", analisisPrevio.Tefs) == null &&
                            analisisPrevio.evento("Llegada", analisisPrevio.ComprometidosAnterior) == null? @"diagramadTSinLlegadaSinTPLL.xml" : template;
            }
            return template;
        }

        public Dictionary<string, string> obtenerPlaceholders(AnalisisPrevio analisisPrevio) {
            var map = new Dictionary<string, string>();
            if(analisisPrevio.TipoDeEjercicio == AnalisisPrevio.Tipo.EaE) { 
                EventoAP eventoLlegada = analisisPrevio.evento("Llegada");
                EventoAP eventoSalida = analisisPrevio.evento("Salida");
                map.Add("eventoLlegada", eventoLlegada == null || eventoLlegada.TEF == "" ? "???" : eventoLlegada.TEF);
                map.Add("condicionLlegada", eventoLlegada == null || eventoLlegada.Condicion == null || eventoLlegada.Condicion == "" ? "???": eventoLlegada.Condicion);
                map.Add("encadenadorLlegada", eventoLlegada == null || eventoLlegada.Encadenador == null ? "???": eventoLlegada.Encadenador);
                map.Add("eventoSalida", eventoSalida == null || eventoSalida.TEF == "" ? "???": eventoSalida.TEF);
                map.Add("condicionSalida", eventoSalida == null || eventoSalida.Condicion == null || eventoSalida.Condicion == "" ? "???" : eventoSalida.Condicion);  
                map.Add("encadenadorSalida", eventoSalida == null || eventoSalida.Encadenador == null ? "???" : eventoSalida.Encadenador);
            }
            map.Add("resultados", analisisPrevio.resultados());
            return map;
        }
    }
}
