using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPClassLibrary.Extensions
{
    /// <summary>
    /// 自定义事件
    /// https://msdn.microsoft.com/zh-cn/library/aa288460(v=vs.71).aspx
    /// </summary>
    public class CustomEvents
    {
        //声明事件   若要在类内声明事件，首先必须声明该事件的委托类型（如果尚未声明的话）。 
        public delegate void ChangedEventHandler(object sender, EventArgs args);

        //声明事件本身(自定义)
        //public event ChangedEventHandler Changed;

        //声明事件本身(使用内置委托类型)
        public event EventHandler Changed;

        //*****注意：当和UI线程相关联时，不要在构造函数中进行初始化，要在Loaded进行初始化
        public CustomEvents()
        {
            //在合适的位置调用该事件
            this.Changed?.Invoke(null, null);
        }

        //SendOrPostCallback
    }
}
