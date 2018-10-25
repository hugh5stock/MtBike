using AppNotificationPushService;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Handler
{
    public class NotificationPush
    {
        public static ResultModel PushMessageToAll(string msg)
        {
            INotificationPushService service = new NotificationPushService();
            var st = service.PushNotificationToAll(msg);
            return new ResultModel { Message = st.ErrorMsg, IsSuccess = st.Success, ResObject = st.MsgId };
        }

        public static ResultModel PushMessageWithAlias(string msg, params string[] alias)
        {
            INotificationPushService service = new NotificationPushService();
            var st = service.PushNotificationToAlias(msg, alias);
            return new ResultModel { Message = st.ErrorMsg, IsSuccess = st.Success, ResObject = st.MsgId };
        }

        public static ResultModel PushMessageWithTag(string msg, params string[] tags)
        {
            INotificationPushService service = new NotificationPushService();
            var st = service.PushNotificationToTags(msg, tags);
            return new ResultModel { Message = st.ErrorMsg, IsSuccess = st.Success, ResObject = st.MsgId };
        }

        public static ResultModel PushMessageToReg(string msg, params string[] regs)
        {
            INotificationPushService service = new NotificationPushService();
            var st = service.PushNotificationToRegistrationId(msg, regs);
            return new ResultModel { Message = st.ErrorMsg, IsSuccess = st.Success, ResObject = st.MsgId };
        }
    }

    public class PushMsgModel
    {
        public string ErrorMsg { get; set; }
        public string MsgId { get; set; }
        public bool Success { get; set; }
    }
}
