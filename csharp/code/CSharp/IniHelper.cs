using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFClassLibrary.Helpers
{
    class IniHelper
    {
    }

    public class IniReader
    {
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In, Out] char[] lpReturnedString, uint nSize, string lpFileName);

        /// <summary>
        ///  获取所有节点名称（section）
        /// </summary>
        /// <param name="lpszReturnedBuffer">存放节点名称的内存地址，每个节点之间用\0分隔</param>
        /// <param name="nSize">内存大小</param>
        /// <param name="lpFileName">ini文件</param>
        /// <returns>内容的实际长度，为0表示没有内容为nSize -2表示内存大小不够</returns>
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnedBuffer, uint nSize, string lpFileName);

        private static string ReadString(string section, string key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(1024);
            try
            {
                GetPrivateProfileString(section, key, def, temp, 1024, filePath);
            }
            catch
            { }
            return temp.ToString();
        }

        /// <summary>
        ///  获取所有的节点的名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] ReadInAllSectionNames(string filePath)
        {
            uint MAX_BUFFER = 32767;
            string[] sections = new string[0];  //返回值
            IntPtr pReturnString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char)); //申请内存
            UInt32 byteReturn = GetPrivateProfileSectionNames(pReturnString, MAX_BUFFER, filePath);
            if (byteReturn != 0)
            {
                string returnString = Marshal.PtrToStringAuto(pReturnString, (int)byteReturn);
                sections = returnString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            Marshal.FreeCoTaskMem(pReturnString);
            return sections;
        }
        /// <summary>
        ///  获取指定节点的所有的key和value值
        /// </summary>
        /// <param name="section">节点</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string[] ReadInAllSectionContent(string section, string filePath)
        {
            UInt32 MAX_BUFFER = 32767;

            string[] items = new string[0];

            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));

            UInt32 byteReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, filePath);
            if (!(byteReturned == MAX_BUFFER - 2) || byteReturned == 0)
            {
                string returnString = Marshal.PtrToStringAuto(pReturnedString, (int)byteReturned);
                items = returnString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            Marshal.FreeCoTaskMem(pReturnedString);
            return items;
        }
        /// <summary>
        ///  获取指定节点下面所有的key值
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string[] ReadInAllKeys(string section, string filePath)
        {
            string[] items = new string[0];
            const int SIZE = 2014 * 10;
            char[] chars = new char[SIZE];

            uint byteReturned = GetPrivateProfileString(section, null, null, chars, SIZE, filePath);
            if (byteReturned != 0)
            {
                items = new string(chars).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            chars = null;
            return items;
        }


        /// <summary>
        ///  获取指定文件的指定节点的指定key的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultStr"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadInStringValue(string section, string key, string defaultStr, string filePath)
        {
            string result = defaultStr;
            const int SIZE = 1024 * 10;
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称", "section");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("必须指定键名称key", "section");
            }
            StringBuilder temp = new StringBuilder(SIZE);
            UInt32 byteReturned = (UInt32)GetPrivateProfileString(section, key, defaultStr, temp, SIZE, filePath);
            if (byteReturned != 0)
            {
                result = temp.ToString();
            }
            return result;
        }
    }

    public class IniWriter
    {
        /// <summary>
        ///  将指定的键值对写入指定的节点，如果存在则替换
        /// </summary>
        /// <param name="lpSection">节点名称</param>
        /// <param name="lpKeyValues">Item键值对，多个用\0分开，key1=value1\0key2=value2</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileSection(string lpSection, string lpKeyValues, string filePath);
        /// <summary>
        ///  在ini文件中，将指定的键值对写到指定的节点，如果没有此节点则创建，如果存在则替换
        /// </summary>
        /// <param name="section">节点名称，如果不存在此节点则创建此节点</param>
        /// <param name="items">键值对，多个用\0分开，key1=value1\0key2=value2</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IniWriteItems(string section, string items, string filePath)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称", "section");
            }
            if (string.IsNullOrEmpty(items))
            {
                throw new ArgumentException("必须指定键值对", "keyValues");
            }
            return WritePrivateProfileSection(section, items, filePath);
        }
        /// <summary>
        ///  将指定的值写入指定的节点的键中
        /// </summary>
        /// <param name="lpSection">节点名称</param>
        /// <param name="lpKey">键名称：如果为null则移除指定的节点及其所有的项目</param>
        /// <param name="lpValue">值：如果为null则删除指定节点中指定的键</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpSection, string lpKey, string lpValue, string filePath);
        /// <summary>
        ///  将指定的值写入指定的节点的键中
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="keyStr">键值</param>
        /// <param name="valueStr">值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool IniWriteItems(string section, string keyStr, string valueStr, string filePath)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException("必须指定节点名称", "section");
            }
            if (string.IsNullOrEmpty(keyStr))
            {
                throw new ArgumentException("必须指定键", "key");
            }
            if (string.IsNullOrEmpty(valueStr))
            {
                throw new ArgumentException("必须指定值", "value");
            }
            return WritePrivateProfileString(section, keyStr, valueStr, filePath);
        }
    }
}
