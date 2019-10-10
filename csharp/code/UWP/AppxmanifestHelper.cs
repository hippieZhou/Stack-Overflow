using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.Storage;

namespace UWPClassLibrary.Helpers
{
    public class AppxmanifestHelper
    {
        public static string FileName = "AppxManifest.xml";
        private static AppxmanifestHelper _current;
        public static AppxmanifestHelper Current => _current ?? (_current = new AppxmanifestHelper());

        public AppxmanifestHelper() { }

        private static async Task<XmlNodeList> InitAppxmanifestFile()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync(FileName);
            var xml = await FileIO.ReadTextAsync(file);
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.ChildNodes[1].ChildNodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetIdentity()
        {
            try
            {
                var nodes = await InitAppxmanifestFile();
                return nodes[1].Attributes.Cast<XmlAttribute>().ToDictionary(attr => attr.Name, attr => attr.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Dictionary<string, string>();
            }
        }
        public async Task<Dictionary<string, string>> GetPhoneIdentity()
        {
            try
            {
                var nodes = await InitAppxmanifestFile();
                return nodes[2].Attributes.Cast<XmlAttribute>().ToDictionary(attr => attr.Name, attr => attr.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Dictionary<string, string>();
            }
        }
        public async Task<Dictionary<string, string>> GetProperties()
        {
            try
            {
                var nodes = await InitAppxmanifestFile();
                return nodes[3].ChildNodes.Cast<XmlNode>().ToDictionary(node => node.Name, node => node.InnerText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Dictionary<string, string>();
            }
        }
        public async Task<List<string>> GetCapabilities()
        {
            try
            {
                var list = new List<string>();

                var nodes = await InitAppxmanifestFile();
                for (var i = 0; i < nodes[7].ChildNodes.Count; i++)
                {
                    list.Add(nodes[7].ChildNodes[i].Attributes["Name"].Value);
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<string>();
            }
        }
    }
}
