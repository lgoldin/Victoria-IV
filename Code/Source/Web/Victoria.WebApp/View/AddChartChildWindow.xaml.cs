using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Victoria.WebApp.View
{
    public partial class AddChartChildWindow : ChildWindow
    {
        private string chartName;
        public string ChartName
        {
            get { return this.chartName; }
            set { this.chartName = value; }
        }

        public AddChartChildWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ThisChildWindow_Loaded);
        }

        private void BtnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        bool isDragging = false;
        Point offset;

        private FrameworkElement root;
        private FrameworkElement chrome;

        private void ThisChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(this) == 0)
            {
                Dispatcher.BeginInvoke(() => ThisChildWindow_Loaded(this, e));
                return;
            }
            root = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
            chrome = root.FindName("Chrome") as FrameworkElement;
            chrome.Visibility = Visibility.Collapsed;

        }
    }
}

