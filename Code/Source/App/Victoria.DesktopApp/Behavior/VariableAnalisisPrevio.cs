using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victoria.DesktopApp.Behavior
{
    public class VariablesAnalisisPrevio
    {
        public List<VariableAnalisisPrevio> variables { get; set; }
    }

    public class VariableAnalisisPrevio
    {
        public string nombre { get; set; }

        public string valor { get; set; }
    }
}
