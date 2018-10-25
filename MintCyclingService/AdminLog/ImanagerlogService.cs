
using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.AdminLog
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public interface ImanagerlogService
    {
        /// 增加一条数据
        /// </summary>
        int AddManagerLog(dt_manager_log model);


        ///// <summary>
        ///// 删除7天前的日志数据
        ///// </summary>
        int Delete(int dayCount);



        
        }
}