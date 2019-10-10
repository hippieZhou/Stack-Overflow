using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Email;
using Windows.System;

namespace UWPClassLibrary.Helpers
{
    public class FeatureHelper
    {
        //打电话
        public static void CallFunc()
        {
            try
            {
                PhoneCallManager.ShowPhoneCallUI(18317722768.ToString(), "hippieZhou");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //发短信
        public static async void MsgFunc()
        {
            try
            {
                var chatMessage = new ChatMessage();
                chatMessage.Recipients.Add("18317722768");
                chatMessage.Body = "hello world";
                await ChatMessageManager.ShowComposeSmsMessageAsync(chatMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        //发邮件
        public static async void EmailFunc()
        {
            //解析消息体
            var msg = new EmailMessage
            {
                Body = "hello world"
            };
            msg.To.Add(new EmailRecipient("hippiezhou@outlook.com"));
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }
        //跳转链接
        public static async void UrlFunc()
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/hippieZhou"));
        }
    }
}
