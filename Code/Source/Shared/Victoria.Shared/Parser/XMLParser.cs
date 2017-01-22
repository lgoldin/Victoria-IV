using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Victoria.Shared.Parser;


namespace Victoria.Shared
{
    public static class XMLParser
    {
        public static Simulation GetSimulation(string xmlString)
        {
            try
            {
                var doc = XElement.Parse(xmlString);

                var diagramasPreProcesados = doc.Descendants("Diagrama").Select(diag => new PreParsedDiagram
                {
                    name = diag.Attribute("Name").Value,
                    diagram = new Diagram
                    {
                        Name = diag.Attribute("Name").Value
                    },
                    nodos = parseNodes(diag.Descendants("flowchart").Descendants("block"))
                });

                var variables = parseVariables(doc.Descendants("variables").First());

                List<Diagram> diagramas = posprocesarDiagramas(diagramasPreProcesados);

                foreach (var diagram in diagramas)
                {
                    foreach (var node in diagram.Nodes)
                    {
                        if (node is NodeDiagram)
                        {
                            ((NodeDiagram)node).Diagram = diagramas.First(X => X.Name == ((NodeDiagram)node).DiagramName);
                            if ((node as NodeDiagram).IsInitializer)
                            {
                                (node as NodeDiagram).Diagram.Execute(variables.Values.ToList());
                            }
                        }
                    }
                }

                foreach (var variable in variables)
                {
                    variable.Value.InitialValue = variable.Value.ActualValue;
                    variable.Value.ActualValue = 0;
                }

                List<Stage> stages;
                if(doc.Descendants("Stages").Count() > 0)
                    stages = parseStages(doc.Descendants("Stages").First());
                else
                    stages = new List<Stage>();

                return new Simulation(diagramas, variables, stages);
            }
            catch (Exception e)
            {                
                throw new ParsingException("Error al parsear la simulacion",e);
            }
        }

        static Dictionary<string, PreParsedNode> parseNodes(IEnumerable<XElement> XmlNodes)
        {
            return XmlNodes.Select(node => parseBlock(node)).ToDictionary(node => node.name);
        }

        static PreParsedNode parseBlock(XElement node)
        {
            switch (node.Attribute("type").Value)
            {
                case "nodo_titulo_inicializador":
                    return parseNodoInicializador(node);
                case "nodo_sentencia":
                    return parseNodoSentencia(node);
                case "nodo_iterador":
                    return parseNodoIterador(node);
                case "nodo_fin":
                    return parseNodoFin(node);
                case "nodo_condicion":
                    return parseNodoCondicion(node);
                case "nodo_inicializador":
                    return parseNodoDiagrama(node, true);
                case "nodo_diagrama":
                    return parseNodoDiagrama(node, false);
                case "nodo_condicion_cierre":
                    return parseNodoCondicionCierre(node);
                case "nodo_referencia":
                    return parseNodoReferencia(node);
                case "nodo_resultado":
                    return parseNodoResultado(node);
                case "nodo_titulo_diagrama":
                    return parseNodoTituloDiagrama(node);
                case "nodo_random":
                    return parseNodoRandom(node);
                default:
                    throw new XMLFormatError("tipo de nodo desconocido");
            }
        }

        static Dictionary<string, Variable> parseVariables(XElement node)
        {
            JObject vars = JObject.Parse(node.Value);
            var result = new Dictionary<string, Variable>();
            foreach (var variable in vars["variables"])
            {
                if (!result.ContainsKey((string)variable["nombre"]))
                {
                    var nombre = (string)variable["nombre"];
                    if (nombre.Contains("(") && nombre.Contains(")"))
                    {
                        var indexFirstParenthesis = nombre.LastIndexOf('(');
                        var indexLastParenthesis = nombre.LastIndexOf(')');
                        var nombreItem = nombre.Remove(indexFirstParenthesis, nombre.Length - indexFirstParenthesis);
                        var lenght = Convert.ToInt32(nombre.Substring(indexFirstParenthesis + 1, indexLastParenthesis - indexFirstParenthesis - 1));

                        var variableList = new List<Variable>();
                        for (int i = 1; i <= lenght; i++)
                        {
                            var variableItem = new Variable
                            {
                                Name = new StringBuilder(nombreItem).Append('(').Append(i).Append(')').ToString(),
                                InitialValue = (double)variable["valor"]
                            };
                            variableList.Add(variableItem);
                        }
                        //es vector
                        result.Add(nombre, new VariableArray()
                        {
                            Name = nombre,
                            Variables = variableList
                        });

                    }
                    else
                    {
                        result.Add(nombre, new Variable
                        {
                            Name = nombre,
                            InitialValue = (double)variable["valor"]
                        });
                    }
                }
            }

            return result;
        }

