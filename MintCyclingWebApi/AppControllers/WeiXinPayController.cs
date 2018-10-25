using Autofac;
using MintCyclingService.alipay;
using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;
using MintCyclingWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MintCyclingWebApi.AppControllers
{
    [RequestCheck]
    public class WeiXinPayController : ApiController
    {
        //微信支付数据访问接口类
        IWeixinAlipayTradeService _WeiPayService;


        /// <summary>
        /// 初始化微信支付控制器
        /// </summary>
        public WeiXinPayController()
        {
            _WeiPayService = AutoFacConfig.Container.Resolve<IWeixinAlipayTradeService>();

        }


 

    }
}