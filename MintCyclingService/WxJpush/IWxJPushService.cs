using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.WxJpush
{
    public interface IWxJpushService
    {

        /// <summary>
        /// 该API 只用于验证推送调用是否能够成功，与推送 API 的区别在于：不向用户发送任何消息。 其他字段说明：同推送 API。调用返回
        /// 说明：应用内消息。或者称作：自定义消息，透传消息。此部分内容不会展示到通知栏上，JPush SDK 收到消息内容后透传给 App。需要 App 自行处理。iOS 平台上，此部分内容在推送应用内消息通道（非APNS）获取。Windows Phone 暂时不支持应用内消息。
        /// </summary>
        /// <returns></returns>
        ResultModel GetJPushValidate();
 
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="content">请求的json参数，一般由Platform(平台)、Audience(设备对象标识)、Notification(通知)、Message(自定义消息)、Options(推送可选项)组成</param>
        /// <returns></returns>
        string SendPostRequest(String content, List<string> registrationList);

    }
}