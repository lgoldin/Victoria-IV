using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using Victoria.DesktopApp.View;
using Victoria.UI.SharedWPF;
using Victoria.ViewModelWPF;

namespace Victoria.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            if (e.Args.Any() && e.Args[0].IndexOf(".vic", System.StringComparison.Ordinal) > 0)
            {
                var simulationXmlURI = e.Args[0];
                var mainWindow = new MainWindow(simulationXmlURI, false);
                mainWindow.Show();
            }
            else
            {
                var welcomeView = new WelcomeView();
                if (welcomeView.ShowDialog() == false)
                {
                    Application.Current.Shutdown(0);
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception realerror = e.Exception;
            while (realerror.InnerException != null)
                realerror = realerror.InnerException;
            MessageBox.Show(realerror.Message);
        }

    }
}
