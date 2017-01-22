using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria.ModelSL;

namespace Vitoria.ModelSL
{
    public class AnimationConfigurationBase
    {
        public IEnumerable<Variable> Variables { get; set; }

        public string SelectedVariable { get; set; }

        public string AnimationName { get; set; }

        public string AnimationType { get; set; }

    }
}
