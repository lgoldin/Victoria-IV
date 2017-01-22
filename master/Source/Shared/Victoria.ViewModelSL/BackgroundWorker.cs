using System;
using System.ComponentModel;
using System.Net;
using System.Windows;

namespace Victoria.ViewModelSL
{
    public static class BackgroundWorker
    {
        public static void RunInBackGround(DoWorkEventHandler dowork, RunWorkerCompletedEventHandler completed = null)
        {
            Action workAction = delegate
            {
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += dowork;
                worker.RunWorkerCompleted += completed;
                worker.RunWorkerAsync();
            };
            workAction.Invoke();
        }
    }
}
