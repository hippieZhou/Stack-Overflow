using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace UWPClassLibrary.Helpers
{
    public sealed class SettingsHelper
    {
        /// <summary>
        /// 应用视图相关设置
        /// </summary>
        public static ApplicationView ApplicationView => ApplicationView.GetForCurrentView();

        /// <summary>
        /// 虚拟键盘相关设置
        /// </summary>
        public static InputPane InputPane => InputPane.GetForCurrentView();
    }
}
