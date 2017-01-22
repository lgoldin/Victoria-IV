using System;
using System.IO;
using Victoria.Shared;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Victoria.Consola
{
	class Options {
		[Option('v', "verbose", DefaultValue = false,
			HelpText = "Imprime los valores mientras ejecuta.")]
		public bool Verbose { get; set; }

		[CommandLine.ValueList(typeof(List<string>))]
		public List<string> InputFile { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage() {
			return HelpText.AutoBuild(this,
				(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}

	class MainClass
	{
		static Simulation simu;

		public static void Main (string[] args)
		{
			var options = new Options();

			if (!CommandLine.Parser.Default.ParseArguments (args, options)) {
				return;
			}
			if (!File.Exists(options.InputFile[0]))
			{
				Console.WriteLine ("Debe especificar el archivo xml a simular");
				return;
			}

			try {
				string xml = File.ReadAllText(options.InputFile[0]);

				simu = XMLParser.GetSimulation (xml);

				if(options.Verbose)
				{
					left = 0;
					top = Console.CursorTop;
					var tiempo = simu.Variables.Find ((variable) => variable.Name == "T");
					tiempo.ValueChanged += MostrarValoresEvento;
				}

				simu.Execute ();
				MostrarValores ();
			} catch (Exception e) {
				Console.WriteLine ("Excepcion al tratar de abrir el archivo:" + e.Message);
				return;
			}
		}
			
		static int left;
		static int top;

		static void MostrarValoresEvento (object sender, Victoria.Shared.EventArgs.VariableValueChangeEventArgs e)
		{
			MostrarValores ();
		}

		static void MostrarValores ()
		{
			var t = simu.Variables.Find (variable => variable.Name == "T");
			var tf = simu.Variables.Find (variable => variable.Name == "Tf");
			var porcentaje = t.ActualValue * 100 / tf.ActualValue;

			Console.Clear ();
			Console.SetCursorPosition (left, top);
			Console.WriteLine ("Procesando({0}%)",(int)porcentaje);
			simu.Variables.ForEach (variable => Console.WriteLine ("{0}: {1} ", variable.Name, variable.ActualValue));

		}
	}
}
