using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Victoria.DesktopApp.Behavior
{
    public static class FindAncestor
    {
        public static T FindAncestorMethod<T>(this DependencyObject obj)
    where T : DependencyObject
        {
            DependencyObject tmp = VisualTreeHelper.GetParent(obj);
            while (tmp != null && !(tmp is T))
            {
                tmp = VisualTreeHelper.GetParent(tmp);
            }
            return tmp as T;
        }
    }
}
