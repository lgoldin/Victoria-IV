using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Victoria.Shared.Mocks
{
    public class DiagramMock : Diagram
    {
        #region Methods

        public override Node Execute(IList<Variable> variables)
        {
            try
            {
                Random rnd = new Random();
                for (int i = 0; i < 300; i++)
                {
                    for (int z = 0; z < 1000000; z++) { }
                    variables.First(x => x.Name == "T").ActualValue++;
                    variables.First(x => x.Name == "TEST").ActualValue = rnd.Next(1, 100);
                    variables.First(x => x.Name == "TEST2").ActualValue = rnd.Next(1, 100);
                    variables.First(x => x.Name == "TEST3").ActualValue = rnd.Next(1, 100);
                    variables.First(x => x.Name == "TEST4").ActualValue = rnd.Next(1, 100);
                    variables.First(x => x.Name == "TEST5").ActualValue = rnd.Next(1, 100);
                }
                return new Node();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
