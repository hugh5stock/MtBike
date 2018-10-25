using Autofac;
using MintCyclingService.alipay;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MintCyclingWebApi.AppControllers
{
    [RequestCheck]
    public class AlipayController : ApiController
    {
        IAlipayTradeService _alipayService;


        /// <summary>
        /// 初始化支付宝支付控制器
        /// </summary>
        public AlipayController()
        {
            _alipayService = AutoFacConfig.Container.Resolve<IAlipayTradeService>();

        }


        /// <summary>
        /// 生成APP支付订单信息及签名  complete TOM
        /// DATE:2017-03-23
        /// </summary>
        /// <returns>类别：TypeName =1表示押金充值；TypeName=2表示余额充值</returns>
        [HttpPost]
        public ResultModel GetAlipayTradeAppPay([FromBody]AlipayTradeModel para)
        {
            return _alipayService.GetAlipayTradeAppPay(para);
        }


      
        /// <summary>
        /// 用户退押金集成了支付宝和微信接口 complete TOM
        /// DATE:2017-03-25
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel DepositAlipayTradeRefund([FromBody]UserDepositRefundModel para)
        {

            return _alipayService.AlipayTradeRefund(para);
        }



    }
}