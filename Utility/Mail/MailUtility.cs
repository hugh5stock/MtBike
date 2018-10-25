using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Utility.Mail
{
    public class MailUtility
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="targetMailAddress">目标邮箱地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件正文</param>
        /// <returns>是否发送成功</returns>
        public static bool SendMail(string targetMailAddress,string title, string content)
        {
            // Get setting data from web.config
            var server = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Server"];
            var account = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Account"];
            var password = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Password"];
            var displayAccount = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Dispaly Account"];
            var representsText = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Represents text"];
            var isBodyHtml = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["IsBodyHtml"];
            var encoding = ((NameValueCollection)ConfigurationManager.GetSection("mailSettings"))["Encoding"];

            try
            {
                var client = new SmtpClient(server, 25);

                // 出站方式设置为NetWork
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // smtp服务器验证并制定账号密码
                client.Credentials = new NetworkCredential(account, password);
                var message = new MailMessage();

                // 设置优先级
                message.Priority = MailPriority.Normal;

                // 设置收件方看到的邮件来源为 :发送方邮件地址、来源标题、编码
                message.From = new MailAddress(displayAccount, representsText, Encoding.GetEncoding(encoding));

                // 接收方 
                message.To.Add(targetMailAddress);

                message.Subject = title;
                message.SubjectEncoding = Encoding.GetEncoding(encoding);

                // 邮件正文是否支持 HTML
                message.IsBodyHtml = (isBodyHtml == "true");

                // 正文编码               
                message.BodyEncoding = Encoding.GetEncoding(encoding);

                // 正文内容
                message.Body = content;
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 发送验证邮件给用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">Email地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="vaildateUrl">验证路径</param>
        public static void SendVaildateMail(string userName, string email, string title, string vaildateUrl)
        {
            var content = new StringBuilder();
            content.Append("你好," + userName + ":<br /><br />");
            content.Append("请点击以下链接来验证你的邮件地址:<br />");
            content.Append("<a href='" + vaildateUrl + "' target='_blank'>" + vaildateUrl + "</a><br /><br />");
            content.Append("谢谢你的光临!<br /><br />");

            SendMail(email, title, content.ToString());
        }
    }

}
