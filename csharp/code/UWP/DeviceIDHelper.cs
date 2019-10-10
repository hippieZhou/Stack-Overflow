using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.System.UserProfile;

namespace UWPClassLibrary.Helpers
{
    /// <summary>
    /// http://validvoid.net/solutions-get-device-id-for-uwp/
    /// Windows Store 应用获得各种 ID 的几种方案
    /// </summary>
    public class DeviceIdHelper
    {
        /// <summary>
        /// 广告ID
        /// </summary>
        public static string AdvertisingId => AdvertisingManager.AdvertisingId;

        /// <summary>
        /// EasClientDeviceInformation
        /// Id 属性表示 DeviceId（使用从 MachineID、用户 SID1 和包系列名称 (PFN)2 的 SHA256 哈希值截断的前 16 个字符 GUID，其中 MachineID 使用本地用户组的 SID）
        /// </summary>
        public static string EasId => new EasClientDeviceInformation().Id.ToString();

        /// <summary>
        /// 应用特定硬件ID(ASHWID)
        /// </summary>
        public static string PackageSpecificToken
        {
            get
            {
                var packageSpecificToken = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = packageSpecificToken.Id;
                //IBuffer signature = packageSpecificToken.Signature;
                //IBuffer certificate = packageSpecificToken.Certificate;
                string id;
                using (var dataReader = DataReader.FromBuffer(hardwareId))
                {
                    var bytes = new byte[hardwareId.Length];
                    dataReader.ReadBytes(bytes);
                    id = BitConverter.ToString(bytes);
                }
                return id;
            }
        }

        /// <summary>
        /// 应用程序包ID
        /// </summary>
        public static PackageId PackageId => Package.Current.Id;
    }
}
