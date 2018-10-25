using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.alipay;
using MintCyclingService.Common;
using MintCyclingService.LogServer;
using MintCyclingService.UserAccount;
using MintCyclingService.UserRecharge;
using MintCyclingService.Utils;
using System;
using System.Data;
using System.Linq;
using System.Text;
using Utility.Common;
using Utility.LogHelper;
using WxPayAPI;

namespace MintCyclingService.WeixinApliay
{
    /// <summary>
    /// 微信支付服务类
    /// </summary>
    public class WeixinAlipayTradeService : IWeixinAlipayTradeService
    {
        //获取配置文件中的值  
      
        private static string YjAmoiunt = System.Web.Configuration.WebConfigurationManager.AppSettings["YjAmount"];       //押金
        //private static string Sslcert_path = System.Web.Configuration.WebConfigurationManager.AppSettings["Sslcert_path"];       //证书路径
 


        //接口实例化对象
        private IUserAccountService _UserAccountService = new UserAccountService();
        private IUserRechargeRecordService _UserRechargeService = new UserRechargeRecordService();
        private ILogService LogService = new LogService();
 

    }
}