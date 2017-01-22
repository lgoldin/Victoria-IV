using NUnit.Framework;
using System.IO;
using Victoria.Shared;

namespace Victoria.tests
{
	[TestFixture ()]
	public class ParserTest
	{
		string xml;

		[SetUp]
		public void setUp()
		{
			this.xml = File.ReadAllText ("../../xml/XMLPrueba.xml");
		}

		[Test ()]
		public void TestBasico ()
		{
			
			Simulation s = XMLParser.GetSimulation (this.xml);
			Assert.That (s, Is.InstanceOf<Simulation>());
		}
	}
}

