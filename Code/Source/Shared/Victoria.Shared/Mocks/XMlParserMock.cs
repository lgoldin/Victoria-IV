using System.Collections.Generic;

namespace Victoria.Shared.Mocks
{
    public static class XMlParserMock
    {
        public static Simulation GetSimulation(string xmlString)
        {

            var diagram = new DiagramMock();
            var variables = new Dictionary<string, Variable>();
            variables.Add("T", new Variable() { Name = "T", ActualValue = 0 , InitialValue = 0});
            variables.Add("TEST", new Variable() { Name = "TEST", ActualValue = 0, InitialValue = 0 });
            variables.Add("TEST2", new Variable() { Name = "TEST2", ActualValue = 0, InitialValue = 0 });
            variables.Add("TEST3", new Variable() { Name = "TEST3", ActualValue = 0, InitialValue = 0 });
            variables.Add("TEST4", new Variable() { Name = "TEST4", ActualValue = 0, InitialValue = 0 });
            variables.Add("TEST5", new Variable() { Name = "TEST5", ActualValue = 0, InitialValue = 0 });
            var simulation = new Simulation(diagram, variables);

            return simulation;
        }
    }
}
