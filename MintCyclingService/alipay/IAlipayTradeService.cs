using MintCyclingService.Utils;
using System.Collections.Generic;

namespace MintCyclingService.alipay
{
    /// <summary>
    /// .NET服务端SDK生成APP支付相关接口
    /// </summary>
    public interface IAlipayTradeService
    {
 
        ResultModel GetAlipayTradeAppPay(AlipayTradeModel para);

 
 
        ResultModel AlipayTradeRefund(UserDepositRefundModel para);



    }
}