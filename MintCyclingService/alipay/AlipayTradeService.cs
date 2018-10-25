using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using DTcms.Common;
using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.LogServer;
using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Utility.Common;
using Utility.LogHelper;

namespace MintCyclingService.alipay
{
    public class AlipayTradeService : IAlipayTradeService
    {
        //获取配置文件中的值
        private static string AppID = System.Web.Configuration.WebConfigurationManager.AppSettings["AppID"];                      //APPID

        private static string App_private_Key = System.Web.Configuration.WebConfigurationManager.AppSettings["App_private_Key"];         //私钥
        private static string Alipay_public_Key = System.Web.Configuration.WebConfigurationManager.AppSettings["Alipay_public_Key"];     //公钥
        private static string Charset = System.Web.Configuration.WebConfigurationManager.AppSettings["Charset"];             //编码方式
        private static string SellerID = System.Web.Configuration.WebConfigurationManager.AppSettings["SellerID"];           //收款支付宝用户ID
        private static string ProductCode = System.Web.Configuration.WebConfigurationManager.AppSettings["ProductCode"];    //销售产品码，商家和支付宝签约的产品码
        private static string notify_url = System.Web.Configuration.WebConfigurationManager.AppSettings["notify_url"];      //获取服务器异步通知页面路径
        private static string YjAmoiunt = System.Web.Configuration.WebConfigurationManager.AppSettings["YjAmount"];   //押金

        //支付宝通知验证路径
        private string Https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&";

        private IUserAccountService _UserAccountService = new UserAccountService();
        private IWeixinAlipayTradeService _WeixinService = new WeixinAlipayTradeService();

        private ILogService LogServer = new LogService();
        #region 

        /// <summary>
        /// 生成APP支付订单信息
        /// 主要用来做签名用的
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAlipayTradeAppPay(AlipayTradeModel para)
        {

            var result = new ResultModel();
            string OrderNumber = string.Empty;
            string typeName = "RechargeBalanceOrDeposit";

            //对支付宝Url编码string encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));进行解密
            //string encodedValue = HttpUtility.UrlDecode("app_id=2017030806107198&biz_content=%7b%22body%22%3a%22%e7%94%a8%e6%88%b7%e5%85%85%e5%80%bc%e9%92%b1%e5%8c%85%22%2c%22out_trade_no%22%3a%22YE201703230953530000000001%22%2c%22product_code%22%3a%22QUICK_MSECURITY_PAY%22%2c%22subject%22%3a%22%e7%94%a8%e6%88%b7%e5%85%85%e5%80%bc%22%2c%22total_amount%22%3a%22200%22%7d&charset=utf-8&format=json&method=alipay.trade.app.pay&sign_type=RSA2&timestamp=2017-03-23+09%3a54%3a13&version=1.0&sign=VUTpywXcQMm1PgGnMJPrOjBs%2fDlVAU%2fPKBfn1ODXypMC7nCc6W917%2f5KzfYd89ftQwpSH9C7vrCZra5H8Ns691Pj4EtkfAnllYVz1EAlWh2mmhe7IOUp1RGQwXyfY1d0vY4cZOEcxB9vFXGU%2f1wuS2xpQeUgup6BDtR2h9I%2bv3nffwQYOpoj8FjreyuF4vwT0orNOnV4cXGZwjLzH2wlgdct1SEFuDXtd9C1A1yb32GdsKtsvIvr1x1p0awEn2F8foe51SmsxuiELoUZxAU%2bN463tMW%2brz3b3ZrrRxwJBAZ97zrBZum%2bo%2fmm3dYJ5e6abLWYziTRtL8nE6XMSwwRAA%3d%3d", Encoding.GetEncoding(Charset));
            try
            {
                if (para == null)
                {
                    LogServer.InsertDBPay(para.UserInfoGuid, 0, 0, para.TotalAmount, "请求参数出现问题！");

                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                }
                //生成唯一的订单号
                if (para.TypeName == 1) //押金充值
                {
                    OrderNumber = "YJ" + OrderHelper.GenerateOrderNumber();
                }
                else //余额充值
                {
                    OrderNumber = "YE" + OrderHelper.GenerateOrderNumber();
                }

                IAopClient client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", AppID, App_private_Key, "json", "1.0", "RSA2", Alipay_public_Key, Charset, false);
                //实例化具体API对应的request类,类名称和接口名称对应,当前调用接口名称如：alipay.trade.app.pay

                AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();
                //SDK已经封装掉了公共参数，这里只需要传入业务参数。以下方法为sdk的model入参方式(model和biz_content同时存在的情况下取biz_content)。
                AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
                model.Body = para.Body;
                model.Subject = para.Subject;
                model.TotalAmount = para.TotalAmount;
                model.ProductCode = ProductCode;
                model.OutTradeNo = OrderNumber;       //订单号
                model.TimeoutExpress = "30m";         //默认1小时
                model.SellerId = SellerID;
                request.SetBizModel(model);
                request.SetNotifyUrl(notify_url);   //异步通知路径-商户外网可以访问的异步地址
                //这里和普通的接口调用不同，使用的是sdkExecute
                AlipayTradeAppPayResponse response = client.SdkExecute(request);

                if (!string.IsNullOrEmpty(response.Body))
                {
                }
                else
                {
                    LogServer.InsertDBPay(para.UserInfoGuid, 0,1, para.TotalAmount, "生成APP支付订单信息失败！");
                    result.IsSuccess = false;
                    result.MsgCode = "1";
                    result.Message = "生成APP支付订单信息失败！";
                    result.ResObject = null;
                
                }
            }
            catch (Exception ex)
            {
                //FileLog.Log("用户充值异常信息：当前用户Guid为" + para.UserInfoGuid + ",服务器生成订单信息异常!", typeName);
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AlipayServiceErrorCode, Message = ResPrompt.AlipayServiceErrorMessageMessage };
            }
            return result;
        }

