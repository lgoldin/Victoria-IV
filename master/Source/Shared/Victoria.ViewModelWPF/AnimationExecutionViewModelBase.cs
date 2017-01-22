using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Victoria.Shared.Prism;

namespace Victoria.ViewModelWPF
{
    public abstract class AnimationExecutionViewModelBase : NotificationObject
    {
        protected DelegateCommand executeAnimationsCommand;
        protected DelegateCommand stopAnimationsCommand;

        public ICommand ExecuteAnimationsCommand
        {
            get
            {
                return this.executeAnimationsCommand;
            }
        }
        public ICommand StopAnimationsCommand
        {
            get
            {
                return this.stopAnimationsCommand;
            }
        }

        public AnimationExecutionViewModelBase()
        {

        }
    }
}
