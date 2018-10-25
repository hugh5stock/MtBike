using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;
using System;

namespace MintCyclingService.UserAccount
{
    public interface IUserAccountService
    {
        /// <summary>
        /// 通过用户的Guid查询可用余额
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetAccountUsableAmountByUserGuid(MyAccountUsableAmount_PM data);

        /// <summary>
        /// 检查用户的余额
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckUserAvailableBalance(Guid userGuid, out string msg);

        /// <summary>
        /// 用户充值押金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <param name="Deposit"></param>
        ResultModel AddDeposit(AddDeposit_PM model, MintCyclingData.Deposit Deposit);
        
        /// <summary>
        /// 用户退押金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel RefundDeposit(RefundDeposit_PM model);

        /// <summary>
        /// 通过用户的Guid查询用户的押金余额
        /// </summary>
        /// <param name="UserInfoGuid"></param>
        /// <returns></returns>
        ResultModel GetUserDepositByUserGuid(Guid UserInfoGuid);

        /// <summary>
        /// 通过用户的Guid查询用户的钱包余额和押金详情
        /// </summary>
        /// <param name="UserInfoGuid"></param>
        /// <returns></returns>
        ResultModel GetUserAccountDetailByUserGuid(Guid UserInfoGuid);

        /// <summary>
        /// 通过用户UserGuid查询处理跳转界面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetUrlPageByUserInfoGuid(MyAccountUsableAmount_PM data);
        
        /// <summary>
        /// 用户充值押金或者退押金--主要用于支付宝充值成功后修改用户押金充值记录表信息
        /// </summary>
        bool UserReOrUpdateDeposit(UserReDepositUpdate_PM model);
        
        /// <summary>
        /// 用户充值押金或者退押金--主要用于微信充值成功后修改用户押金充值记录表信息
        /// </summary>
        bool UserReOrUpdateDepositToWeiXinPay(NotifyWeinXinResultModel model);
        
        /// <summary>
        /// 校验通知数据的正确性--支付宝
        /// </summary>
        /// <param name="out_trade_no">订单号码</param>
        /// <returns></returns>
        ResultModel GetUserDepositByout_trade_no(string out_trade_no,string total_amount);

        /// <summary>
        /// 校验通知数据的正确性--针对微信
        /// </summary>
        /// <param name="out_trade_no">订单号码</param>
        /// <returns></returns>
        ResultModel WeixinGetUserDepositByout_trade_no(string out_trade_no, string total_amount);
        
        /// <summary>
        /// 给用户手机发送手机信息通用接口
        /// </summary>
        /// <param name="userinfguid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UsersSendSMS(Guid userinfguid, string out_trade_no, string T, string remark);
        
        }
}