using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel.UserDataAccounts;
using Windows.Devices.Geolocation;
using Windows.Devices.Input;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.NetworkOperators;
using Windows.Phone.Management.Deployment;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.Display;
using Windows.System.Profile;
using Windows.System.UserProfile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPClassLibrary.Helpers
{
    /// <summary>
    /// http://validvoid.net/uwp-system-info-collect-1/
    /// http://validvoid.net/uwp-system-info-collect-2/
    /// 获取各类系统、用户信息 (1) - 设备和系统的基本信息、应用包信息、用户数据账户信息和用户账户信息
    /// </summary>
    public class DeviceInfoHelepr
    {
        #region AnalyticsInfo：Windows.System.Profile
        /// <summary>
        /// 使用未知
        /// </summary>
        public static string DeviceForm => AnalyticsInfo.DeviceForm;
        /// <summary>
        /// 获取设备类型
        /// </summary>
        public static string DeviceFamily => AnalyticsInfo.VersionInfo.DeviceFamily;
        /// <summary>
        /// 获取系统版本号
        /// 注意事项：需要指出的是，如果你打算通过 DeviceFamilyVersion 进行数据统计、分析工作，那么在应用的客户端代码中不要将原始的 DeviceFamilyVersion 返回值格式化为可读形式。据微软官方人员在 MSDN 的解释，AnalyticsInfo.VersionInfo 旨在为遥测和日志记录提供一个不透明的版本号字符串值，最佳做法是将该原始值传回服务器，如果有必要，在服务器端进行格式化解析的工作。
        /// </summary>
        public static string DeviceFamilyVersion
        {
            get
            {
                var sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                var v = ulong.Parse(sv);
                var v1 = (v & 0xFFFF000000000000L) >> 48;
                var v2 = (v & 0x0000FFFF00000000L) >> 32;
                var v3 = (v & 0x00000000FFFF0000L) >> 16;
                var v4 = (v & 0x000000000000FFFFL);
                return $"{v1}.{v2}.{v3}.{v4}";
            }
        }


        #endregion

        #region 资源限定符：Windows.ApplicationModel.Resources.Core.ResourceContext
        /// <summary>
        /// https://msdn.microsoft.com/zh-cn/library/windows/apps/xaml/windows.applicationmodel.resources.core.resourcecontext.aspx
        /// 该值是通过键值对的方式来进行访问;
        /// keys:Language,Contrast,Scale,HomeRegion,TargetSize,LayoutDirection…………
        /// </summary>
        public static IObservableMap<string, string> QualifierValues => ResourceContext.GetForCurrentView().QualifierValues;
        #endregion

        #region 应用包信息：Windows.ApplicationModel.Package
        /// <summary>
        /// IsBundle 指示该包是否为 Bundle 集合包。
        /// IsDevelopmentMode 指示该包是否以开发模式安装。
        /// IsFramework 指示是否有其它包将该包声明为依赖项。
        /// Id
        /// {
        ///     Architecture: 获取当前包的对应处理器架构。
        ///     Author: 获取包作者。仅限 Windows Phone，在 Windows 10 上无效。
        ///     ProductId: 获取包的 ProductID 属性值。仅限 Windows Phone，在 Windows 10 上无效。
        ///     ……………
        /// }
        /// …………
        /// </summary>
        public static Package Package => Package.Current;
        #endregion

        #region 列举Windows Mobile设备上已部署的应用包，仅在 Windows Mobile 设备上有效（需要添加对Mobile的扩展引用）:Windows.Phone.Management.Deployment
        /// <summary>
        /// 获取同一发布者的应用包部署情况同一发布者的应用包部署情况
        /// 在 UWP 应用中使用时，需要配合 AnalyticsInfo.VersionInfo.DeviceFamily 检测当前设备类型，选择是否调用该命名空间下的方法。另外，'InstallationManager' 类中提供的其它方法需要 ID_CAP_OEM_DEPLOYMENT 特别权限才能够正常使用，故一般开发者无法使用。
        /// </summary>
        public static IEnumerable<Package> FindPackagesForCurrentPublisher => InstallationManager.FindPackagesForCurrentPublisher();
        #endregion

        #region 获取用户数据账户信息:Windows.ApplicationModel.UserDataAccounts
        /// <summary>
        /// 获取用户数据账户信息
        /// 要使用 UserDataAccounts 相关 API，要求应用在清单文件中声明联系人(contacts)、预约(appointments)、邮件(email)等功能中的一个或多个。
        /// </summary>
        /// <param name="userDataAccountStoreAccessType">用于指定要求的用户数据账户存储区的访问类型</param>
        /// <returns>用户数据账户的储存区</returns>
        public static async Task<IReadOnlyList<UserDataAccount>> FindAllAccountsAsync(UserDataAccountStoreAccessType userDataAccountStoreAccessType)
        {
            try
            {
                var userDataAccountStore = await UserDataAccountManager.RequestStoreAsync(userDataAccountStoreAccessType);
                var userDataAccounts = await userDataAccountStore.FindAccountsAsync();
                return userDataAccounts.Any() ? userDataAccounts : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region 获取系统用户信息(注意使用该 API 需要应用在清单文件中配置“用户账户信息”(User Account Information) 功能。):Windows.System.UserProfile 
        /// <summary>
        /// 获取系统用户信息
        /// </summary>
        /// <param name="userType">账户类型</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<User>> FindAllUsersAsync(UserType userType)
        {
            try
            {
                var users = await User.FindAllAsync(userType);
                return users.Any() ? users : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region 商店授权信息：Windows.ApplicationModel.Store.CurrentApp(Simulator)
        /// <summary>
        /// 获取应用ID，注意区分市场活动ID和广告ID
        /// <param name="isStoreApp">是否为商店应用</param>
        /// <returns></returns>
        public static Guid CurrentAppId
        {
            get
            {
                try
                {
                    return CurrentApp.AppId;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"GetCurrentAppId：{ex.Message}");
                    return CurrentAppSimulator.AppId;
                }
            }
        }
        #endregion

        #region 广告ID:Windows.System.UserProfile
        /// <summary>
        /// 获取广告ID
        /// 广告 ID 是每用户、每设备唯一的
        /// 广告 ID 可以被用户通过系统设置关闭
        /// 广告 ID 不具有持久性，在不同情况下会发生改变
        /// 广告 ID 在儿童账户中是关闭的
        /// </summary>
        public static string AdvertisingId => AdvertisingManager.AdvertisingId;
        #endregion

        #region EAS设备信息：Windows.Security.ExchangeActiveSyncProvisioning
        /// <summary>
        /// 获取EAS相关信息
        /// 设备名称
        /// 设备ID
        /// 操作系统
        /// 系统固件版本号
        /// 硬件版本号
        /// 设备制造商
        /// 设备型号名
        /// </summary>
        private static EasClientDeviceInformation _easClientDeviceInformation;
        public static EasClientDeviceInformation EasClientDeviceInformation => _easClientDeviceInformation ?? (_easClientDeviceInformation = new EasClientDeviceInformation());

        #endregion

        #region 移动网络信息:Windows.Networking.NetworkOperators 
        /// <summary>
        /// 该命名空间下提供的所有功能特性属于微软合作伙伴 API。这意味着开发者需要从微软获得特定的私有权限才能在应用中调用这些 API 并正常工作
        /// </summary>
        public static IReadOnlyList<string> AvailableNetworkAccountIds => MobileBroadbandAccount.AvailableNetworkAccountIds;
        #endregion

        public static CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        /// <summary>
        /// 是否允许屏幕一直处于点亮状态
        /// </summary>
        /// <param name="isOn"></param>
        public static void DisPlayScreen(bool isOn)
        {
            var dispRequest = new DisplayRequest();
            if (isOn)
            {
                dispRequest.RequestActive();
            }
            else
            {
                dispRequest.RequestRelease();
            }
        }

        /// <summary>
        /// 可视区域相关信息
        /// </summary>
        public static ApplicationView CurrentView => ApplicationView.GetForCurrentView();

        /// <summary>
        /// Gets the current physical display information.
        /// </summary>
        public static DisplayInformation DisplayInformation => DisplayInformation.GetForCurrentView();

        /// <summary>
        /// 获取屏幕实际分辨率，不建议使用WebView来通过脚本获取，因为这样会导致使用UI线程
        /// </summary>
        public static string ScreenResolution
        {
            get
            {
                var strOs = new EasClientDeviceInformation().OperatingSystem;
                if (strOs.Equals("WINDOWS"))
                {
                    var pointerDevices = PointerDevice.GetPointerDevices().ToList();
                    var rect = pointerDevices.FirstOrDefault().PhysicalDeviceRect;
                    return $"{pointerDevices.FirstOrDefault().PhysicalDeviceRect.Height}*{pointerDevices.FirstOrDefault().PhysicalDeviceRect.Width}";
                }
                else if (strOs.Equals("WindowsPhone"))
                {
                    var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    return $"{Math.Round(Window.Current.Bounds.Width * dpiRatio)}*{Math.Round(Window.Current.Bounds.Height * dpiRatio)}";
                }
                else
                {
                    return "未知屏幕分辨率";
                }
            }
        }

        /// <summary>
        /// 获取设备的地理位置
        /// </summary>
        /// <returns></returns>
        public static async Task<Tuple<double?, double, double>> GetCurrentLocation()
        {
            try
            {
                var geoposition = await new Geolocator().GetGeopositionAsync();
                //海拔
                var altitude = geoposition.Coordinate.Point.Position.Altitude;
                //经度
                var longitude = geoposition.Coordinate.Point.Position.Longitude;
                //维度
                var latitude = geoposition.Coordinate.Point.Position.Latitude;
                return new Tuple<double?, double, double>(altitude, longitude, latitude);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Tuple<double?, double, double>(null, 0, 0);
            }
        }
    }
}
