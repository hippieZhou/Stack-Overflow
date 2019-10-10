using System;
using System.Threading.Tasks;

namespace CSharpBase
{
    /// <summary>
    /// WCF使用通道调用服务的实现类
    /// </summary>
    public class WCFHelper
    {
        /// <summary>
        /// 调用无返回值WCF服务
        /// </summary>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="action"></param>
        public static void UseService<TChannel>(Action<TChannel> action)
        {
            ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>("*");
            //替换WCF服务主机地址
            string address = factory.Endpoint.Address.Uri.ToString().Replace(factory.Endpoint.Address.Uri.Host, GetServiceIpByConfigSection(typeof(TChannel)));
            factory.Endpoint.Address = new EndpointAddress(address);
            //创建通道
            TChannel tChannel = factory.CreateChannel();
            ((IClientChannel)tChannel).Open();
            action(tChannel);
            try
            {
                ((IClientChannel)tChannel).Close();
            }
            catch
            {
                ((IClientChannel)tChannel).Abort();
            }
        }
        /// <summary>
        /// 调用带返回值WCF服务
        /// </summary>
        /// <typeparam name="TChannel">要调用的WCF契约的类型</typeparam>
        /// <typeparam name="TReturn">服务的返回值类型</typeparam>
        /// <param name="func">执行调用的Func委托</param>
        public static TReturn UseService<TChannel, TReturn>(Func<TChannel, TReturn> func)
        {
            ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>("*");
            //替换WCF服务主机地址
            string address = factory.Endpoint.Address.Uri.ToString().Replace(factory.Endpoint.Address.Uri.Host, GetServiceIpByConfigSection(typeof(TChannel)));
            factory.Endpoint.Address = new EndpointAddress(address);
            //创建通道
            TChannel tChannel = factory.CreateChannel();
            ((IClientChannel)tChannel).Open();
            TReturn result = func(tChannel);
            try
            {
                ((IClientChannel)tChannel).Close();
            }
            catch
            {
                ((IClientChannel)tChannel).Abort();
            }
            return result;
        }
        /// <summary>
        /// 异步调用带返回值WCF服务
        /// </summary>
        /// <typeparam name="TChannel">要调用的WCF契约的类型</typeparam>
        /// <typeparam name="TReturn">服务的返回值类型</typeparam>
        /// <param name="func">执行调用的Func委托</param>
        public async static Task<TReturn> UseServiceAsync<TChannel, TReturn>(Func<TChannel, TReturn> func)
        {
            TReturn res = await Task.Run<TReturn>(() =>
            {
                ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>("*");
                //替换WCF服务主机地址
                string address = factory.Endpoint.Address.Uri.ToString().Replace(factory.Endpoint.Address.Uri.Host, GetServiceIpByConfigSection(typeof(TChannel)));
                factory.Endpoint.Address = new EndpointAddress(address);
                //创建通道
                TChannel tChannel = factory.CreateChannel();
                ((IClientChannel)tChannel).Open();
                TReturn result = func(tChannel);
                try
                {
                    ((IClientChannel)tChannel).Close();
                }
                catch
                {
                    ((IClientChannel)tChannel).Abort();
                }
                return result;
            });
            return res;
        }
        /// <summary>
        /// 从配置文件获取WCF服务主机IP地址
        /// </summary>
        /// <returns></returns>
        private static string GetServiceIpByConfigSection(Type serviceContractType)
        {
            var configException = new ConfigurationErrorsException(string.Format("配置文件中不存在契约 {0} 的终结点信息", serviceContractType));
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ClientSection clientSection = config.GetSection("system.serviceModel/client") as ClientSection;
            foreach (ChannelEndpointElement element in clientSection.Endpoints)
            {
                if (element.Contract == serviceContractType.ToString())
                {
                    return element.Address.Host;
                }
            }
            throw configException;
        }
    }
}
