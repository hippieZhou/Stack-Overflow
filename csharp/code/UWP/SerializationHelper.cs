using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UWPClassLibrary.Helpers
{
    public class SerializationHelper
    {
        #region JSON 序列化
        public static string Serialize<T>(T obj)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(obj.GetType());
                    serializer.WriteObject(stream, obj);
                    var json = stream.ToArray();
                    return Encoding.UTF8.GetString(json, 0, json.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public static T Deserialize<T>(string json) where T : class
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default(T);
            }
        }
        #endregion

        #region XML 序列化
        //public static void Serialize<T>(T o, string filePath)
        //{
        //    try
        //    {
        //        XmlSerializer formatter = new XmlSerializer(typeof(T));
        //        StreamWriter sw = new StreamWriter(filePath, false);
        //        formatter.Serialize(sw, o);
        //        sw.Flush();
        //        sw.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //}
        //public static T DeSerialize<T>(string filePath)
        //{
        //    try
        //    {
        //        XmlSerializer formatter = new XmlSerializer(typeof(T));
        //        StreamReader sr = new StreamReader(filePath);
        //        T o = (T)formatter.Deserialize(sr);
        //        sr.Close();
        //        return o;
        //    }
        //    catch (Exception e)
        //    {
        //        return default(T);
        //    }
        //}
        #endregion
    }
}
