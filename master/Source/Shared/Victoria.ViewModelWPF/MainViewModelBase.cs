using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Practices.Prism.Commands;
using Victoria.Shared;
using Victoria.Shared.Prism;


namespace Victoria.ViewModelWPF
{
    public abstract class MainViewModelBase : NotificationObject
    {

        #region Fields

        private IList<StageViewModelBase> stages;
        private StageViewModelBase selectedStage;

        #endregion

        #region Properties
        public IList<StageViewModelBase> Stages
        {
            get { return stages; }
            set
            {
                stages = value;
                this.RaisePropertyChanged("Stages");
            }
        }

        public StageViewModelBase SelectedStage
        {
            get { return selectedStage; }
            set
            {
                selectedStage = value;
                this.RaisePropertyChanged("SelectedStage");
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Methods
     
        #endregion

    }
}
