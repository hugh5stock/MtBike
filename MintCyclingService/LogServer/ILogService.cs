using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.LogServer
{
  public interface ILogService
    {

        /// <summary>
        /// 开关锁交易记录日志
        /// </summary>
        /// <param name="LockNumber"></param>
        /// <param name="BicycleNumber"></param>
        /// <param name="LockOpenClose"></param>
        /// <param name="UserGuid"></param>
        /// <param name="UploadType"></param>
        /// <param name="Remark"></param>
        void InsertDBLockOC(string LockNumber, int LockOpenClose, Guid UserGuid, string UploadType, string Remark);



        /// <summary>
        /// 支付交易日志
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="PayType"></param>
        /// <param name="TradeType"></param>
        /// <param name="Amount"></param>
        /// <param name="Remark"></param>

        void InsertDBPay(Guid UserGuid, int PayType, int TradeType, string Amount, string Remark);



        /// <summary>
        /// 添加预约日志
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="ReservationType"></param>
        /// <param name="Remark"></param>

        void InsertDBReservation(Guid UserGuid, string LockNumber, int ReservationType, string Remark);






    }
}