        #endregion



 
        #region 集成支付宝退押金接口


        public ResultModel AlipayTradeRefund(UserDepositRefundModel para)
        {
            string typeName = "UserRefundDeposit";
            IUserAccountService userService = new UserAccountService();
            var result = new ResultModel();
            AlipayTradeRefundModel userRefund = new AlipayTradeRefundModel();

            using (var db = new MintBicycleDataContext())
            {
                //新增判断
                var QueryRe = db.Reservation.OrderByDescending(r => r.CreateTime).FirstOrDefault(r => r.UserInfoGuid == para.UserInfoGuid && r.Status == 1);
                if (QueryRe != null)
                {
                    LogServer.InsertDBPay(para.UserInfoGuid, 0, 2,"押金", "您在预约用车中，不能退押金！");
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "您在预约用车中，不能退押金！" };
                }

                    var DepositQueryLog = db.UserDepositRechargeRecord.OrderByDescending(s=>s.CreateTime).FirstOrDefault(s => s.UserInfoGuid == para.UserInfoGuid && s.Status==1 && s.MoneyType=="1");
                if (DepositQueryLog != null)
                {
                  
                }
            }
            return result;
        }

        #endregion 集成支付宝退押金接口



        #region 

        public bool Verify(IDictionary<string, string> inputPara, string notify_id, string sign)
        {

            string responseTxt = "true";
            if (notify_id != "")
            {
                responseTxt = GetResponseTxt(notify_id);
            }

            if (responseTxt == "true")//验证成功
            {
                return true;
            }
            else//验证失败
            {
                return false;
            }
        }

        /// <summary>
        /// 获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notify_id">通知验证ID</param>
        /// <returns>验证结果</returns>
        private string GetResponseTxt(string notify_id)
        {
            string veryfy_url = Https_veryfy_url + "partner=" + SellerID + "&notify_id=" + notify_id;

            string responseTxt = Get_Http(veryfy_url, 120000);  //2分钟120秒

            return responseTxt;
        }

        /// <summary>
        /// 获取远程服务器ATN结果
        /// </summary>
        /// <param name="strUrl">指定URL路径地址</param>
        /// <param name="timeout">超时时间设置</param>
        /// <returns>服务器ATN结果</returns>
        private string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }
                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }
            return strResult;
        }
        public static IDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            //创建泛型哈希表，然后加入元素
            IDictionary<string, string> sArray = new Dictionary<string, string>(); 
            NameValueCollection coll;
            //表单变量加载到NameValueCollection变量
            coll = System.Web.HttpContext.Current.Request.Form;

            //得到所有形式的名字添加到一个字符串数组
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], System.Web.HttpContext.Current.Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 交易状态说明
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static int GetTradeStatus(string T)
        {
            //充值的状态：0充值失败()；1充值成功--TRADE_SUCCESS；2交易创建，等待买家付款--WAIT_BUYER_PAY；3.未付款交易超时关闭，或支付完成后全额退款--TRADE_CLOSED；4.交易结束，不可退款--TRADE_FINISHED
            int str = 0;
            switch (T)
            {
                case "TRADE_SUCCESS":
                    str = 1;
                    break;

                case "WAIT_BUYER_PAY":
                    str = 2;
                    break;

                case "TRADE_CLOSED":
                    str = 3;
                    break;

                case "TRADE_FINISHED":
                    str = 4;
                    break;

                default:
                    str = 0;
                    break;
            }
            return str;
        }

        #endregion 支付宝异步回调验证签名和验证消息是否是支付宝发出的 暂时废弃

    }
}