using HuRongClub.DBUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Utility.Common;

namespace MintCyclingService.LogServer
{
   public   class LogService:ILogService
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
        public   void InsertDBLockOC(string LockNumber,int LockOpenClose, Guid UserGuid, string  UploadType, string Remark)
        {

            Task.Factory.StartNew(() =>
            {

                //string SQL = string.Format("INSERT  INTO LockOpenCloseLog(LockNumber,LockOpenClose,UserGuid,UploadType,Remark,CreateTime) VALUES ('{0}',{1},'{2}','{3}','{4}','{5}')", LockNumber,LockOpenClose, UserGuid, UploadType, Remark,DateTime.Now);

                SqlParameter[] pa = new SqlParameter[] { new SqlParameter("@LockNumber", LockNumber), new SqlParameter("@LockOpenClose", LockOpenClose), new SqlParameter("@UserGuid", UserGuid), new SqlParameter("@UploadType", UploadType), new SqlParameter("@Remark", Remark), new SqlParameter("@CreateTime", DateTime.Now) };



                LogHelpSql.RunProcedure("LockOClog",pa);

            });

        }
        /// <summary>
        /// 支付交易日志
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="PayType"></param>
        /// <param name="TradeType"></param>
        /// <param name="Amount"></param>
        /// <param name="Remark"></param>

        public void InsertDBPay(Guid UserGuid,int PayType,int TradeType, string  Amount, string  Remark)
        {

            Task.Factory.StartNew(() =>
            {

                //string SQL = string.Format("INSERT  INTO PayLog(UserGuid,PayType,TradeType,Amount,Remark,CreateTime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", UserGuid, PayType, TradeType, Amount, Remark, DateTime.Now);
                SqlParameter[] pa = new SqlParameter[] { new SqlParameter("@UserGuid",UserGuid),new SqlParameter("@PayType",PayType),new  SqlParameter("@TradeType",TradeType),new SqlParameter("@Amount",Amount),new SqlParameter("@Remark",Remark),new SqlParameter ("@CreateTime",DateTime.Now) };
     
                LogHelpSql.RunProcedure("DBPayLog",pa);

            });

        }
        /// <summary>
        /// 添加预约日志
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="ReservationType"></param>
        /// <param name="Remark"></param>

        public void InsertDBReservation(Guid UserGuid, string  LockNumber, int ReservationType, string  Remark)
        {

            Task.Factory.StartNew(() =>
            {

                //string SQL = string.Format("INSERT  INTO ReservationLog(UserGuid,LockNumber,ReservationType,Remark,CreateTime) VALUES ('{0}','{1}','{2}','{3}','{4}')", UserGuid, LockNumber, ReservationType, Remark,DateTime.Now);


                SqlParameter[] pa = new SqlParameter[] { new SqlParameter("@UserGuid", UserGuid), new SqlParameter("@LockNumber", LockNumber), new SqlParameter("@ReservationType", ReservationType), new SqlParameter("@Remark", Remark), new SqlParameter("@CreateTime", DateTime.Now) };

                LogHelpSql.RunProcedure("DBReservation", pa);

            });

        }

    }
}
