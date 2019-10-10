using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWPClassLibrary.Helpers
{
    public class TreeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">要查找的控件类型</typeparam>
        /// <param name="parentElement">父容器</param>
        /// <returns>当前容器中查找的第一个符合要求的控件</returns>
        public static T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
            {
                return null;
            }
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);
                var tree = child as T;
                if (tree != null)
                {
                    return tree;
                }
                var result = FindFirstElementInVisualTree<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断当前控件是否在可视区域内
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsInCurrentView(UIElement child, UIElement parent)
        {
            var generalTransform = child.TransformToVisual(parent);
            var point = generalTransform.TransformPoint(new Point(0, 0));
            var rect = ApplicationView.GetForCurrentView().VisibleBounds;
            return point.X <= rect.Width && point.Y <= rect.Height && child.Visibility == Visibility.Visible;
        }
    }
}
