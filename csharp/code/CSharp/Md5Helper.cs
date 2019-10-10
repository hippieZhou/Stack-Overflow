using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFClassLibrary.Helpers
{
    public class Md5Helper
    {
        private static Md5Helper _current;

        public static Md5Helper Current
        {
            get { return _current ?? (_current = new Md5Helper()); }
        }
        private Md5Helper() { }

        public string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
