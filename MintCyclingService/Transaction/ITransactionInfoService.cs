using MintCyclingService.User;
using MintCyclingService.Utils;

namespace MintCyclingService.Transaction
{
    public interface ITransactionInfoService
    {
        /// <summary>
        /// 后台根据查询条件搜索用户交易记录列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetTransactionList(UserTransaction_PM model);

        /// <summary>
        /// 手动还车详细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetUserByUserGuid(RetrunBicycle_PM model);

        /// <summary>
        /// 历史交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetHistoryUserTransactionList(HistoryUserTransaction_PM model);

        /// <summary>
        /// 用户充值押金交易记录API列表 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetUserDepositRechargeRecordList(UserDepositRecharge_PM model);

        /// <summary>
        /// 用户充值余额交易记录API列表  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetUserAccountRechargeRecordList(UserDepositRecharge_PM model);


        /// <summary>
        /// 用户退款交易记录API列表 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetUserRefundDepositRecordList(UserDepositRecharge_PM model);






    }
}