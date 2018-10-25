using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;

namespace MintCyclingService.UserRecharge
{
    public interface IUserRechargeRecordService
    {
        /// <summary>
        /// 用户充值余额记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel AddUserRechargeRecord(UserRechargeRecordModel_PM model);

        /// <summary>
        /// 修改用户账户余额订单状态信息--针对支付宝
        /// </summary>
        bool UserRechargeOrAccountInfo(UserReDepositUpdate_PM model);

        /// <summary>
        /// 修改用户账户余额订单状态信息--针对微信
        /// </summary>
        bool UserRechargeOrAccountInfoWeiXinPay(NotifyWeinXinResultModel model);
   
            /// <summary>
            /// 校验通知数据的正确性
            /// </summary>
            /// <param name="out_trade_no">订单号码</param>
            /// <returns></returns>
            ResultModel GetUserRechargeRecordByout_trade_no(string out_trade_no, string total_amount);

        
            /// <summary>
            /// 校验通知数据的正确性
            /// </summary>
            /// <param name="out_trade_no">订单号码</param>
            /// <returns></returns>
            ResultModel WeixinGetUserRechargeRecordByout_trade_no(string out_trade_no, string total_amount);

    }
}