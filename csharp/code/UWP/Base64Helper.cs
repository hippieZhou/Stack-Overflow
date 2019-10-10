using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPClassLibrary.Helpers
{
    public class Base64Helper
    {
        public static string DecodeBase64(string code)
        {
            try
            {
                if (code == null)
                {
                    return string.Empty;
                }
                else
                {
                    var encoding = Encoding.UTF8;
                    var bytes = Convert.FromBase64String(code);
                    return encoding.GetString(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static string EncodeBase64(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return string.Empty;
                }
                else
                {
                    var bytes = Encoding.GetEncoding(Encoding.UTF8.WebName).GetBytes(code);
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
