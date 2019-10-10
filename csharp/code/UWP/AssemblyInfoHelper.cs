using System.Reflection;

namespace UWPClassLibrary.Helpers
{
    /// <summary>
    /// http://validvoid.net/windows-store-app-get-assembly-version/
    /// Windows Store 应用中获取程序集版本号的方法
    /// </summary>
    public sealed class AssemblyInfoHelper
    {
        /// <summary>
        /// 获取程序集版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            #region 方法1
            /*限制条件:
            一是要求知道程序集的完整显示名称；
            二是程序集输出类型必须是“类库”(Class Library)。
            */
            //return Assembly.Load(new AssemblyName("UWPCore.AssemblyInfoCore")).GetName().Version.ToString();

            #endregion

            #region 方法2
            /*方法二在程序集输出类型为类库或者 Windows 运行时组件时均可用，
            其限制条件是需知道程序集中的一个类，再通过 typeof(MyUtils).GetTypeInfo().Assembly 取得目标程序集，进而获得程序集的版本号。
            */

            return typeof(AssemblyInfoHelper).GetTypeInfo().Assembly.GetName().Version.ToString();

            #endregion
        }

        /// <summary>
        /// 获得程序集文件版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyFileVersion()
        {
            return typeof(AssemblyInfoHelper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        }
    }
}