        static IList<string> getNextNodes(XElement node)
        {
            return node.Descendants("connection").Select(con => con.Attribute("ref").Value).ToList();
        }

        private static PreParsedNode parseNodoIterador(XElement node)
        {
            var ns = new NodeIterator();
            var code = node.Attribute("caption").Value.Trim();
            code = code.Replace(" ", string.Empty);
            ns.Name = node.Attribute("id").Value;
            var parametros = code.Split(';');
            ns.ValorInicial = Convert.ToInt32(parametros[0]);
            ns.ValorFinal = Convert.ToInt32(parametros[1]);
            ns.Incremento = Convert.ToInt32(parametros[2]);
            ns.VariableName = (parametros.Count() > 3) ? parametros[3] : string.Empty;
            return new PreParsedNodeIterator()
            {
                name = ns.Name,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoSentencia(XElement node)
        {
            var ns = new NodeSentence();
            ns.Code = node.Attribute("caption").Value;
            ns.Name = node.Attribute("id").Value;
            return new PreParsedNode
            {
                name = ns.Name,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoInicializador(XElement node)
        {
            var ns = new Node
            {
                Name = node.Attribute("id").Value,
            };
            return new PreParsedNode
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoFin(XElement node)
        {
            var ns = new Node
            {
                Name = node.Attribute("id").Value,
            };
            return new PreParsedNode
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = null
            };
        }

        static PreParsedNode parseNodoDiagrama(XElement node, bool isInitializer)
        {
            var ns = new NodeDiagram
            {
                Name = node.Attribute("id").Value,
                DiagramName = node.Attribute("caption").Value,
                IsInitializer = isInitializer

            };
            return new PreparsedNodeDiagram
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = getNextNodes(node)

            };
        }

        static PreParsedNode parseNodoCondicion(XElement node)
        {
            var ns = new NodeCondition
            {
                Name = node.Attribute("id").Value,
                Code = node.Attribute("caption").Value
            };
            return new PreParsedNodeCondition
            {
                name = ns.Name,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoCondicionCierre(XElement node)
        {
            var ns = new Node
            {
                Name = node.Attribute("id").Value,
            };
            return new PreparsedNodeEndCondition
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoReferencia(XElement node)
        {
            var ns = new NodeReferencia();
            ns.Code = node.Attribute("caption").Value;
            ns.Name = node.Attribute("id").Value;
            return new PreParsedNodeReferencia
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoResultado(XElement node)
        {
            var ns = new NodeResult
            {
                Name = node.Attribute("id").Value,
                Variables = node.Attribute("caption").Value.Split(':')
            };
            return new PreParsedNode
            {
                name = ns.Name,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoTituloDiagrama(XElement node)
        {
            var ns = new Node
            {
                Name = node.Attribute("id").Value,
            };
            return new PreParsedNode
            {
                name = node.Attribute("id").Value,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static PreParsedNode parseNodoRandom(XElement node)
        {
            var ns = new NodeRandom
            {
                Name = node.Attribute("id").Value,
                Code = node.Attribute("caption").Value
            };
            return new PreParsedNode
            {
                name = ns.Name,
                node = ns,
                next = getNextNodes(node)
            };
        }

        static List<Diagram> posprocesarDiagramas(IEnumerable<PreParsedDiagram> diagramasPreProcesados)
        {
            var result = new List<Diagram>();
            foreach (PreParsedDiagram preD in diagramasPreProcesados)
            {
                preD.diagram.Nodes = procesarNodos(preD.nodos, diagramasPreProcesados);
                result.Add(preD.diagram);
            }
            return result;
        }

        static ObservableCollection<Node> procesarNodos(Dictionary<string, PreParsedNode> nodos, IEnumerable<PreParsedDiagram> diagramasPreProcesados)
        {
            var result = new List<Node>();
            foreach (var nodoPreProcesado in nodos.Values)
            {
                nodoPreProcesado.posprocesar(nodos, result);
            }

            return new ObservableCollection<Node>(result);
        }

        static List<Stage> parseStages(XElement stages)
        {
            return stages.Descendants("Stage").Select(st => new Stage
            {
                Name = st.Attribute("Name").Value,
                Variables = st.Elements("Variable").Select(v => new Variable
                {
                    Name = v.Attribute("Name").Value,
                    InitialValue = Convert.ToDouble(v.Attribute("Value").Value)
                }).ToList(),
                Charts = st.Descendants("Chart").Select(c => new Chart
                {
                    Name = c.Attribute("Name").Value,
                    DependentVariables = c.Descendants("Variable").Select(v => v.Attribute("Name").Value).ToList()
                }).ToList()
            }).ToList();
        }
    }
}
