using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPClassLibrary.Helpers
{
    //https://msdn.microsoft.com/zh-cn/library/windows/apps/mt244351.aspx
    /// <summary>
    /// 应用程序缓存管理类
    /// </summary>
    public class CacheHelper
    {
        public static string CachePath = "CustomCache";

        private static CacheHelper _current;
        public static CacheHelper Current => _current ?? (_current = new CacheHelper());

        private CacheHelper()
        {
            try
            {
                var folder = ApplicationData.Current.TemporaryFolder;
                Task.Run(async () =>
                {
                    await folder.CreateFolderAsync(CachePath, CreationCollisionOption.OpenIfExists);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 创建图片缓存(图片格式为png)
        /// </summary>
        /// <param name="fileName">图片名称</param>
        /// <param name="fileUrl">图片地址</param>
        public async void WriteCacheAsync(string fileName, string fileUrl)
        {
            try
            {
                fileName = fileName.Contains(".png") ? fileName : $"{fileName}.png";
                var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(CachePath, CreationCollisionOption.OpenIfExists);
                var item = await folder.TryGetItemAsync(fileName);
                if (item == null)
                {
                    var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var client = new HttpClient())
                    {
                        var bytes = await client.GetByteArrayAsync(fileUrl);
                        var buffer = bytes.AsBuffer();
                        await FileIO.WriteBufferAsync(file, buffer);
                    }
                }
                else
                {
                    Debug.WriteLine("缓存文件已存在，不再创建缓存");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="fileName"></param>
        public async Task<BitmapImage> ReadCacheAsync(string fileName)
        {
            try
            {
                fileName = fileName.Contains(".png") ? fileName : $"{fileName}.png";
                var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(CachePath, CreationCollisionOption.OpenIfExists);
                var item = await folder.TryGetItemAsync(fileName);
                if (item != null)
                {
                    var file = await folder.GetFileAsync(fileName);
                    using (var stream = await file.OpenReadAsync())
                    {
                        var bi = new BitmapImage();
                        await bi.SetSourceAsync(stream);
                        return bi;
                    }
                }
                else
                {
                    Debug.WriteLine("缓存文件读取失败，未找到对应缓存文件！");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
