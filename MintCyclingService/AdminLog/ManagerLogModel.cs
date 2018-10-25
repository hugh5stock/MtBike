using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.AdminLog
{
    /// <summary>
    /// 单车入库输入参数模型
    /// </summary>
    public class AddManagerLog_PM
    {
        /// <summary>
        /// 维修人员Guid
        /// </summary>
        public Guid ServicePersonID { get; set; }

        /// <summary>
        /// 自行车编号
        /// </summary>

        public string BicycleNumber { get; set; }

         /// <summary>
         /// 0非助力模式，1助力模式
         /// </summary>
        public int BicycleType { get; set; }
 
    }

   




}
