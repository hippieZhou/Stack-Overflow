using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration.Pnp;
using Windows.System.Profile;

namespace UWPClassLibrary.Helpers
{
    /// <summary>
    /// 获取系统版本号
    /// </summary>
    public partial class DeviceVersionHelper
    {
        /// <summary>
        /// UWP版本获取方法
        /// </summary>
        /// <returns></returns>
        public static string UWPWay()
        {
            var sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            var v = ulong.Parse(sv);
            var v1 = (v & 0xFFFF000000000000L) >> 48;
            var v2 = (v & 0x0000FFFF00000000L) >> 32;
            var v3 = (v & 0x00000000FFFF0000L) >> 16;
            var v4 = (v & 0x000000000000FFFFL);
            return $"{v1}.{v2}.{v3}.{v4}";
        }


        /// <summary>
        /// UAP版本获取方法
        /// </summary>
        /// <returns></returns>
        public static async Task<string> UAPWay()
        {
            const string DeviceDriverVersionKey = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3";
            var hal = await GetHalDevice(DeviceDriverVersionKey);
            if (hal == null || !hal.Properties.ContainsKey(DeviceDriverVersionKey))
                return null;

            var versionParts = hal.Properties[DeviceDriverVersionKey].ToString().Split('.');
            return string.Join(".", versionParts.Take(2).ToArray());
        }
        private static async Task<PnpObject> GetHalDevice(params string[] properties)
        {
            const string DeviceClassKey = "{A45C254E-DF1C-4EFD-8020-67D146A850E0},10";
            const string RootContainer = "{00000000-0000-0000-FFFF-FFFFFFFFFFFF}";
            const string RootQuery = "System.Devices.ContainerId:=\"" + RootContainer + "\"";
            const string HalDeviceClass = "4d36e966-e325-11ce-bfc1-08002be10318";

            var actualProperties = properties.Concat(new[] { DeviceClassKey });
            var rootDevices = await PnpObject.FindAllAsync(PnpObjectType.Device,
                actualProperties, RootQuery);

            foreach (var rootDevice in rootDevices.Where(d => d.Properties != null && d.Properties.Any()))
            {
                var lastProperty = rootDevice.Properties.Last();
                if (lastProperty.Value != null)
                    if (lastProperty.Value.ToString().Equals(HalDeviceClass))
                        return rootDevice;
            }
            return null;
        }

        /// <summary>
        /// SL办不办获取方法
        /// </summary>
        /// <returns></returns>
        public static string SLWay()
        {
            /*
             * Environment.OSVersion
             */
            return string.Empty;
        }
    }
}
