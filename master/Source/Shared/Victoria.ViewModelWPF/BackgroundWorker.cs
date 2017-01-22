using System;
using System.ComponentModel;

namespace Victoria.ViewModelWPF
{
    public static class BackgroundWorker
    {
        public static void RunInBackGround(DoWorkEventHandler dowork, RunWorkerCompletedEventHandler completed = null)
        {
            Action workAction = delegate
            {
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += dowork;
                worker.RunWorkerCompleted += completed;
                worker.RunWorkerAsync();
            };
            workAction.Invoke();
        }
    }
}
