using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Common
{
    /// <summary>
    /// 单车锁状态枚举
    /// </summary>
    public enum BicycleStatusEnum
    {
        /// <summary>
        /// 开锁
        /// </summary>
        OpenLockStatus,

        /// <summary>
        /// 关锁
        /// </summary>
        CloseLockStatus,
 
        /// <summary>
        /// 故障
        /// </summary>
        fault,
 
        /// <summary>
        /// 电量不足
        /// </summary>
        LowBattery,

 
    }

    public class EnumRemarksHandler
    {
        /// <summary>
        /// 查询单车锁状态
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string GetBicycleState(int st)
        {
            var str = string.Empty;
            switch (st)
            {
                case (int)BicycleStatusEnum.OpenLockStatus:
                    str = "已开锁";
                    break;
                case (int)BicycleStatusEnum.CloseLockStatus:
                    str = "已关锁";
                    break;
 
                case (int)BicycleStatusEnum.fault:
                    str = "故障";
                    break;

                case (int)BicycleStatusEnum.LowBattery:
                    str = "电量不足";
                    break;

                 
                default:
                    str = "未知状态";
                    break;
            }
            return str;
        }



    }





}
