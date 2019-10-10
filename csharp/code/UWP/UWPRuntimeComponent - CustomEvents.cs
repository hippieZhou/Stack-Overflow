using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace UWPRuntimeComponent.Extensions
{
    public sealed class CustomEvents
    {
        //https://msdn.microsoft.com/zh-cn/library/dn169426.aspx
        //https://msdn.microsoft.com/en-us/library/hh972883.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1

        private EventRegistrationTokenTable<EventHandler<string>> _mStatusChangedTokenTable;
        public event EventHandler<string> StatusChanged
        {
            add
            {
                return EventRegistrationTokenTable<EventHandler<string>>
                    .GetOrCreateEventRegistrationTokenTable(ref _mStatusChangedTokenTable)
                    .AddEventHandler(value);
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<string>>
                    .GetOrCreateEventRegistrationTokenTable(ref _mStatusChangedTokenTable)
                    .RemoveEventHandler(value);
            }
        }
        internal void OnStatusChanged(string code)
        {
            var temp =
                EventRegistrationTokenTable<EventHandler<string>>
                .GetOrCreateEventRegistrationTokenTable(ref _mStatusChangedTokenTable)
                .InvocationList;
            temp?.Invoke(this, code);
        }
    }
}
