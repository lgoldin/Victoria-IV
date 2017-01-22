using System.Collections.Generic;
using System.Windows.Controls;

namespace Victoria.ModelWPF
{
    public class AnimationConfigurationBase 
    {
        public IEnumerable<Variable> Variables { get; set; }

        public string AnimationName { get; set; }

        public string AnimationType { get; set; }

        public List<StackPanel> DllExtraConfigurations { get; set; }
        public AnimationConfigurationBase(List<Variable> variables)
        {
            Variables = variables;
        }
        public virtual void BindProperties()
        {

        }
    }
}
