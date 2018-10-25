using MintCyclingService.Common;
using MintCyclingService.SMSService;
using MintCyclingService.Utils;


namespace Utility.SMS
{
    public class SMSUtility
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phoneNo">手机号码</param>
        /// <param name="msg">消息</param>
        /// <returns>发送短信结果</returns>
        public static ResultModel SendSMS(string phoneNo, string msg)
        {
            var result = new ResultModel();
            var res = new SMSResponse { };

            using (var client = new SMSServiceClient())
            {
                var smsRequest = new SMSRequest();
                smsRequest.MsgSupplier = "SMS3";
                smsRequest.ToNumber = phoneNo;
                smsRequest.MessageContent = msg;

                res = client.SendMessage(smsRequest);
            }

            if (res == null)
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.SMSHaveNotReturnMessage;
                result.Message = ResPrompt.SMSHaveNotReturnMessageMessage;
                return result;
            }

            if (res.RetMessage == "109")
            {
                res.RetMessage = "短信平台网络忙, 请稍后再试";
            }

            result.IsSuccess = res.Success;
            result.Message = res.RetMessage;

            return result;
        }
    }
}
