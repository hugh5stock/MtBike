using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Breakdown
{
    public enum BreakdownEnum
    {
        //0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
        /// <summary>
        /// 无法开关锁
        /// </summary>
        Breakdown_A,

        /// <summary>   
        /// 无法骑行
        /// </summary>
        Breakdown_B,

        /// <summary>
        /// 无法结算
        /// </summary>
        Breakdown_C,
        /// <summary>
        /// 二维码受损
        /// </summary>
        Breakdown_D,

        /// <summary>
        /// 其它问题
        /// </summary>
        Breakdown_E,
        /// <summary>
        /// 全部
        /// </summary>
        BreakDown_ALL

    }

    /// <summary>
    /// 故障状态值
    /// </summary>
    public class BreakDownState
    {
        public static string GetBreakDownState(int st)
        {
            var str = string.Empty;
            switch (st)
            {
                case (int)BreakdownEnum.Breakdown_A:
                    str = "无法开关锁";
                    break;

                case (int)BreakdownEnum.Breakdown_B:
                    str = "无法骑行";
                    break;
                case (int)BreakdownEnum.Breakdown_C:
                    str = "无法结算";
                    break;
                case (int)BreakdownEnum.Breakdown_D:
                    str = "二维码受损";
                    break;

                default:
                    str = "其它问题";
                    break;
            }
            return str;
        }
    }


    public enum ServiceStatus
    {
        /// <summary>
        /// 故障
        /// </summary>
        BreakDown,
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 回仓库
        /// </summary>
        GoWare,
        /// <summary>
        /// 报废
        /// </summary>
        scrap

    }

    /// <summary>
    /// 等级名称
    /// </summary>
    public enum GreaGradeNameEnum
    {
        /// <summary>
        /// 第一等级
        /// </summary>
        GreaGradeName_A,

        /// <summary>
        /// 第二等级
        /// </summary>
        GreaGradeName_B,

        /// <summary>
        /// 第三等级
        /// </summary>
        GreaGradeName_C,


    }

    /// <summary>
    /// 等级的状态值
    /// </summary>
    public class GreaGradeNameState
    {
        public static string GetGradeNameState(int st)
        {
            var str = string.Empty;
            switch (st)
            {
                case (int)GreaGradeNameEnum.GreaGradeName_A:
                    str = "第一等级";
                    break;

                case (int)GreaGradeNameEnum.GreaGradeName_B:
                    str = "第二等级";
                    break;

                default:
                    str = "第三等级";
                    break;
            }
            return str;
        }
    }



    public class ServiceState
    {

        public static string GetServiceState(int str)
        {
            var res = string.Empty;

            switch (str)
            {
                case (int)ServiceStatus.BreakDown:
                    res = "故障";
                    break;
                case (int)ServiceStatus.GoWare:
                    res = "回仓库";
                    break;
                case (int)ServiceStatus.Normal:
                    res = "修好";
                    break;

                case (int)ServiceStatus.scrap:
                    res = "报废";
                    break;

                default:
                    res = "其他";
                    break;



            }

            return res;








        }





    }



}
